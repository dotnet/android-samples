To build this example:
  1. Run Eclipse.
  2. Open the the Import dialog ("File->Import..." menu).
  3. Select "Android/Existing Android Code Into Workspace" and click "Next>".
  4. Select the directory containing this file (by either typing it into the
     "Root Directory" field, or using "Browse..." to select it).
  5. Click "Refresh", and you should see "HelloWorld" listed as a
     "New Project Name".
  6. Click the checkbox on the "HelloWorld" line.
  7. Optionally, click the "Copy projects into workspace" checkbox.
  8. Click "Finish".
  9. At this point, you have a project.  You still need to add the GMS Play
     services library.
 10. Copy and import the library project as described in the Android developer
     documentation:
     (https://developer.android.com/google/play-services/setup.html#Install)
        * Copy the library project at
          <android-sdk>/extras/google/google_play_services/libproject/
              google-play-services_lib/
          to the location where you maintain your Android app projects.
        * Import the TagManager library project into your workspace.
             - Click on "File->Import"
             - Select "Android-> Existing Android Code into Workspace",
             - Browse to the copy of the library project to import it.
 11. Add a reference to the TagManager library project as described in Android
     developer documentation:
     (https://developer.android.com/tools/projects/projects-eclipse.html#ReferencingLibraryProject)
        * Right-click on the "CuteAnimals" project and select "Properties...".
        * Select the "Android" submenu on the property sheet.
        * Click "Add..." and select "google-play-services_lib".
        * Click "OK".

This sample uses a container ID for a non-existent container: "GTM-XXXX".
There's a corresponding "gtm_xxxx_json" in res/raw which is used
as the default container.  Although the app will run and use those values,
there's no way to dynamically update those values, since this is a
non-existent container.

To use real values, create a container in the Tag Manager UI, and note the
resulting container ID:
  1. Change the name of gtm_xxxx_json to gtm_1234_json (where
     GTM-1234 is the container ID for the new container).
  2. Change the value of the CONTAINER_ID constant in MainActivity.java to the
     new container ID and modify the resource ID to use the correct value
     (e.g. R.raw.gtm_1234_json) instead of R.raw.gtm_xxxx_json.
