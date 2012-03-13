using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace TablesAndCellStyles {
    //Since: API Level 1 
    public class ActivityListItem_Adapter : ArrayAdapter <Tuple<string,int>> {
        Activity context;
        public ActivityListItem_Adapter(Activity context, IList<Tuple<string, int>> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            var item = GetItem(position);

            view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = item.Item1;
            view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item2);

            return view;
        }
    }
}