namespace SwitchDemo
{
    [Activity(Label = "SwitchDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);   

            var s = FindViewById<Switch>(Resource.Id.monitored_switch);

            ArgumentNullException.ThrowIfNull(s);
            s.CheckedChange += (sender, e) => {

                var toast = Toast.MakeText(this, "Your answer is " + (e.IsChecked ? "correct" : "incorrect"),
                                            ToastLength.Short);
                toast?.Show();
            };
        }
    }
}