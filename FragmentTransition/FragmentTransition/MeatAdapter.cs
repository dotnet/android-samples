using System;

using Android.Widget;
using Android.App;
using Android.Views;

namespace FragmentTransition
{
	public class MeatAdapter : BaseAdapter<Meat>
	{
		private LayoutInflater mLayoutInflater;
		private int mResourceId;

		public MeatAdapter (LayoutInflater mLayoutInflater, int resourceId) : base ()
		{
			this.mLayoutInflater = mLayoutInflater;
			mResourceId = resourceId;
		}

		public override long GetItemId (int pos)
		{
			return this [pos].resourceId;
		}

		public override int Count {
			get { return Meat.MEATS.Length; }
		}

		public override Meat this [int position] {
			get { return Meat.MEATS [position]; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view;
			ViewHolder holder;
			if (null == convertView) {
				view = mLayoutInflater.Inflate (mResourceId, parent, false);
				System.Diagnostics.Debug.Assert (view != null);
				holder = new ViewHolder ();
				holder.image = view.FindViewById<ImageView> (Resource.Id.image);
				holder.title = view.FindViewById<TextView> (Resource.Id.title);
				view.Tag = holder;
			} else {
				view = convertView;
				holder = (ViewHolder)view.Tag;
			}
			BindView (holder, position);
			return view;
		}

		public void BindView (ViewHolder holder, int position)
		{
			Meat meat = this [position];
			holder.image.SetImageResource (meat.resourceId);
			holder.title.Text = meat.title;
		}

	}
	//Being a child of Java Object allows the ViewHolder to be set as a View's tag
	public class ViewHolder : Java.Lang.Object
	{
		public ImageView image;
		public TextView title;
	}
}

