using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "HorizontalLinearLayoutWeight")]
    public class HorizontalLinearLayoutWeightScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HorizontalLinearLayoutWeight);
        }
    }
}

