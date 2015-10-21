using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace DemoMessengerClient
{
    [Activity (Label = "DemoMessengerClient", MainLauncher = true)]
    public class DemoMessengerActivity : Activity
    {
        bool isBound = false;
        Messenger demoMessenger;
        DemoServiceConnection demoServiceConnection;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Main);
     
            var button = FindViewById<Button> (Resource.Id.callMessenger);
            
            button.Click += delegate {
                if (isBound) {

                    Message message = Message.Obtain ();
                    Bundle b = new Bundle ();
                    b.PutString ("InputText", "text from client");
                    message.Data = b;

                    demoMessenger.Send (message);
                }
            };
        }

        protected override void OnStart ()
        {
            base.OnStart ();

            var demoServiceIntent = new Intent ("com.xamarin.DemoMessengerService");
            demoServiceConnection = new DemoServiceConnection (this);
            BindService (demoServiceIntent, demoServiceConnection, Bind.AutoCreate);
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();

            if (isBound) {
                UnbindService (demoServiceConnection);
                isBound = false;
            }
        }

        class DemoServiceConnection : Java.Lang.Object, IServiceConnection
        {
            DemoMessengerActivity activity;

            public DemoServiceConnection (DemoMessengerActivity activity)
            {
                this.activity = activity;
            }
          
            public void OnServiceConnected (ComponentName name, IBinder service)
            {
                activity.demoMessenger = new Messenger (service);
                activity.isBound = true;
            }

            public void OnServiceDisconnected (ComponentName name)
            {
                activity.demoMessenger.Dispose ();
                activity.demoMessenger = null;
                activity.isBound = false;
            }
        }
    }
}


