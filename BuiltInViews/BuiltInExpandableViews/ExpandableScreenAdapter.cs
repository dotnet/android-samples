using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuiltInExpandableViews
{
    public class ExpandableScreenAdapter : BaseExpandableListAdapter
    {
        // Context, usually set to the activity:
        private readonly Context _context;

        // List of produce objects ("vegetables", "fruits", "herbs"):
        private readonly List<Produce> _produce;

        public ExpandableScreenAdapter(Context context, List<Produce> produce)
        {
            _context = context;
            _produce = produce;
        }

        public override bool HasStableIds
        {
            // Indexes are used for IDs:
            get { return true; }
        }

        //---------------------------------------------------------------------------------------
        // Group methods:

        public override long GetGroupId(int groupPosition)
        {
            // The index of the group is used as its ID:
            return groupPosition;
        }

        public override int GroupCount
        {
            // Return the number of produce ("vegetables", "fruits", "herbs") objects:
            get { return _produce.Count; }
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            // Recycle a previous view if provided:
            var view = convertView;

            // If no recycled view, inflate a new view as a simple expandable list item 1:
            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Android.Resource.Layout.SimpleExpandableListItem1, null);
            }

            // Grab the produce object ("vegetables", "fruits", etc.) at the group position:
            var produce = _produce[groupPosition];

            // Get the built-in first text view and insert the group name ("Vegetables", "Fruits", etc.):
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = produce.Type;

            return view;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        //---------------------------------------------------------------------------------------
        // Child methods:

        public override long GetChildId(int groupPosition, int childPosition)
        {
            // The index of the child is used as its ID:
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            // Return the number of children (produce item objects) in the group (produce object):
            var produce = _produce[groupPosition];
            return produce.ProduceItems.Length;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            // Recycle a previous view if provided:
            var view = convertView;

            // If no recycled view, inflate a new view as a simple expandable list item 2:
            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Android.Resource.Layout.SimpleExpandableListItem2, null);
            }

            // Grab the produce object ("vegetables", "fruits", etc.) at the group position:
            var produce = _produce[groupPosition];

            // Extract the produce item object ("bananas", "apricots", etc.) at the child position:
            var produceItem = produce.ProduceItems[childPosition];

            // Get the built-in first text view and insert the child name ("Bananas", "Apricots", etc.):
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = produceItem.Name;

            // Reuse the textView to insert the number of produce units into the child's second text field:
            textView = view.FindViewById<TextView>(Android.Resource.Id.Text2);
            textView.Text = produceItem.Count.ToString() + " units";

            return view;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}
