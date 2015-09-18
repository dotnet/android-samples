namespace com.xamarin.evolve2013.animationsdemo
{
    using Android.App;
    using Android.OS;
    using Android.Widget;

    [Activity(Theme = "@android:style/Theme.Holo.Light.DarkActionBar", Label = "@string/title_propertyanimation")]
    public class PropertyAnimationActivity : Activity
    {
        private KarmaMeter _karmaMeter;
        private SeekBar _seekBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_propertyanimation);
            _seekBar = FindViewById<SeekBar>(Resource.Id.karmaSeeker);
            _seekBar.StopTrackingTouch += (sender, args) =>{
                double karmaValue = ((double)_seekBar.Progress) / _seekBar.Max;
                _karmaMeter.SetKarmaValue(karmaValue, true);
            };
            _karmaMeter = FindViewById<KarmaMeter>(Resource.Id.karmaMeter);
        }
    }
}
