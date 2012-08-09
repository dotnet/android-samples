using System;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Util;
using Java.Interop;

namespace StockService
{
    [Service]
    [IntentFilter(new String[]{"com.xamarin.DemoMessengerService"})]
    public class DemoMessengerService : Service
    {
        Messenger demoMessenger;

        public DemoMessengerService ()
        {
            demoMessenger = new Messenger (new DemoHandler ());
        }

        public override IBinder OnBind (Intent intent)
        {
            Log.Debug ("StockMessengerService", "client bound to service");

            return demoMessenger.Binder;
        }

        class DemoHandler : Handler
        {
            public override void HandleMessage (Message msg)
            {
                Log.Debug ("DemoMessengerService", msg.What.ToString ());

                string text = msg.Data.GetString ("InputText");

                Log.Debug ("DemoMessengerService", "InputText = " + text);
            }
        }
    }
}


