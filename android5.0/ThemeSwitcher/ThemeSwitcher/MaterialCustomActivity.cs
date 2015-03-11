using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace ThemeSwitcher
{
    [Activity(Label = "Material Custom Theme", MainLauncher = false, Icon = "@drawable/icon", 
              Theme = "@style/MyCustomTheme")]
    public class MaterialCustomActivity : Activity
    {
        // Activities for the other three themes:
        private ActivityItem darkThemeActivity;
        private ActivityItem lightThemeActivity;
        private ActivityItem lightDarkBarThemeActivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Custom Theme" layout resource
            SetContentView(Resource.Layout.CustomTheme);

            // Create intents for the other two theme activities:
            darkThemeActivity = new ActivityItem { 
                Title = "Material Dark", Intent = new Intent(this, typeof(MaterialDarkActivity)) };
            lightThemeActivity = new ActivityItem { 
                Title = "Material Light", Intent = new Intent(this, typeof(MaterialLightActivity)) };
            lightDarkBarThemeActivity = new ActivityItem { 
                Title = "Material Dark Action Bar", Intent = new Intent(this, typeof(MaterialLightDarkBarActivity)) };

            // Create buttons to select the other theme types:
            Button darkThemeBtn = FindViewById<Button>(Resource.Id.materialDarkButton);
            Button lightThemeBtn = FindViewById<Button>(Resource.Id.materialLightButton);
            Button lightDarkBarThemeBtn = FindViewById<Button>(Resource.Id.materialLightDarkBarButton);

            // Launch the Dark Theme activity if selected:
            darkThemeBtn.Click += delegate
            {
                StartActivity(darkThemeActivity.Intent);
            };

            // Launch the Light Theme activity if selected:
            lightThemeBtn.Click += delegate
            {
                StartActivity(lightThemeActivity.Intent);
            };

            // Launch the Light Theme with Dark Action Bar activity if selected:
            lightDarkBarThemeBtn.Click += delegate
            {
                StartActivity(lightDarkBarThemeActivity.Intent);
            };
        }
    }
}
