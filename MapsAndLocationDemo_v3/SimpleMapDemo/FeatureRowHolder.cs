namespace SimpleMapDemo
{
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    internal class FeatureRowHolder : FrameLayout
    {
        private readonly TextView _description;
        private readonly TextView _title;

        public FeatureRowHolder(Context context)
            : base(context)
        {
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.Feature, this);
            _title = view.FindViewById<TextView>(Resource.Id.title);
            _description = view.FindViewById<TextView>(Resource.Id.description);
        }

        public void UpdateFrom(SampleActivity sample)
        {
            _title.SetText(sample.TitleResource);
            _description.SetText(sample.DescriptionResource);
        }
    }
}