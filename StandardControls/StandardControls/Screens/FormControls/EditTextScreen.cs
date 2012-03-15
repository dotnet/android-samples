using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "EditText")]
    public class EditTextScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.EditText);
        }
    }
}

