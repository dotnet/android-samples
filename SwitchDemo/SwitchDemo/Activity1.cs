namespace SwitchDemo
{
    [Activity(Label = "SwitchDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);   

            var monitored_switch = RequireViewById<Switch>(Resource.Id.monitored_switch);
            monitored_switch.CheckedChange += (sender, e) =>
            {
                var answer = e.IsChecked ? "correct" : "incorrect";
                Toast.MakeText(this, $"Your answer is {answer}", ToastLength.Long)!.Show();
            };
        }
    }
}