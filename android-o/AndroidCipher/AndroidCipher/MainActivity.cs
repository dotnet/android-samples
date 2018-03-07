using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AndroidCipher
{
    [Activity(Label = "AndroidCipher", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : AppCompatActivity
    {
        AndroidCipher androidCipher;

        public EditText textInput;
        public TextView textOutput;
        public TextView textOriginal;
        Button buttonEncrypt, buttonDecrypt;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            androidCipher = new AndroidCipher(this);

            textInput = FindViewById<EditText>(Resource.Id.textInput);
            textOutput = FindViewById<TextView>(Resource.Id.textOutput);
            buttonEncrypt = FindViewById<Button>(Resource.Id.buttonEncrypt);
            buttonDecrypt = FindViewById<Button>(Resource.Id.buttonDecrypt);
            textOriginal = FindViewById<TextView>(Resource.Id.textOriginal);

            buttonDecrypt.Click += androidCipher.Decryption;
            buttonEncrypt.Click += androidCipher.Encryption;
        }
    }
}

