using Android.App;
using Android.Runtime;
using Android.Webkit;
using Android.Widget;

namespace SafeBrowsing
{
    public class WebViewClientImpl : WebViewClient
    {
        public Activity Activity { get; set; }
        public override void OnSafeBrowsingHit(WebView view, IWebResourceRequest request, [GeneratedEnum] SafeBrowsingThreat threatType, SafeBrowsingResponse callback)
        {
            callback.BackToSafety(true);
            var textView = Activity.FindViewById<TextView>(Resource.Id.safe_browsing_status);
            Activity.ActionBar.SetTitle(Resource.String.blocked);
        }
    }
}