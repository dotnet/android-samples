using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace ThemeSwitcher
{
    [Activity(Label = "Material Dark Action Bar", MainLauncher = false, Icon = "@drawable/icon", 
                        Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
    public class MaterialLightDarkBarActivity : Activity
    {
        // Activities for the other themes:
        private ActivityItem darkThemeActivity;
        private ActivityItem lightThemeActivity;
        private ActivityItem customThemeActivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Light Theme + Dark Bar" layout resource
            SetContentView(Resource.Layout.DarkBarTheme);

            // Create intents for the other theme activities:
            darkThemeActivity = new ActivityItem { 
                Title = "Material Dark", Intent = new Intent(this, typeof(MaterialDarkActivity)) };
            lightThemeActivity = new ActivityItem { 
                Title = "Material Light", Intent = new Intent(this, typeof(MaterialLightActivity)) };
            customThemeActivity = new ActivityItem { 
                Title = "Custom Theme", Intent = new Intent(this, typeof(MaterialCustomActivity)) };

            // Create buttons to select the other theme types:
            Button darkThemeBtn = FindViewById<Button>(Resource.Id.materialDarkButton);
            Button lightThemeBtn = FindViewById<Button>(Resource.Id.materialLightButton);
            Button customThemeBtn = FindViewById<Button>(Resource.Id.customThemeButton);

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

            // Launch the Custom Theme activity if selected:
            customThemeBtn.Click += delegate
            {
                StartActivity(customThemeActivity.Intent);
            };
        }
    }
}
