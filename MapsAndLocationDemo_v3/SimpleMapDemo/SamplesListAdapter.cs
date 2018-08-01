using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace SimpleMapDemo
{
    class SamplesListAdapter : BaseAdapter<SampleMetaData>
    {
        readonly List<SampleMetaData> _activities;
        readonly Context _context;

        public SamplesListAdapter(Context context, IEnumerable<SampleMetaData> sampleActivities)
        {
            _context = context;
            _activities = sampleActivities == null ? new List<SampleMetaData>(0) : sampleActivities.ToList();
        }

        public override int Count => _activities.Count;

        public override SampleMetaData this[int position] => _activities[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = convertView as FeatureRowHolder ?? new FeatureRowHolder(_context);
            var sample = _activities[position];

            row.UpdateFrom(sample);
            return row;
        }
    }
}
