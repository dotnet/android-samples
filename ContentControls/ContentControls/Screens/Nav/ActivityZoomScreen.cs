using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace ContentControls {

    [Activity(Label = "ActivityZoom")]
    public class ActivityZoomScreen : Activity {

        /*
         * To animate the appearance of this activity, see Home.cs
         * (and the animations defined in \Resources\Anim\
         * 
                StartActivity(sample);
                OverridePendingTransition(Resource.Animation.zoom_enter, Resource.Animation.zoom_exit);
         */

        string[] planets = {
			"Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"
		};

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LightTheme);

            var s1 = FindViewById<Spinner>(Resource.Id.spinner1);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, planets);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            s1.Adapter = adapter;
        }
    }
}

