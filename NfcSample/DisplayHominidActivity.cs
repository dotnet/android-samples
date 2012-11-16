namespace NfcXample
{
    using System;
    using System.Text;

    using Android.App;
    using Android.Nfc;
    using Android.OS;
    using Android.Widget;

    /// <summary>
    /// This activity will be used to display the image 
    /// for the string that was written to the NFC tag by MainActivity.
    /// </summary>
    [Activity, IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" },
        DataMimeType = MainActivity.ViewApeMimeType,
        Categories = new[] { "android.intent.category.DEFAULT" })]
    public class DisplayHominidActivity : Activity
    {
        private ImageView _imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DisplayHominid);
            if (Intent == null)
            {
                return;
            }

            var intentType = Intent.Type ?? String.Empty;
            _imageView = FindViewById<ImageView>(Resource.Id.ape_view);

            var button = FindViewById<Button>(Resource.Id.back_to_main_activity);
            button.Click += (sender, args) => Finish();

            // MainActivity write the mimetype to the tag. We just do a quick check
            // to make sure that the tag that was discovered is indeed a tag that
            // this application wrote.
            if (MainActivity.ViewApeMimeType.Equals(intentType))
            {
                // Get the string that was written to the NFC tag, and display it.
                var rawMessages = Intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                var msg = (NdefMessage)rawMessages[0];
                var hominidRecord = msg.GetRecords()[0];
                var hominidName = Encoding.ASCII.GetString(hominidRecord.GetPayload());
                DisplayHominid(hominidName);
            }
        }

        /// <summary>
        /// Display the image that is associated with the string in question.
        /// </summary>
        /// <param name="name"></param>
        private void DisplayHominid(string name)
        {
            var hominidImageId = 0;

            if ("cornelius".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                hominidImageId = Resource.Drawable.cornelius;
            }
            if ("dr_zaius".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                hominidImageId = Resource.Drawable.dr_zaius;
            }
            if ("gorillas".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                hominidImageId = Resource.Drawable.gorillas;
            }
            if ("heston".Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                hominidImageId = Resource.Drawable.heston;
            }

            if (hominidImageId > 0)
            {
                _imageView.SetImageResource(hominidImageId);
            }
        }
    }
}
