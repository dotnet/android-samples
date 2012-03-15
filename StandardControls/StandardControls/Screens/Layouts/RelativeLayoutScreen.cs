using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "RelativeLayout")]
    public class RelativeLayoutScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.RelativeLayout);
        }
    }
}

