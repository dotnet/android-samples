using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "FrameLayout")]
    public class FrameLayoutScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.FrameLayout);
        }
    }
}

