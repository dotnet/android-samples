
This solution contains binding for Google Play Services SDK (which can be
installed via Android SDK Manager).

Note that this solution DOES NOT BUILD until unless you make some changes
in the project.[*1] To make it usable:

- build Google Play Services SDK binaries, using eclipse or ant.
  The library is located at:
  extras/google/google_play_services/libproject/google-play-services_lib.

	To build it with ant, run:

		- android update project -p .
		- ant debug

- In GooglePlayServices project, add a missing project file 
  "project.properties" from Google's Android SDK as
  "LibraryProjectProperties" [*2]. (You will have to use "add as a link"
  or MfA won't find the actual library files.)

- Open GooglePlayServices.sln. Then open Properties/AndroidManifest.xml in
  GooglePlayServices project, and insert your API Key there.

  For the Google API key details, see https://developers.google.com/maps/documentation/android/start#the_google_maps_api_key
  
  Your android key is located at... see http://docs.xamarin.com/Android/Guides/Platform_Features/Maps_and_Location/Obtaining_a_Google_Maps_API_Key
  (note that the doc is about Maps API v1, so only the description
  on the key location applies here.)

[*1] We cannot distribute the prebuilt dll because the library
is not freely distributable and there is no way to support required
resources bundled with it (if it were only *.jar then we could use InputJar).

(The Google samples are Apache licensed, so we ported some of them
to C# and included in this project.)

[*2] extras/google/google-play-services/libproject/google-plat-services_lib/project.properties
