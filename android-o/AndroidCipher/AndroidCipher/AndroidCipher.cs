using System;
using Android.Widget;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using System.Text;
using Android.Util;
using Java.Security.Spec;

namespace AndroidCipher
{
    public class AndroidCipher
    {
        private readonly MainActivity _activity;

        private Cipher _encipher;
        private SecureRandom _random;
        private ISecretKey _secretKey;

        public AndroidCipher(MainActivity activity)
        {
            this._activity = activity;
        }

        public void Decryption(object sender, EventArgs eventArgs)
        {
            var decipher = Cipher.GetInstance(Constants.Transformation);
            var algorithmParameterSpec = (IAlgorithmParameterSpec)_encipher.Parameters.GetParameterSpec(Java.Lang.Class.FromType(typeof(GCMParameterSpec)));
            decipher.Init(CipherMode.DecryptMode, _secretKey, algorithmParameterSpec);

            byte[] decodedValue = Base64.Decode(Encoding.UTF8.GetBytes(_activity.textOutput.Text), Base64Flags.Default);
            byte[] decryptedVal = decipher.DoFinal(decodedValue);
            _activity.textOriginal.Text = Encoding.Default.GetString(decryptedVal);
        }

        public static T Cast<T>(Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        public void Encryption(object sender, EventArgs eventArgs)
        {
            _secretKey = GenerateKey();
            if (ValidateInput(_activity.textInput.Text)) return;

            _encipher = Cipher.GetInstance(Constants.Transformation);
            _encipher.Init(CipherMode.EncryptMode, _secretKey, GenerateGcmParameterSpec());

            byte[]
            results = _encipher.DoFinal(Encoding.UTF8.GetBytes(_activity.textInput.Text));
            _activity.textOutput.Text = Base64.EncodeToString(results, Base64Flags.Default);
        }

        private bool ValidateInput(string input)
        {
            if (input.Trim().Equals(string.Empty))
            {
                Toast.MakeText(_activity.ApplicationContext, Constants.ValidationMessage, ToastLength.Short).Show();
                return true;
            }
            return false;
        }

        private GCMParameterSpec GenerateGcmParameterSpec()
        {
            var source = new byte[Constants.GcmNonceLength];
            _random.NextBytes(source);
            return new GCMParameterSpec(Constants.GcmTagLength * 8, source);
        }

        private ISecretKey GenerateKey()
        {
            _random = SecureRandom.InstanceStrong;
            var keyGen = KeyGenerator.GetInstance(Constants.Algorithm);
            keyGen.Init(Constants.AesKeySize, _random);
            return keyGen.GenerateKey();
        }
    }
}