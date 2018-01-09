using Android.App;
using Android.OS;
using Android.Webkit;
using System;

namespace SafeBrowsing
{
    [Activity(Label = "SafeBrowsing", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public WebView webView;
        public bool mSafeBrowsingIsInitialized;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            webView = FindViewById<WebView>(Resource.Id.webview);
            webView.SetWebViewClient(new WebViewClientImpl { Activity = this });

            mSafeBrowsingIsInitialized = false;
            var valueCallback = new ValueCallback { MainActivity = this };
            WebView.StartSafeBrowsing(this, valueCallback);
            webView.LoadUrl("https://university.xamarin.com/");
        }
    }

    public class ValueCallback : Java.Lang.Object, IValueCallback
    {
        public MainActivity MainActivity;
        public void OnReceiveValue(Java.Lang.Object value)
        {
            MainActivity.mSafeBrowsingIsInitialized = true;
            MainActivity.ActionBar.SetTitle(Convert.ToBoolean(value) ? Resource.String.safe_status : Resource.String.unsafe_status);
        }
    }
}

