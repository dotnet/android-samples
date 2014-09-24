using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace ThemeSwitcher
{
    [Activity(Label = "Material Light Theme", MainLauncher = false, Icon = "@drawable/icon", 
              Theme = "@android:style/Theme.Material.Light")]
    public class MaterialLightActivity : Activity
    {
        // Activities for the other themes:
        private ActivityItem darkThemeActivity;
        private ActivityItem lightDarkBarThemeActivity;
        private ActivityItem customThemeActivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Light Theme" layout resource
            SetContentView(Resource.Layout.LightTheme);

            // Create intents for the other two theme activities:
            darkThemeActivity = new ActivityItem { 
                Title = "Material Dark", Intent = new Intent(this, typeof(MaterialDarkActivity)) };
            lightDarkBarThemeActivity = new ActivityItem { 
                Title = "Material Dark Action Bar", Intent = new Intent(this, typeof(MaterialLightDarkBarActivity)) };
            customThemeActivity = new ActivityItem { 
                Title = "Custom Theme", Intent = new Intent(this, typeof(MaterialCustomActivity)) };

            // Create buttons to select the other theme types:
            Button darkThemeBtn = FindViewById<Button>(Resource.Id.materialDarkButton);
            Button lightDarkBarThemeBtn = FindViewById<Button>(Resource.Id.materialLightDarkBarButton);
            Button customThemeBtn = FindViewById<Button>(Resource.Id.customThemeButton);

            // Launch the Dark Theme activity if selected:
            darkThemeBtn.Click += delegate
            {
                StartActivity(darkThemeActivity.Intent);
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
