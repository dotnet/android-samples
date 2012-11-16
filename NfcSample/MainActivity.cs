namespace NfcXample
{
    using System;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.Nfc;
    using Android.Nfc.Tech;
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using Android.Widget;

    using Java.IO;

    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public const string ViewApeMimeType = "application/vnd.xamarin.nfcxample";
        public static readonly string NfcAppRecord = "xamarin.nfxample";
        public static readonly string Tag = "NfcXample";

        private bool _inWriteMode;
        private NfcAdapter _nfcAdapter;
        private TextView _textView;
        private Button _writeTagButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            _writeTagButton = FindViewById<Button>(Resource.Id.write_tag_button);
            _writeTagButton.Click += WriteTagButtonOnClick;

            _textView = FindViewById<TextView>(Resource.Id.text_view);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (_inWriteMode)
            {
                _inWriteMode = false;
                var tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
                var appRecord = NdefRecord.CreateApplicationRecord(NfcAppRecord);
                var payload = Encoding.ASCII.GetBytes(GetRandomHominid());
                var mimeBytes = Encoding.ASCII.GetBytes(ViewApeMimeType);
                var apeRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);

                var ndefMessage = new NdefMessage(new[] { apeRecord });

                if (!TryAndWriteToTag(tag, ndefMessage))
                {
                    TryAndFormatTagWithMessage(tag, ndefMessage);                    
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _nfcAdapter.DisableForegroundDispatch(this);
        }

        private void DisplayMessage(string message)
        {
            _textView.Text = message;
            Log.Info(Tag, message);
        }

        private void EnableWriteMode()
        {
            _inWriteMode = true;
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };
            _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        private bool TryAndFormatTagWithMessage(Tag tag, NdefMessage ndefMessage)
        {
            var format = NdefFormatable.Get(tag);
            if (format == null)
            {
                DisplayMessage("Tag does not appear to support NDEF format.");
            }
            else
            {
                try
                {
                    format.Connect();
                    format.Format(ndefMessage);
                    DisplayMessage("Tag successfully written.");
                    return true;
                }
                catch (IOException ioex)
                {
                    var msg = "There was an error trying to format the tag.";
                    DisplayMessage(msg);
                    Log.Error(Tag, ioex, msg);
                }
            }
            return false;
        }

        private string GetRandomHominid()
        {
            var random = new Random();
            var r = random.NextDouble();
            Log.Debug(Tag, "Random number: {0}", r.ToString("N2"));
            if (r < 0.25)
            {
                return "heston";
            }
            if (r < 0.5)
            {
                return "gorillas";
            }
            if (r < 0.75)
            {
                return "dr_zaius";
            }
            return "cornelius";
        }

        private void WriteTagButtonOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View)sender;
            if (view.Id == Resource.Id.write_tag_button)
            {
                DisplayMessage("Touch and hold the tag against the phone to write.");
                EnableWriteMode();
            }
        }

        private bool TryAndWriteToTag(Tag tag, NdefMessage ndefMessage)
        {
            var ndef = Ndef.Get(tag);
            if (ndef != null)
            {
                ndef.Connect();

                if (!ndef.IsWritable)
                {
                    DisplayMessage("Tag is read-only.");
                }

                var size = ndefMessage.ToByteArray().Length;
                if (ndef.MaxSize < size)
                {
                    DisplayMessage("Tag doesn't have enough space.");
                }

                ndef.WriteNdefMessage(ndefMessage);
                DisplayMessage("Succesfully wrote tag.");
                return true;
            }

            return false;
        }
    }
}
