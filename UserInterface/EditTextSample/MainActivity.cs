namespace EditTextSample
{
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

    [Activity(Label = "EditTextSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            EditText edittext = FindViewById<EditText>(Resource.Id.edittext);

            edittext.KeyPress += (object sender, View.KeyEventArgs e) => {
				e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    Toast.MakeText(this, edittext.Text, ToastLength.Short).Show();
                    e.Handled = true;
				}
            };
        }
    }
}
