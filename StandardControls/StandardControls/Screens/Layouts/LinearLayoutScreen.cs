using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "LinearLayout")]
    public class LinearLayoutScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LinearLayout);
        }
    }
}

