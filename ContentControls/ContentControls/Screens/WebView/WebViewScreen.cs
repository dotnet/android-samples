using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace ContentControls {

    [Activity(Label = "WebView")]
    public class WebViewScreen : Activity {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.WebView);

            var wv = FindViewById<WebView>(Resource.Id.Web);
            wv.Settings.JavaScriptEnabled = true;
            wv.LoadUrl("http://xamarin.com/");
            wv.SetWebViewClient(new MonkeyWebViewClient());

            // allow zooming/panning
            wv.Settings.BuiltInZoomControls = true;
            wv.Settings.SetSupportZoom(true);

            // loading with the page zoomed-out, so you can see the whole thing (like the default behaviour of the real browser)
            wv.Settings.LoadWithOverviewMode = true;
            wv.Settings.UseWideViewPort = true;

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

