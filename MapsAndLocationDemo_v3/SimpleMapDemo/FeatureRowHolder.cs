using Android.Content;
using Android.Views;
using Android.Widget;

namespace SimpleMapDemo
{
    class FeatureRowHolder : FrameLayout
    {
        readonly TextView _description;
        readonly TextView _title;

        public FeatureRowHolder(Context context)
            : base(context)
        {
            var inflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.Feature, this);
            _title = view.FindViewById<TextView>(Resource.Id.title);
            _description = view.FindViewById<TextView>(Resource.Id.description);
        }

        public void UpdateFrom(SampleMetaData sample)
        {
            _title.SetText(sample.TitleResource);
            _description.SetText(sample.DescriptionResource);
        }
    }
}
