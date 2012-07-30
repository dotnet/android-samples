using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ME.Kiip.Api;
using Android.Util;

namespace KiipSampleApplication
{
    [Application()]
    public class ExampleApplication : Application
    {
        private static readonly string TAG = "example";
        private static readonly string KP_APP_KEY = "xxx";
        private static readonly string KP_APP_SECRET = "yyy";

        public ME.Kiip.Api.Kiip.IRequestListener startSessionListener;
        public ME.Kiip.Api.Kiip.IRequestListener endSessionListener;

        public ExampleApplication(IntPtr handle, JniHandleOwnership transfer): base(handle, transfer)
        {

        }
    
        public override void OnCreate()
        {
            base.OnCreate();
            Kiip.Init(this, KP_APP_KEY, KP_APP_SECRET);
            startSessionListener = new StartSessionListener(this) as ME.Kiip.Api.Kiip.IRequestListener;
            endSessionListener = new EndSessionListener(this) as ME.Kiip.Api.Kiip.IRequestListener;

        }

        public void toast(string message)
        {
            Log.Verbose(TAG, message);
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        public class StartSessionListener : Java.Lang.Object, ME.Kiip.Api.Kiip.IRequestListener
        {
            ExampleApplication example;

            public StartSessionListener(ExampleApplication example)
            {
                this.example = example;
            }

            public void OnError(Kiip p0, KiipException p1)
            {
                example.toast("End Session Failed: " + "(" + p1.Code + ") " + p1.Message);
            }

            public void OnFinished(Kiip p0, Java.Lang.Object p1)
            {
                //ME.Kiip.Api.Resource response = p1.JavaCast<ME.Kiip.Api.Resource>();
                //if (response != null)
                //{
                //    example.toast("Start Session Finished w/ Promo");
                //}
                //else
                //{
                //    example.toast("Start Session Finished No Promo");
                //}
                //p0.ShowResource(response);
            }
        }

        public class EndSessionListener : Java.Lang.Object, ME.Kiip.Api.Kiip.IRequestListener
        {
            ExampleApplication example;

            public EndSessionListener(ExampleApplication example)
            {
                this.example = example;
            }

            public void OnError(Kiip p0, KiipException p1)
            {
                example.toast("End Session Failed: " + "(" + p1.Code + ") " + p1.Message);
            }

            public void OnFinished(Kiip p0, Java.Lang.Object p1)
            {
                example.toast("End Session Finished");
            }
        }
    }
}