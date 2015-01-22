using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace ThemeSwitcher
{
    [Activity(Label = "Material Dark Theme", MainLauncher = true, Icon = "@drawable/icon", 
		      Theme = "@android:style/Theme.Material")]
    public class MaterialDarkActivity : Activity
    {
        // Activities for the other three themes:
        private ActivityItem lightThemeActivity;
        private ActivityItem lightDarkBarThemeActivity;
        private ActivityItem customThemeActivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Dark Theme" layout resource
            SetContentView(Resource.Layout.DarkTheme);

            // Create intents for the other two theme activities:
            lightThemeActivity = new ActivityItem { 
                Title = "Material Light", Intent = new Intent(this, typeof(MaterialLightActivity)) };
            lightDarkBarThemeActivity = new ActivityItem { 
                Title = "Material Dark Action Bar", Intent = new Intent(this, typeof(MaterialLightDarkBarActivity)) };
            customThemeActivity = new ActivityItem { 
                Title = "Custom Theme", Intent = new Intent(this, typeof(MaterialCustomActivity)) };

            // Create buttons to select the other theme types:
            Button lightThemeBtn = FindViewById<Button>(Resource.Id.materialLightButton);
            Button lightDarkBarThemeBtn = FindViewById<Button>(Resource.Id.materialLightDarkBarButton);
            Button customThemeBtn = FindViewById<Button>(Resource.Id.customThemeButton);

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

            // Launch the Custom Theme activity if selected:
            customThemeBtn.Click += delegate
            {
                StartActivity(customThemeActivity.Intent);
            };

        }
    }
}

