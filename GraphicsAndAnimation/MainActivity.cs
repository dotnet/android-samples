namespace com.xamarin.evolve2013.animationsdemo
{
    using System;
    using System.Collections.Generic;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

    [Activity(Label = "@string/app_name", 
        MainLauncher = true,
        Theme = "@android:style/Theme.Holo.Light.DarkActionBar",
        Icon = "@drawable/ic_launcher")]
    public class MainActivity : ListActivity
    {
        private List<SampleActivity> _activities;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _activities = new List<SampleActivity>
                              {
                                  new SampleActivity(Resources.GetString(Resource.String.title_drawing), typeof(DrawingActivity)),
                                  new SampleActivity(Resources.GetString(Resource.String.title_shapedrawables), typeof(ShapeDrawableActivity)),
                                  new SampleActivity(Resources.GetString(Resource.String.title_animationdrawable), typeof(AnimationDrawableActivity)),
                                  new SampleActivity(Resources.GetString(Resource.String.title_viewanimation), typeof(ViewAnimationActivity)),
                                  new SampleActivity(Resources.GetString(Resource.String.title_propertyanimation), typeof(PropertyAnimationActivity)),
                                  new SampleActivity(Resources.GetString(Resource.String.title_zoom), typeof(ZoomActivity))
                              };

            ListAdapter = new ArrayAdapter<SampleActivity>(this,
                                                           Android.Resource.Layout.SimpleListItem1,
                                                           Android.Resource.Id.Text1,
                                                           _activities);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            SampleActivity sample = _activities[position];
            sample.Start(this);
        }
    }

    public class SampleActivity
    {
        public SampleActivity(string title, Type activityToLaunch)
        {
            Title = title;
            ActivityToLaunch = activityToLaunch;
        }

        private Type ActivityToLaunch { get; set; }
        private string Title { get; set; }

        public void Start(Activity context)
        {
            Intent i = new Intent(context, ActivityToLaunch);
            context.StartActivity(i);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
