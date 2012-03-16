using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace ContentControls {

    [Activity(Label = "WebViewGeneratedContent")]
    public class WebViewGeneratedContentScreen : Activity {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.WebView);

            var wv = FindViewById<WebView>(Resource.Id.Web);
            wv.Settings.JavaScriptEnabled = true;

            string data = @"<html><body>
<h1>Generated Html</h1>
<p>This Html was generated in c#, but can link to local files</p>
<p><image src='Images/Image_Seedlings.png' /></p>
<p><a href='SubPage1.html'>Sub Page 1</a></p>
</body></html>";

            wv.LoadData(data, "text/html", "UTF-8");
            wv.LoadDataWithBaseURL("file:///android_asset/Content/", data, "text/html", "UTF-8", "");

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

