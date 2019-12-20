using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Webkit;
using AndroidX.AppCompat.App;

namespace AutofillFramework
{
    [Activity(Label = "WebViewSignInActivity")]
    [Register("com.xamarin.AutofillFramework.WebViewSignInActivity")]
    public class WebViewSignInActivity : AppCompatActivity
    {
        public static Intent GetStartActivityIntent(Context context)
        {
            var intent = new Intent(context, typeof(WebViewSignInActivity));
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_webview_activity);

            var webView = FindViewById<WebView>(Resource.Id.webview);
            var webSettings = webView.Settings;
            webView.SetWebViewClient(new WebViewClient());
            webSettings.JavaScriptEnabled = true;

            var url = Intent.GetStringExtra("url");
            if (url == null)
            {
                url = "file:///android_asset/Resources/raw/sample_form.html";
            }
            if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "Clearing WebView data");
            webView.ClearHistory();
            webView.ClearFormData();
            webView.ClearCache(true);
            Log.Info(CommonUtil.TAG, "Loading URL " + url);
            webView.LoadUrl(url);
        }
    }
}