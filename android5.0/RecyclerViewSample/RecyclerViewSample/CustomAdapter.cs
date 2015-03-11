using System;
using CommonSampleLibrary;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace RecyclerViewSample
{
	public class CustomAdapter : RecyclerView.Adapter
	{
		public const string TAG = "CustomAdapter";
		private string[] dataSet;

		// Provide a reference to the type of views that you are using (custom ViewHolder)
		public class ViewHolder : RecyclerView.ViewHolder
		{
			private TextView textView;

			public TextView TextView
			{
				get{ return textView; }
			}

			public ViewHolder(View v) : base(v)
			{
				textView = (TextView) v.FindViewById(Resource.Id.textView);
			}
		}

		// Initialize the dataset of the Adapter
		public CustomAdapter(string[] dataSet)
		{
			this.dataSet = dataSet;
		}

		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup viewGroup, int position)
		{
			View v = LayoutInflater.From (viewGroup.Context)
				.Inflate (Resource.Layout.text_row_item, viewGroup, false);
			ViewHolder vh = new ViewHolder (v);
			return vh;
		}

		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			Log.Debug (TAG, "\tElement " + position + " set.");

			// Get element from your dataset at this position and replace the contents of the view
			// with that element
			(viewHolder as ViewHolder).TextView.SetText(dataSet [position],TextView.BufferType.Normal);
		}

		// Return the size of your dataset (invoked by the layout manager)
		public override int ItemCount
		{
			get{ return dataSet.Length; }
		}
	}
}

