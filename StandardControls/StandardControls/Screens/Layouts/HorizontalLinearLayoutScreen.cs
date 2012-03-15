using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "HorizontalLinearLayout")]
    public class HorizontalLinearLayoutScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HorizontalLinearLayout);
        }
    }
}

