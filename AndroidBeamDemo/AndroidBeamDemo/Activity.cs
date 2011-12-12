using System;
using System.Text;
using Android.App;
using Android.Nfc;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.OS;
using Android.Text.Format;
using Android.Views;
using Android.Widget;

namespace Com.Android.Example.Beam
{
	//[Activity (Label = "MonoDroid BeamDemo", MainLauncher = true)]
	public class Beam : Activity, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
	{
		public Beam ()
		{
			mHandler = new MyHandler (HandlerHandleMessage);
		}
		
		NfcAdapter mNfcAdapter;
		TextView mInfoText;
		private const int MESSAGE_SENT = 1;
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mInfoText = (TextView) FindViewById (Resource.Id.textView);
			// Check for available NFC Adapter
			mNfcAdapter = NfcAdapter.GetDefaultAdapter (this);
			if (mNfcAdapter == null) {
				mInfoText = (TextView) FindViewById (Resource.Id.textView);
				mInfoText.Text = "NFC is not available on this device.";
			}
			// Register callback to set NDEF message
			mNfcAdapter.SetNdefPushMessageCallback (this, this);
			// Register callback to listen for message-sent success
			mNfcAdapter.SetOnNdefPushCompleteCallback (this, this);
		}
		
		public NdefMessage CreateNdefMessage (NfcEvent evt) 
		{
			DateTime time = DateTime.Now;
			var text = ("Beam me up!\n\n" + 
			               "Beam Time: " + time.ToString ("HH:mm:ss"));
			NdefMessage msg = new NdefMessage (
			new NdefRecord[] { CreateMimeRecord (
				"application/com.example.android.beam", Encoding.UTF8.GetBytes (text))
			/**
			* The Android Application Record (AAR) is commented out. When a device
			* receives a push with an AAR in it, the application specified in the AAR
			* is guaranteed to run. The AAR overrides the tag dispatch system.
			* You can add it back in to guarantee that this
			* activity starts when receiving a beamed message. For now, this code
			* uses the tag dispatch system.
			*/
			//,NdefRecord.CreateApplicationRecord("com.example.android.beam")
			});
			return msg;
		}
		
		public void OnNdefPushComplete (NfcEvent arg0)
		{
			// A handler is needed to send messages to the activity when this
			// callback occurs, because it happens from a binder thread
			mHandler.ObtainMessage (MESSAGE_SENT).SendToTarget ();
		}
		
		class MyHandler : Handler
		{
			public MyHandler (Action<Message> handler)
			{
				this.handle_message = handler;
			}
			
			Action<Message> handle_message;
			public override void HandleMessage (Message msg)
			{
				handle_message (msg);
			}
		}
		
		private readonly Handler mHandler;
		
		protected void HandlerHandleMessage (Message msg) 
		{
			switch (msg.What) {
			case MESSAGE_SENT:
				Toast.MakeText (this.ApplicationContext, "Message sent!", ToastLength.Long).Show ();
				break;
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			// Check to see that the Activity started due to an Android Beam
			if (NfcAdapter.ActionNdefDiscovered == Intent.Action) {
				ProcessIntent (Intent);
			}
		}
		
		protected override void OnNewIntent (Intent intent) 
		{
			// onResume gets called after this to handle the intent
			Intent = intent;
		}

		void ProcessIntent (Intent intent)
		{
			IParcelable [] rawMsgs = intent.GetParcelableArrayExtra (
				NfcAdapter.ExtraNdefMessages);
			// only one message sent during the beam
			NdefMessage msg = (NdefMessage) rawMsgs [0];
			// record 0 contains the MIME type, record 1 is the AAR, if present
			mInfoText.Text = Encoding.UTF8.GetString (msg.GetRecords () [0].GetPayload ());
		}

		public NdefRecord CreateMimeRecord (String mimeType, byte [] payload) 
		{
			byte [] mimeBytes = Encoding.UTF8.GetBytes (mimeType);
			NdefRecord mimeRecord = new NdefRecord (
				NdefRecord.TnfMimeMedia, mimeBytes, new byte [0], payload);
			return mimeRecord;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// If NFC is not available, we won't be needing this menu
			if (mNfcAdapter == null) {
				return base.OnCreateOptionsMenu (menu);
			}
			MenuInflater inflater = MenuInflater;
			inflater.Inflate (Resource.Menu.Options, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item) 
		{
			switch (item.ItemId) {
			case Resource.Id.menu_settings:
				Intent intent = new Intent (Settings.ActionNfcsharingSettings);
				StartActivity (intent);
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}
	}
}


