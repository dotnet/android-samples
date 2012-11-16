NFCXample
=========

This sample application demonstrate using Near Field Communications (NFC) to read and write from an NFC tag. The application will write a string to an NFC tag. Then when the application detects the NFC tag that it had previously written, it will read that string and display an image associated with that string.

It is strongly recommended to have an actual device and an NFC tag to see this sample in action.

This application targets API 15 (Android 4.0.3) and was tested on a Galaxy Nexus on API level 16. It should be run on API level 10 (Android 2.3.3), which was the first version that NFC was available on on Android.

Not all devices have NFC enable by default. Make sure to enable it on your device or this sample will not work.