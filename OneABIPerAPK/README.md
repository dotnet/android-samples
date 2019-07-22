---
name: Xamarin.Android - OneABIPerAPK
description: This solution will build a simple HelloWorld style of project. There will be three APK's built, one for each ABI. This solution demonstrates how to...
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: oneabiperapk
---
# OneABIPerAPK

This solution will build a simple HelloWorld style of project. There will be three APK's built, one for each ABI. This solution demonstrates how to create a unique `android:versionCode` for each APK.

To build the solution run the following at the command line:

	rake build
		
This will create three folders with the APK:

	bin.armeabi
	bin.armeabi-v7a
	bin.x86
	
	

