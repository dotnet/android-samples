using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace ContentControls {

    [Activity(Label = "WebViewInterop")]
    public class WebViewInteropScreen : Activity {
        WebView wv;
        Button RunScriptButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.WebViewInterop);

            wv = FindViewById<WebView>(Resource.Id.Web);
            wv.Settings.JavaScriptEnabled = true;

            // wire up the c#-to-javascript button
            RunScriptButton = FindViewById<Button>(Resource.Id.RunScriptButton);
            RunScriptButton.Click += (s, e) => {
                wv.LoadUrl("javascript:RunAction();");
                wv.LoadUrl("javascript:SetContent('Yay for content from C#');");
            };

            wv.LoadUrl("file:///android_asset/Content/InteractivePages/Home.html");
            
            wv.SetWebViewClient(new MonkeyWebViewClient(this));
            wv.SetWebChromeClient(new MonkeyWebChromeClient());     // required for javascript:alert() handling

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
        
        class MonkeyWebChromeClient : WebChromeClient {
            public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
            {
                // the built-in alert is pretty ugly, you could do something different here if you wanted to
                return base.OnJsAlert(view, url, message, result);
            }
        }

        class MonkeyWebViewClient : WebViewClient {
            Activity context;
            public MonkeyWebViewClient(Activity context)
            {
                this.context = context;
            }
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (url.StartsWith("file://LOCAL")) {
                    // parse the url to decide what to do
                    Toast.MakeText(context, "Handle the link in c#\n"+url, ToastLength.Short).Show();
                } else {
                    view.LoadUrl(url);
                }
                return true;
            }
        }
    }
}

