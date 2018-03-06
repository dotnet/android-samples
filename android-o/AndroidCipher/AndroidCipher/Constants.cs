namespace AndroidCipher
{
    public class Constants
    {
        public static readonly int AesKeySize = 128;
        public static readonly int GcmTagLength = 16;
        public static readonly int GcmNonceLength = 12;

        public static readonly string Charset = "UTF-8";
        public static readonly string Algorithm = "AES";
        public static readonly string Transformation = "AES/GCM/NoPadding";
        public static readonly string ValidationMessage = "Input must not be blank!";
    }
}