using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BuiltInViews {
    public class HomeScreenAdapter : BaseAdapter<TableItem> {
        List<TableItem> items;
        Activity context;
        public HomeScreenAdapter(Activity context, List<TableItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override TableItem this[int position]
        {   
            get { return items[position]; } 
        }
        public override int Count {
            get { return items.Count; } 
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.ImageResourceId);

            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;
            
            return view;
        }
    }
}