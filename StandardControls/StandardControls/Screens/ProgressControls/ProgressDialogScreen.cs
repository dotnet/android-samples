using System.Threading;
using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "ProgressDialog")]
    public class ProgressDialogScreen : Activity {

        ProgressDialog progress;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ProgressDialog);
            
            progress = ProgressDialog.Show(this, "Loading...", "Please Wait (about 4 seconds)", true); 

             new Thread(new ThreadStart(() => {
                 Thread.Sleep(4 * 1000);
                 this.RunOnUiThread ( () => {
                    FindViewById<TextView>(Resource.Id.Text1).Text = "...and we're done!";
                    progress.Dismiss();
                 });
             })).Start();
        }
    }
}

