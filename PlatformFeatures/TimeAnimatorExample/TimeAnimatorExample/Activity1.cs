namespace TimeAnimatorExample
{
    using System;

    using Android.Animation;
    using Android.App;
    using Android.Media;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "TimeAnimatorExample", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, TimeAnimator.ITimeListener
    {
        public void OnTimeUpdate(TimeAnimator animation, long totalTime, long deltaTime)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
        }
    }
}
