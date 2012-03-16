using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.GoogleMaps;
using Android.Widget;

namespace ContentControls {

    [Activity(Label = "LightTheme", Theme = "@android:style/Theme.Light")]
    public class LightThemeScreen : Activity { 

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

