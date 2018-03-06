using System.Text;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    public partial class BluetoothChatFragment
    {
        /// <summary>
        /// Handles messages that come back from the ChatService.
        /// </summary>
        class ChatHandler : Handler
        {
            BluetoothChatFragment chatFrag;
            public ChatHandler(BluetoothChatFragment frag)
            {
                chatFrag = frag;

            }
            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case Constants.MESSAGE_STATE_CHANGE:
                        switch (msg.What)
                        {
                            case BluetoothChatService.STATE_CONNECTED:
                                chatFrag.SetStatus(chatFrag.GetString(Resource.String.title_connected_to, chatFrag.connectedDeviceName));
                                chatFrag.conversationArrayAdapter.Clear();
                                break;
                            case BluetoothChatService.STATE_CONNECTING:
                                chatFrag.SetStatus(Resource.String.title_connecting);
                                break;
                            case BluetoothChatService.STATE_LISTEN:
                                chatFrag.SetStatus(Resource.String.not_connected);
                                break;
                            case BluetoothChatService.STATE_NONE:
                                chatFrag.SetStatus(Resource.String.not_connected);
                                break;
                        }
                        break;
                    case Constants.MESSAGE_WRITE:
                        var writeBuffer = (byte[])msg.Obj;
                        var writeMessage = Encoding.ASCII.GetString(writeBuffer);
                        chatFrag.conversationArrayAdapter.Add($"Me:  {writeMessage}");
                        break;
                    case Constants.MESSAGE_READ:
                        var readBuffer = (byte[])msg.Obj;
                        var readMessage = Encoding.ASCII.GetString(readBuffer);
                        chatFrag.conversationArrayAdapter.Add($"{chatFrag.connectedDeviceName}: {readMessage}");
                        break;
                    case Constants.MESSAGE_DEVICE_NAME:
                        chatFrag.connectedDeviceName = msg.Data.GetString(Constants.DEVICE_NAME);
                        if (chatFrag.Activity != null)
                        {
                            Toast.MakeText(chatFrag.Activity, $"Connected to {chatFrag.connectedDeviceName}.", ToastLength.Short).Show();
                        }
                        break;
                    case Constants.MESSAGE_TOAST:
                        break;
                }
            }
        }
    }
}
