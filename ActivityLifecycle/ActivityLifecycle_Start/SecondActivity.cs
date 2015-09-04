namespace ActivityLifecycle
{
    using Android.App;
    using Android.OS;
    using Android.Util;

    [Activity(Label = "Activity B")]
    public class SecondActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Log.Debug(GetType().FullName, "Activity B - OnCreate");
            base.OnCreate(bundle);
        }

        protected override void OnRestart()
        {
            Log.Debug(GetType().FullName, "Activity B - OnRestart");
            base.OnRestart();
        }

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, "Activity B - OnStart");
            base.OnStart();
        }

        protected override void OnResume()
        {
            Log.Debug(GetType().FullName, "Activity B - OnResume");
            base.OnResume();
        }

        protected override void OnPause()
        {
            Log.Debug(GetType().FullName, "Activity B - OnPause");
            base.OnPause();
        }

        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, "Activity B - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, "Activity B - OnDestroy");
            base.OnDestroy();
        }


    }
}
