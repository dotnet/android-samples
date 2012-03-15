using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "RatingBar")]
    public class RatingBarScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.RatingBar);
        }
    }
}

