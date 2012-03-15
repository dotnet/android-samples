using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "LinearLayoutWeight")]
    public class LinearLayoutWeightScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LinearLayoutWeight);
        }
    }
}

