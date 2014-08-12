# Google Drive Android API Demos

Google Drive Android API Demos app illustrates all possible ways to talk to
Drive service with the use of interfaces available in [Google Play
Services](http://developer.android.com/google/play-services). The calls
illustrated within the app are:

### Listing and querying
* [List files with pagination](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/ListFilesActivity.java)
* [Query files](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/QueryFilesActivity.java)

### Working with files and folders
* [Create a file](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFileActivity.java)
* [Create a file in App Folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFileInAppFolderActivity.java)
* [Create a folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFolderActivity.java)
* [Retrieve metadata](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/RetrieveMetadataActivity.java)
* [Retrieve contents](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/RetrieveContentsActivity.java)
* [Listen download progress](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/RetrieveContentsWithProgressDialogActivity.java)
* [Edit metadata](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/EditMetadataActivity.java)
* [Edit contents](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/EditContentsActivity.java)
* [Pin file to the device](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/PinFileActivity.java)

### Intents
* [Create a file with creator activity](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFileWithCreatorActivity.java)
* [Pick a file with opener activity](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/PickFileWithOpenerActivity.java)
* [Pick a folder with opener activity](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/PickFolderWithOpenerActivity.java)

### Hierarchical operations
* [Create a file in a folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFileInFolderActivity.java)
* [Create a folder in a folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/CreateFolderInFolderActivity.java)
* [List files in a folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/ListFilesInFolderActivity.java)
* [Query files in a folder](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/QueryFilesInFolderActivity.java)

### Others
* [Authorization, authentication and client connection](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/BaseDemoActivity.java)
* [Synchronous requests](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/SyncRequestsActivity.java)
* [Listen for metadata and contents change events](https://github.com/googledrive/android-demos/blob/master/src/com/google/android/gms/drive/sample/demo/events/ListenChangeEventsForFilesActivity.java)

## Can I run this app?

If you actually want to run this sample app (though it is mostly provided so you
can read the code), you will need to register an OAuth 2.0 client for the
package `com.google.android.gms.drive.sample.demo` with your own debug keys
and set any resource IDs to those that you have access to. Resource ID definitions
are on:

* com.google.android.gms.drive.sample.demo.BaseDemoActivity.EXISTING_FOLDER_ID
* com.google.android.gms.drive.sample.demo.BaseDemoActivity.EXISTING_FILE_ID

![Analytics](https://ga-beacon.appspot.com/UA-46884138-1/android-demos?pixel)

