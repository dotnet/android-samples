using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using System;

namespace TablesAndCellStyles {
    public class DateList_Adapter : BaseAdapter<Tuple<string,DateTime>> {
        protected Activity context = null;
        protected IList<Tuple<string, DateTime>> news = new List<Tuple<string, DateTime>>();

        public DateList_Adapter(Activity context, IList<Tuple<string, DateTime>> news)
            : base()
        {
            this.context = context;
            this.news = news;
        }

        public override Tuple<string, DateTime> this[int position]
        {
            get { return news[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return news.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for this position
            var item = news[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // This gives us some performance gains by not always inflating a new view
            // This will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
            var view = (convertView ??
                    this.context.LayoutInflater.Inflate(
                    Resource.Layout.DateListItem,
                    parent,
                    false)) as LinearLayout;

            // Find references to each subview in the list item's view
            var titleTextView = view.FindViewById<TextView>(Resource.Id.BigTextView);
            var monthTextView = view.FindViewById<TextView>(Resource.Id.MonthTextView);
            var dayTextView = view.FindViewById<TextView>(Resource.Id.DayTextView);

            //Assign this item's values to the various subviews
            titleTextView.SetText(news[position].Item1, TextView.BufferType.Normal);
            monthTextView.SetText(news[position].Item2.ToString("MMM").ToUpper(), TextView.BufferType.Normal);
            dayTextView.SetText(news[position].Item2.ToString("dd"), TextView.BufferType.Normal);

            //Finally return the view
            return view;
        }
    }
}

