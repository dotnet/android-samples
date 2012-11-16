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
        /// <summary>
        /// A mime type for the the string that this app will write to the NFC tag. Will be
        /// used to help this application identify NFC tags that is has written to.
        /// </summary>
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

            // Get a reference to the default NFC adapter for this device. This adapter 
            // is how an Android application will interact with the actual hardware.
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            _writeTagButton = FindViewById<Button>(Resource.Id.write_tag_button);
            _writeTagButton.Click += WriteTagButtonOnClick;

            _textView = FindViewById<TextView>(Resource.Id.text_view);
        }

        /// <summary>
        /// This method is called when an NFC tag is discovered by the application.
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
        {
            if (_inWriteMode)
            {
                _inWriteMode = false;
                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

                if (tag == null)
                {
                    return;
                }

                // These next few lines will create a payload (consisting of a string)
                // and a mimetype. NFC record are arrays of bytes. 
                var payload = Encoding.ASCII.GetBytes(GetRandomHominid());
                var mimeBytes = Encoding.ASCII.GetBytes(ViewApeMimeType);
                var apeRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);
                var ndefMessage = new NdefMessage(new[] { apeRecord });

                if (!TryAndWriteToTag(tag, ndefMessage))
                {
                    // Maybe the write couldn't happen because the tag wasn't formatted?
                    TryAndFormatTagWithMessage(tag, ndefMessage);                    
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            // App is paused, so no need to keep an eye out for NFC tags.
            _nfcAdapter.DisableForegroundDispatch(this);
        }

        private void DisplayMessage(string message)
        {
            _textView.Text = message;
            Log.Info(Tag, message);
        }

        /// <summary>
        /// Identify to Android that this activity wants to be notified when 
        /// an NFC tag is discovered. 
        /// </summary>
        private void EnableWriteMode()
        {
            _inWriteMode = true;

            // Create an intent filter for when an NFC tag is discovered.  When
            // the NFC tag is discovered, Android will u
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };
            
            // When an NFC tag is detected, Android will use the PendingIntent to come back to this activity.
            // The OnNewIntent method will invoked by Android.
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
            _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ndefMessage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pick one of the four hominids to display
        /// </summary>
        /// <returns>A string that corresponds to one of the images in this application.</returns>
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

        /// <summary>
        /// This method will try and write the specified message to the provided tag. 
        /// </summary>
        /// <param name="tag">The NFC tag that was detected.</param>
        /// <param name="ndefMessage">An NDEF message to write.</param>
        /// <returns>true if the tag was written to.</returns>
        private bool TryAndWriteToTag(Tag tag, NdefMessage ndefMessage)
        {

            // This object is used to get information about the NFC tag as 
            // well as perform operations on it.
            var ndef = Ndef.Get(tag); 
            if (ndef != null)
            {
                ndef.Connect();

                // Once written to, a tag can be marked as read-only - check for this.
                if (!ndef.IsWritable)
                {
                    DisplayMessage("Tag is read-only.");
                }

                // NFC tags can only store a small amount of data, this depends on the type of tag its.
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
