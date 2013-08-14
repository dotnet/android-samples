namespace SimpleMapDemo
{
    using System.Collections.Generic;
    using System.Linq;

    using Android.Content;
    using Android.Views;
    using Android.Widget;

    internal class SimpleMapDemoActivityAdapter : BaseAdapter<SampleActivity>
    {
        private readonly List<SampleActivity> _activities;
        private readonly Context _context;

        public SimpleMapDemoActivityAdapter(Context context, IEnumerable<SampleActivity> sampleActivities)
        {
            _context = context;
            if (sampleActivities == null)
            {
                _activities = new List<SampleActivity>(0);
            }
            else 
            {
                _activities = sampleActivities.ToList();
            }
        }

        public override int Count { get { return _activities.Count; } }

        public override SampleActivity this[int position] { get { return _activities[position]; } }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FeatureRowHolder row = convertView as FeatureRowHolder ?? new FeatureRowHolder(_context);
            SampleActivity sample = _activities[position];

            row.UpdateFrom(sample);
            return row;
        }
    }
}