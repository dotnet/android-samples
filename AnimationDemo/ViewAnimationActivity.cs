namespace com.xamarin.evolve2013.animationsdemo
{
    using Android.App;
    using Android.OS;
    using Android.Views.Animations;
    using Android.Widget;

    [Activity(Theme = "@android:style/Theme.Holo.Light.DarkActionBar", Label = "@string/title_viewanimation")]
    public class ViewAnimationActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_imageandbutton);

            FindViewById<ImageView>(Resource.Id.imageView1).SetImageResource(Resource.Drawable.ship2_2);
            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Text = Resources.GetString(Resource.String.title_hyperspace);
            button.Click += (sender, args) =>{
                Animation hyperspaceAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.hyperspace);
                FindViewById<ImageView>(Resource.Id.imageView1).StartAnimation(hyperspaceAnimation);
            };
        }
    }
}
