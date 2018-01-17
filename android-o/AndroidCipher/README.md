AndroidCipher
=====

AndroidCipher is a simple project which encrypts and decrypts string with usage of  android cipher.
This sample demonstrates some of the new encryption algorithms available in Android 8.1
Namely, Cipher.getParameters().getParameterSpec(IvParameterSpec.class) no longer works for algorithms that use GCM. Instead, use getParameterSpec(GCMParameterSpec.class).

Authors
-------

Malkin Dmytro