using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace ContentControls {

    [Activity(Label = "WebViewLocalContent")]
    public class WebViewLocalContentScreen : Activity {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.WebView);

            var wv = FindViewById<WebView>(Resource.Id.Web);
            wv.Settings.JavaScriptEnabled = true;
            
            wv.LoadUrl("file:///android_asset/Content/Home.html");

            wv.SetWebViewClient(new MonkeyWebViewClient());

            // allow zooming/panning
            wv.Settings.BuiltInZoomControls = true;
            wv.Settings.SetSupportZoom(true);

            // we DON'T want the page zoomed-out, since it is phone-sized content
            wv.Settings.LoadWithOverviewMode = false;
            wv.Settings.UseWideViewPort = false;

            // scrollbar stuff
            wv.ScrollBarStyle = ScrollbarStyles.OutsideOverlay; // so there's no 'white line'
            wv.ScrollbarFadingEnabled = false;
        }
        class MonkeyWebViewClient : WebViewClient {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return true;
            }
        }
    }
}

