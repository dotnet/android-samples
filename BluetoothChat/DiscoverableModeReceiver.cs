using System;
using Android.Bluetooth;
using Android.Content;

namespace com.xamarin.samples.bluetooth
{

    public class BluetoothDiscoveryModeArgs: EventArgs 
    {
        public BluetoothDiscoveryModeArgs(bool inDiscoveryMode)
        {
            InDiscoveryMode = inDiscoveryMode;
        }
        public bool InDiscoveryMode { get; private set; }
    }


    /// <summary>
    /// Listen for when the device goes in and out of Bluetooth discoverability
    /// mode, and will raise an Event.
    /// </summary>
    public class DiscoverableModeReceiver : BroadcastReceiver
    {
        public event EventHandler<BluetoothDiscoveryModeArgs> BluetoothDiscoveryModeChanged;


        public override void OnReceive(Context context, Intent intent)
        {
            var currentScanMode = intent.GetIntExtra(BluetoothAdapter.ExtraScanMode, -1);
            var previousScanMode = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousScanMode, -1);


            bool inDiscovery = currentScanMode == (int)ScanMode.ConnectableDiscoverable;

            BluetoothDiscoveryModeChanged?.Invoke(this, new BluetoothDiscoveryModeArgs(inDiscovery));

        }
    }
}
