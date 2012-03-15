using Android.App;
using Android.OS;
using Android.Text;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "TextView")]
    public class TextViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TextView);

            var stv = FindViewById<TextView>(Resource.Id.StyledTextView);
            var html = Html.FromHtml("Hello <b>there</b>, how <i>are</i> you today? (<b><i>html formatted</i></b>)");
            
            stv.TextFormatted = html;
        }
    }
}

