using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace TablesAndCellStyles {
    //Since: API Level 1 
    public class TwoLineListItem_Adapter : ArrayAdapter <Tuple<string,string>> {
        Activity context;
        public TwoLineListItem_Adapter(Activity context, IList<Tuple<string, string>> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);

            var item = GetItem(position);

            view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = item.Item1;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Item2;

            return view;
        }
    }
}