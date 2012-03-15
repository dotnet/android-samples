using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "RadioGroup")]
    public class RadioGroupScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.RadioGroup);
        }
    }
}

