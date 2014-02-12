using System;
using Android.App;
using Android.OS;
using Android.Nfc.CardEmulators;

namespace KitKat
{
	// Setup for an NFC HCE payments application 
	[Service(Exported=true, Permission="android.permissions.BIND_NFC_SERVICE"), 
		IntentFilter(new[] {"android.nfc.cardemulation.HOST_APDU_SERVICE"}), 
		MetaData("andorid.nfc.cardemulation.host.apdu_service", 
			Resource="@xml/hceservice")]

	// The hceservice.xml resource contains important information for pairing the HCE Service
	// with the correct NFC reader, as well as other required metadata. It should be saved to
	// the Resources/xml directory. An example is provided in this application.

	class HceService : HostApduService
	{

		#region implemented abstract members of HostApduService

		// This method receives an Application Protocol Data Unit (ADPU) from the NFC reader, 
		// and returns an ADPU. It's the data exchange between the reader and this device.

		public override byte[] ProcessCommandApdu(byte[] apdu, Bundle extras) 
		{
			throw new NotImplementedException ();
		}

		// This gets called when the NFC reader is offline, or communicating
		// with another application
		public override void OnDeactivated (DeactivationReason reason)
		{
			throw new NotImplementedException ();
		}

		#endregion

	}
}

