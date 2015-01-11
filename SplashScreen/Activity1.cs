namespace SplashScreen
{
    using Android.App;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "Activity 1")]
    public class Activity1 : Activity
    {
        private Button _button;
        private int _clickCount;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _button = FindViewById<Button>(Resource.Id.MyButton);

            _button.Click += (sender, args) =>
                                 {
                                     _clickCount += 1;
                                     _button.Text = string.Format("You clicked {0} times.", _clickCount);
                                 };
        }
    }
}
