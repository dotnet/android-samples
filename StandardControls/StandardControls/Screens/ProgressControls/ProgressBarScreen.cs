using System.Threading;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "ProgressBar")]
    public class ProgressBarScreen : Activity {

        ProgressBar progress;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ProgressBar);

            progress = FindViewById<ProgressBar>(Resource.Id.ProgressBar);

             new Thread(new ThreadStart(() => {
                 for (int i = 0; i <= 100; i++) {
                     this.RunOnUiThread ( () => {
                        progress.Progress = i;
                     });
                     Thread.Sleep(30);
                 }

                 this.RunOnUiThread(() => {
                     FindViewById<TextView>(Resource.Id.Text1).Text = "";
                     FindViewById<TextView>(Resource.Id.Text2).Text = "...and we're done!";
                     FindViewById<ProgressBar>(Resource.Id.ProgressCircle).ClearAnimation();
                     FindViewById<ProgressBar>(Resource.Id.ProgressCircle).Visibility = ViewStates.Invisible;
                 });
             })).Start();
        }
    }
}

