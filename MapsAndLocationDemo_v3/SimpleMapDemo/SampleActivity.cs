namespace SimpleMapDemo
{
    using System;

    using Android.App;
    using Android.Content;

    /// <summary>
    /// This class holds meta-data about the various activities that are used in this application.
    /// </summary>
    internal class SampleActivity
    {
        public SampleActivity(int titleResourceId, int descriptionId, Type activityToLaunch)
        {
            ActivityToLaunch = activityToLaunch;
            TitleResource = titleResourceId;
            DescriptionResource = descriptionId;
        }

        public Type ActivityToLaunch { get; private set; }
        public int DescriptionResource { get; private set; }
        public int TitleResource { get; private set; }

        public void Start(Activity context)
        {
            Intent i = new Intent(context, ActivityToLaunch);
            context.StartActivity(i);
        }
    }
}