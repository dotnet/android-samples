using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "TableLayout")]
    public class TableLayoutScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TableLayout);
        }
    }
}

