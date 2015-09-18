namespace com.xamarin.evolve2013.animationsdemo
{
    using Android.App;
    using Android.OS;

    [Activity(Label = "@string/title_drawing", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class DrawingActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_drawing);

            KarmaMeter meter = FindViewById<KarmaMeter>(Resource.Id.karmaMeter);

            meter.KarmaValue = 0.25d;
        }
    }
}
