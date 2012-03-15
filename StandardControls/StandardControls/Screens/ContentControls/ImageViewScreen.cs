using Android.App;
using Android.OS;

namespace StandardControls {

    [Activity(Label = "ImageView")]
    public class ImageViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ImageView);
        }
    }
}

