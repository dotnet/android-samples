To build this example:
  1. Run Eclipse.
  2. Open the the Import dialog ("File->Import..." menu).
  3. Select "Android/Existing Android Code Into Workspace" and click "Next>".
  4. Select the directory containing this file (by either typing it into the
     "Root Directory" field, or using "Browse..." to select it).
  5. Click "Refresh", and you should see "CuteAnimals" listed as a
     "New Project Name".
  6. Ensure the checkbox on the "CuteAnimals" line is checked.
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
There are two corresponding files, "defaultcontainer_binary" and
"defaultcontainer_json", which are used as default containers.

defaultcontainer_json contains default key-value pairs. It is limited to represent the
key-value pairs representable in value collection macros.

defaultcontainer_binary is a full-featured default container. It contains the same default
key-value pairs as defaultcontainer_json plus the configuration of how to trigger
Universal Analytics tags and a custom function call tag. Here is a summary of
the container (more details can be seen in the snapshot at images/Container.png)
   8 Macros:
      * "app name": the pre-populated application name macro.
      * "app version": the pre-populated app version macro.
      * "Cute Animals Android": a value collection macro containing key/value
        pairs as shown in defaultcontainer_json.
      * "event": an event macro.
      * "numRefreshes": a custom function call macro which records how many
        times the "Refresh" button is clicked.
      * "numRefreshesMod5": a custom function call macro whose value is equal
        to "numRefreshes" mod 5. See images/NumRefreshesMod5.png for more
        details.
      * "screen name": a data layer macro whose data layer variable name
        is "screenName".
      * "true": the pre-populated constant string macro whose value is equal to
        "true".
   5 Rules:
      * "CustomTagFires": the value of the event macro is equal to 'custom_tag'.
      * "CloseScreenEvent": the value of the event macro is equal to
        'closeScreen'.
      * "OpenScreenEvent": the value of the event macro is equal to
        'openScreen'.
      * "RefreshEvent": the value of the event macro is equal to 'refresh' and
        the value of the numRefreshesMod5 macro is equal to 1. See
        images/RefreshEvent.png for more details.
      * "Always": the pre-populated rule which is always evaluated to true.
   4 Tags:
      * "CustomTag": a custom function call tag with the firing rule:
        CustomTagFires is true. See images/CustomTag.png for more details.
      * "RefreshEvent": a Universal Analytics tag with the firing rule:
        RefreshEvent is true. See images/RefreshEventTag.png for more details.
      * "ScreenClosed": a Universal Analytics tag with the firing rule:
        CloseScreenEvent is true. See images/ScreenClosedTag.png for more
        details.
      * "ScreenOpen": a Universal Analytics tag with the firing rule:
        OpenScreenEvent is true. See images/ScreenOpenTag.png for more
        details.

Although the app will run and use those values specified in the
defaultcontainer_binary (or defaultcontainer_json if you delete
defaultcontainer_binary from your local machine), there's no way to dynamically
update those values, since this is a non-existent container.

To use real values, create a container in the Tag Manager UI, and note the
resulting container ID:
  1. Download the container from the Tag Manager UI and rename it to
     gtm_1234_binary (where GTM-1234 is the container ID for the new container).
     Put gtm_1234_binary into the same directory with defaultcontainer_json file.
  2. Change the value of the CONTAINER_ID constant in SplashScreenActivity.java
     to the new container ID and modify the resource ID to use the correct value 
     (e.g. R.raw.gtm_1234_binary) instead of R.raw.defaultcontainer_binary.
  3. Optional: delete defaultcontainer_binary and defaultcontainer_json files.
