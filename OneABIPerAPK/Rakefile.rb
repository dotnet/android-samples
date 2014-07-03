require 'fileutils'
require 'httpclient'
require 'rubygems'
require 'rexml/document'

@supported_abis  = ["armeabi", "x86", "armeabi-v7a"]

task :default => [:clean, :build, :sign]

desc "Removes build artifacts"
task :clean do
  directories_to_delete = [ "bin", "obj" ]
  directories_to_delete.each { |x|
    rm_rf x, :verbose=>false
  }
  @supported_abis.each { |abi| 
    rm_rf "bin.#{abi}", :verbose=>false
    rm_rf "obj.#{abi}", :verbose=>false
    rm_rf "./Properties/AndroidManifest.xml.#{abi}", :verbose=>false
  }
end

desc "Compiles the project, creating one APK for each ABI that is supported."
task :build =>[:clean] do
  versionName = "1.2.0"
  # Explanation of this version number:
  #   08  - is the minimum API level (API 8 or higher)
  #   14  - the screen sizes supported (from small to extra large)
  #   120 - the version name, but without the dots.
  version="0814" << versionName.gsub('.', '')

  @supported_abis.each { |abi| 
    # Step One: Determine the android:versionCode value for each ABI. The first digit in the version code
    # defines the ABI that is supported by the APK.
    versionCode = "1"
    if abi == "armeabi" then
      versionCode = "1" << version
    elsif abi == "armeabi-v7a" then
      versionCode = "2" << version
    elsif abi == "x86" then
      versionCode = "6" << version
    end

    # Step Two: We need to update the versionCode AndroidManifest.XML before each
    # build. Here we load AndroidManifest.XML, set the versionCode, and then
    # save the changes to AndroidManifest.XML.
    doc = REXML::Document.new(File.new("Properties/AndroidManifest.xml"))
    root = doc.root
    root.attributes["android:versionCode"] = versionCode
    root.attributes["android:versionName"] = versionName

    # We could also create a custom AndroidManifest XML, and then
    # specify the path to this custom file by using
    # /p:AndroidManifest=Path\To\AndroidManifest.XML for XBuild
    build_manifest = "Properties/AndroidManifest.xml" << ".#{abi}"
    file = File.open(build_manifest, "w")
    formatter = REXML::Formatters::Default.new
    formatter.write(doc, file)
    file.close
    puts "==> Building an APK for ABI #{abi} with ./#{build_manifest}, android:versionCode = #{versionCode}."

    #Step Three: Build one APK per ABI.
    `xbuild /t:SignAndroidPackage /p:AndroidSupportedAbis=#{abi} /p:IntermediateOutputPath=obj.#{abi}/ /p:AndroidManifest=#{build_manifest} /p:OutputPath=bin.#{abi}   /p:Configuration=Release HelloWorld.csproj`

    # Make sure to sign the APK with your keystore, and then zipalign it.
    `jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore ~/work/keystores/xample.keystore -storepass password -signedjar bin.#{abi}/xamarin.helloworld-signed.apk bin.#{abi}/com.xamarin.multipleapk.helloworld.apk publishingdoc`
    `zipalign -f -v 4 bin.#{abi}/xamarin.helloworld-signed.apk bin.#{abi}/xamarin.helloworld.apk`
  }
end