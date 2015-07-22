using System;
using Android.Widget;
using Android.Media.Session;
using Android.App;
using System.Collections.Generic;
using Android.Views;

namespace MediaBrowserService
{
	public class QueueAdapter : ArrayAdapter<MediaSession.QueueItem>
	{
		public long ActiveQueueItemId { get; set; }

		public QueueAdapter(Activity context) : 
		base(context, Resource.Layout.media_list_item, new List<MediaSession.QueueItem>())
		{
			ActiveQueueItemId = MediaSession.QueueItem.UnknownId;
		}

		class ViewHolder : Java.Lang.Object {
			public ImageView imageView;
			public TextView titleView;
			public TextView descriptionView;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder holder;

			if (convertView == null) {
				convertView = LayoutInflater.From(Context)
					.Inflate(Resource.Layout.media_list_item, parent, false);
				holder = new ViewHolder();
				holder.imageView = (ImageView) convertView.FindViewById(Resource.Id.play_eq);
				holder.titleView = (TextView) convertView.FindViewById(Resource.Id.title);
				holder.descriptionView = (TextView) convertView.FindViewById(Resource.Id.description);
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder) convertView.Tag;
			}

			var item = GetItem(position);
			holder.titleView.Text = item.Description.Title;
			if (item.Description.Description != null) {
				holder.descriptionView.Text = item.Description.Description;
			}

			if (ActiveQueueItemId == item.QueueId) {
				holder.imageView.SetImageDrawable(
					Context.GetDrawable(Resource.Drawable.ic_equalizer_white_24dp));
			} else {
				holder.imageView.SetImageDrawable(
					Context.GetDrawable(Resource.Drawable.ic_play_arrow_white_24dp));
			}
			return convertView;
		}
	}
}

