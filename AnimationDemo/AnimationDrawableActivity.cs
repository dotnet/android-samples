namespace com.xamarin.evolve2013.animationsdemo
{
    using Android.App;
    using Android.Graphics.Drawables;
    using Android.OS;
    using Android.Widget;

    [Activity(Theme = "@android:style/Theme.Holo.Light.DarkActionBar", Label = "@string/title_animationdrawable")]
    public class AnimationDrawableActivity : Activity
    {
        private AnimationDrawable _asteroidDrawable;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_imageandbutton);

            // Load the animation from resources
            _asteroidDrawable = (AnimationDrawable)Resources.GetDrawable(Resource.Drawable.spinning_asteroid);
            ImageView imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView.SetImageDrawable(_asteroidDrawable);

            Button spinAsteroidButton = FindViewById<Button>(Resource.Id.button1);
            spinAsteroidButton.Text = Resources.GetString(Resource.String.title_spinasteroid);
            spinAsteroidButton.Click += (sender, args) => _asteroidDrawable.Start();
        }
    }
}
