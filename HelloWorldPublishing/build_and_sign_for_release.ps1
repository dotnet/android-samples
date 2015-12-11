# This is a sample Powershell script for compiling, signing, and then zipaliging a Mono for Android APK file.
# This does assume that the JDK and the Android SDK are in their default locations, and that the necessary changes have
# be made to the Release build target in VS2010.
#
# This script also assumes that there is a keystore file name xample.keystore.  
#
# The signed and zipaligned APK will be found in ./helloworld.apk.

#### BEGIN BUILDING
# First clean the Release target.
msbuild.exe HelloWorld.csproj /p:Configuration=Release /t:Clean

# Now build the project, using a the Release target.
msbuild.exe HelloWorld.csproj /p:Configuration=Release /t:PackageForAndroid

# At this point there is only the unsigned APK - sign it.
# The script will pause here as jarsigner prompts for the password.
# It is possible to provide they keystore password for jarsigner.exe by adding an extra command line parameter -storepass, for example
#    -storepass <MY_SECRET_PASSWORD>
# If this script is to be checked in to source code control then it is not recommended to include the password as part of this script.
& 'C:\Program Files\Java\jdk1.6.0_24\bin\jarsigner.exe' -verbose -sigalg MD5withRSA -digestalg SHA1  -keystore ./xample.keystore -signedjar ./bin/Release/mono.samples.helloworld-signed.apk ./bin/Release/mono.samples.helloworld.apk publishingdoc

# Now zipalign it.  The -v parameter tells zipalign to verify the APK afterwards.
& 'C:\Program Files\Android\android-sdk\tools\zipalign.exe' -f -v 4 ./bin/Release/mono.samples.helloworld-signed.apk ./helloworld.apk

# Done!  Do a quick list of APK's in this directory.
ls *.apk