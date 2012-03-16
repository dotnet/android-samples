using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace ContentControls {

    [Activity(Label = "WebViewBrowser")]
    public class WebViewBrowserScreen : Activity {
        WebView web;
        Button BackButton, ForwardButton, GoButton, StopButton;
        EditText UrlText;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.WebViewBrowser);

            web = FindViewById<WebView>(Resource.Id.Web);
            web.Settings.JavaScriptEnabled = true;
            web.LoadUrl("http://xamarin.com/");
            web.SetWebViewClient(new MonkeyWebViewClient());

            // allow zooming/panning
            web.Settings.BuiltInZoomControls = true;
            web.Settings.SetSupportZoom(true);


            // loading with the page zoomed-out, so you can see the whole thing (like the default behaviour of the real browser)
            web.Settings.LoadWithOverviewMode = true;
            web.Settings.UseWideViewPort = true;

            // scrollbar stuff
            web.ScrollBarStyle = ScrollbarStyles.OutsideOverlay; // so there's no 'white line'
            web.ScrollbarFadingEnabled = false;


            BackButton = FindViewById<Button>(Resource.Id.BackButton);
            ForwardButton = FindViewById<Button>(Resource.Id.ForwardButton);
            GoButton = FindViewById<Button>(Resource.Id.GoButton);
            StopButton = FindViewById<Button>(Resource.Id.StopButton);
            UrlText = FindViewById<EditText>(Resource.Id.UrlText);

            BackButton.Click += (s, e) => {
                web.GoBack();
            };
            ForwardButton.Click += (s, e) => {
                web.GoForward(); 
            };
            GoButton.Click += (s, e) => {
                web.LoadUrl(UrlText.Text);
            };
            StopButton.Click += (s, e) => {
                web.StopLoading();
            };
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

