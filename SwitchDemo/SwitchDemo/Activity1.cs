namespace SwitchDemo
{
    [Activity(Label = "SwitchDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);   

            var monitored_switch = FindViewById<Switch>(Resource.Id.monitored_switch);
            ArgumentNullException.ThrowIfNull(monitored_switch);
            monitored_switch.CheckedChange += (sender, e) =>
            {
                var toast = Toast.MakeText(this, "Your answer is " + (e.IsChecked ? "correct" : "incorrect"), ToastLength.Short);
                toast!.Show();
            };
        }
    }
}