using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "ScrollView")]
    public class ScrollViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScrollView);
        }
    }
}

