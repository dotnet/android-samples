using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "Button")]
    public class ButtonScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Button);

            var normalButton = FindViewById<Button>(Resource.Id.NormalButton);
            normalButton.Click += (sender, args) => {
                Toast.MakeText(this, "Normal button clicked", ToastLength.Short).Show();
            };

            var imageButton = FindViewById<ImageButton>(Resource.Id.ImageButton);
            imageButton.Click += (sender, args) => {
                Toast.MakeText(this, "Image button clicked", ToastLength.Short).Show();
            };

            var toggleButton = FindViewById<ToggleButton>(Resource.Id.ToggleButton);
            toggleButton.Click += (sender, args) => {
                Toast.MakeText(this, "Toggle button checked=" + toggleButton.Checked, ToastLength.Short).Show();
            };
        }
    }
}

