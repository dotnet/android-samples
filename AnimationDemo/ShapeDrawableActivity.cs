namespace com.xamarin.evolve2013.animationsdemo
{
    using Android.App;
    using Android.OS;

    [Activity(Label = "@string/title_shapedrawables", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class ShapeDrawableActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_shapedrawable);
        }
    }
}
