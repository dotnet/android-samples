OneABIPerAPK
============

This solution will build a simple HelloWorld style of project. There will be three APK's built, one for each ABI. This solution demonstrates how to create a unique `android:versionCode` for each APK.

To build the solution run the following at the command line:

	rake build
		
This will create three folders with the APK:

	bin.armeabi
	bin.armeabi-v7a
	bin.x86
	
	

> PS: These same example (but **C# console app**) how to build with a unique `android:versionCode`, `android:versionName`, `android:packageName` for each APK and ready to go to Google Play [Link to repo](https://github.com/JTOne123/OnePackagePerABI)
