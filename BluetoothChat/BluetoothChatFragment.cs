
using System;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    public partial class BluetoothChatFragment : Fragment
    {
        const string TAG = "BluetoothChatFragment";

        const int REQUEST_CONNECT_DEVICE_SECURE = 1;
        const int REQUEST_CONNECT_DEVICE_INSECURE = 2;
        const int REQUEST_ENABLE_BT = 3;

        ListView conversationView;
        EditText outEditText;
        Button sendButton;

        String connectedDeviceName = "";
        ArrayAdapter<String> conversationArrayAdapter;
        StringBuilder outStringBuffer;
        BluetoothAdapter bluetoothAdapter = null;
        BluetoothChatService chatService = null;

        bool requestingPermissionsSecure, requestingPermissionsInsecure;

        DiscoverableModeReceiver receiver;
        ChatHandler handler;
        WriteListener writeListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;


            receiver = new DiscoverableModeReceiver();
            receiver.BluetoothDiscoveryModeChanged += (sender, e) =>
            {
                Activity.InvalidateOptionsMenu();
            };

            if (bluetoothAdapter == null)
            {
                Toast.MakeText(Activity, "Bluetooth is not available.", ToastLength.Long).Show();
                Activity.FinishAndRemoveTask();
            }

            writeListener = new WriteListener(this);
            handler = new ChatHandler(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!bluetoothAdapter.IsEnabled)
            {
                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
            }
            else if (chatService == null)
            {
                SetupChat();
            }

            // Register for when the scan mode changes
            var filter = new IntentFilter(BluetoothAdapter.ActionScanModeChanged);
            Activity.RegisterReceiver(receiver, filter);
        }

        public override void OnResume()
        {
            base.OnResume();
            if (chatService != null)
            {
                if (chatService.GetState() == BluetoothChatService.STATE_NONE)
                {
                    chatService.Start();
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_bluetooth_chat, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            conversationView = view.FindViewById<ListView>(Resource.Id.@in);
            outEditText = view.FindViewById<EditText>(Resource.Id.edit_text_out);
            sendButton = view.FindViewById<Button>(Resource.Id.button_send);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            var allGranted = grantResults.AllPermissionsGranted();
            if (requestCode == PermissionUtils.RC_LOCATION_PERMISSIONS)
            {
                if (requestingPermissionsSecure)
                {
                    PairWithBlueToothDevice(true);
                }
                if (requestingPermissionsInsecure)
                {
                    PairWithBlueToothDevice(false);
                }

                requestingPermissionsSecure = false;
                requestingPermissionsInsecure = false;
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case REQUEST_CONNECT_DEVICE_SECURE:
                    if (Result.Ok == resultCode)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case REQUEST_CONNECT_DEVICE_INSECURE:
                    if (Result.Ok == resultCode)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case REQUEST_ENABLE_BT:
                    if (Result.Ok == resultCode)
                    {
                        Toast.MakeText(Activity, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        Activity.FinishAndRemoveTask();
                    }
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.secure_connect_scan:
                    PairWithBlueToothDevice(true);
                    return true;
                case Resource.Id.insecure_connect_scan:
                    PairWithBlueToothDevice(false);
                    return true;
                case Resource.Id.discoverable:
                    EnsureDiscoverable();
                    return true;
            }
            return false;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Activity.UnregisterReceiver(receiver);
            if (chatService != null)
            {
                chatService.Stop();
            }
        }

        void PairWithBlueToothDevice(bool secure)
        {
            requestingPermissionsSecure = false;
            requestingPermissionsInsecure = false;

            // Bluetooth is automatically granted by Android. Location, OTOH,
            // is considered a "dangerous permission" and as such has to 
            // be explicitly granted by the user.
            if (!Activity.HasLocationPermissions())
            {
                requestingPermissionsSecure = secure;
                requestingPermissionsInsecure = !secure;
                this.RequestPermissionsForApp();
                return;
            }

            var intent = new Intent(Activity, typeof(DeviceListActivity));
            if (secure)
            {
                StartActivityForResult(intent, REQUEST_CONNECT_DEVICE_SECURE);
            }
            else
            {
                StartActivityForResult(intent, REQUEST_CONNECT_DEVICE_INSECURE);
            }
        }


        void SetupChat()
        {
            conversationArrayAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.message);
            conversationView.Adapter = conversationArrayAdapter;

            outEditText.SetOnEditorActionListener(writeListener);
            sendButton.Click += (sender, e) =>
            {
                var textView = View.FindViewById<TextView>(Resource.Id.edit_text_out);
                var msg = textView.Text;
                SendMessage(msg);
            };

            chatService = new BluetoothChatService(handler);
            outStringBuffer = new StringBuilder("");
        }

        void SendMessage(String message)
        {
            if (chatService.GetState() != BluetoothChatService.STATE_CONNECTED)
            {
                Toast.MakeText(Activity, Resource.String.not_connected, ToastLength.Long).Show();
                return;
            }

            if (message.Length > 0)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                chatService.Write(bytes);
                outStringBuffer.Clear();
                outEditText.Text = outStringBuffer.ToString();
            }
        }

        bool HasActionBar()
        {
            if (Activity == null)
            {
                return false;
            }
            if (Activity.ActionBar == null)
            {
                return false;
            }
            return true;
        }

        void SetStatus(int resId)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.SetSubtitle(resId);
            }
        }

        void SetStatus(string subTitle)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.Subtitle = subTitle;
            }
        }

        void ConnectDevice(Intent data, bool secure)
        {
            var address = data.Extras.GetString(DeviceListActivity.EXTRA_DEVICE_ADDRESS);
            var device = bluetoothAdapter.GetRemoteDevice(address);
            chatService.Connect(device, secure);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            var menuItem = menu.FindItem(Resource.Id.discoverable);
            if (menuItem != null)
            {
                menuItem.SetEnabled(bluetoothAdapter.ScanMode == ScanMode.ConnectableDiscoverable);
            }

        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.bluetooth_chat, menu);
        }

        /// <summary>
        /// Listen for return key being pressed.
        /// </summary>
        class WriteListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            BluetoothChatFragment host;
            public WriteListener(BluetoothChatFragment frag)
            {
                host = frag;
            }
            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                if (actionId == ImeAction.ImeNull && e.Action == KeyEventActions.Up)
                {
                    host.SendMessage(v.Text);
                }
                return true;
            }
        }

        void EnsureDiscoverable()
        {
            if (bluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable)
            {
                var discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
                StartActivity(discoverableIntent);
            }
        }
    }
}
