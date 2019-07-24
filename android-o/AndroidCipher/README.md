---
name: Xamarin.Android - AndroidCipher
description: "AndroidCipher is a simple project which encrypts and decrypts string with usage of android cipher (Android Oreo)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - androidoreo
urlFragment: android-o-androidcipher
---
# AndroidCipher

AndroidCipher is a simple project which encrypts and decrypts string with usage of Android cipher.
This sample demonstrates some of the new encryption algorithms available in Android 8.1
Namely, Cipher.getParameters().getParameterSpec(IvParameterSpec.class) no longer works for algorithms that use GCM. Instead, use getParameterSpec(GCMParameterSpec.class).

![Android app with encrypt and decrypt inputs](Screenshots/android.png)
