using Android.Content;
using Android.Views;
using Android.Widget;

namespace SimpleMapDemo
{
    class FeatureRowHolder : FrameLayout
    {
        readonly TextView description;
        readonly TextView title;

        public FeatureRowHolder(Context context)
            : base(context)
        {
            var inflater = (LayoutInflater) context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.Feature, this);
            title = view.FindViewById<TextView>(Resource.Id.title);
            description = view.FindViewById<TextView>(Resource.Id.description);
        }

        public void UpdateFrom(SampleActivityMetaData sample)
        {
            title.SetText(sample.TitleResource);
            description.SetText(sample.DescriptionResource);
        }
    }
}
