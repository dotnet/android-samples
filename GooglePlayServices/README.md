This solution contains binding for Google Play Services SDK (which can be
installed via Android SDK Manager).

Note that this solution DOES NOT BUILD until unless you make some changes
in the project[*1] to make it usable:

- You will need to install the Google Play services package from the Android SDK Manager (located under extras)  
  if you have not done so already.

- Build the Google Play Services SDK binaries, using eclipse or ant.
  The library is located at:
  $(ANDROID_SDK)/extras/google/google_play_services/libproject/google-play-services_lib.

	Run the following in the directory mentioned above to build with ant:

		- $(ANDROID_SDK)/tools/android update project -p .
		- ant debug

- In the GooglePlayServices project, add the "project.properties" file[*2] from Google's Android SDK.  
  (You will have to use "add as a link" or MfA won't find the actual library files.)

- In the GooglePlayServices project, copy or create a symbolic link to the "docs" directory.[*3]  
  It will be used by GooglePlayServices binding project to get parameter names.

- In GooglePlayServicesTest project, open Properties/AndroidManifest.xml and replace  
  android:value="INSERT YOUR API KEY HERE" with your API Key.

  For information on obtaining a Google Maps API key, see 
  	- https://developers.google.com/maps/documentation/android/start
  	- http://docs.xamarin.com/Android/Guides/Platform_Features/Maps_and_Location/Obtaining_a_Google_Maps_API_Key
  	  (note that the doc is about Maps API v1, so only the description on the key location applies here.)
  
    To add a Simple API Access Key without specifying an Android app, do the following:
   - Create a new or access an existing API Project at: https://code.google.com/apis/console
   - Click on the 'Services' tab on the left, and enable "Google Maps Android API v2".
   - Click on the 'API Access' tab on the left, and select "Create new Android Key...".
   - Click the "Create" button without specifying a certificate fingerprint and package name.
   - The newly generated API Key should appear under 'Simple API Access' and be ready for use
   	 within this sample or your own.


[*1] We cannot distribute the prebuilt dll because the library
is not freely distributable and there is no way to support required
resources bundled with it (if it were only *.jar then we could use InputJar).

(The Google samples are Apache licensed, so we ported some of them
to C# and included in this project.)

[*2] $(ANDROID_SDK)/extras/google/google-play-services/libproject/google-play-services_lib/project.properties

[*3] $(ANDROID_SDK)/extras/google/google_play_services/docs

