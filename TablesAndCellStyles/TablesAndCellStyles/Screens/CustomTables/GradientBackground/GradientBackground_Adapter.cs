using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace TablesAndCellStyles {

    public class GradientBackground_Adapter : ArrayAdapter <Tuple<string,string,int>> {
        Activity context;
        public GradientBackground_Adapter(Activity context, IList<Tuple<string, string, int>> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.ImageAndSubtitleItem, null);
            var item = GetItem(position);

            view.FindViewById<TextView> (Resource.Id.Text1).Text = item.Item1;
            view.FindViewById<TextView> (Resource.Id.Text2).Text = item.Item2;
            view.FindViewById<ImageView> (Resource.Id.Icon).SetImageResource(item.Item3);

            return view;
        }
    }
}