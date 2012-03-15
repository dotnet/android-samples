using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "CheckBox")]
    public class CheckBoxScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CheckBox);
        }
    }
}

