using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "HorizontalScrollView")]
    public class HorizontalScrollViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HorizontalScrollView);
        }
    }
}

