using System.Collections.Generic;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace DirectBoot
{
	public class AlarmAdapter : RecyclerView.Adapter
	{
		public List<Alarm> Alarmlist { get; set; }
		public AlarmStorage AlarmStorage { get; set; }
		public AlarmUtil AlarmUtil { get; set; }
		public Context Context { get; set; }

		public AlarmAdapter (Context context, List<Alarm> alarms)
		{
			Context = context;
			Alarmlist = new List<Alarm> ();
			Alarmlist.AddRange (alarms);
			Alarmlist.Sort ();
			AlarmStorage = new AlarmStorage (context);
			AlarmUtil = new AlarmUtil (context);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.alarm_row, parent, false);
			return new AlarmViewHolder (v, this);
		}

		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			Alarm alarm = Alarmlist [position];
			var alarmHolder = (AlarmViewHolder)holder;
			var alarmTriggerTime = alarm.GetTriggerTime ();
			alarmHolder.AlarmTimeTextView.Text = alarmTriggerTime.ToShortTimeString ();
			alarmHolder.AlarmDateTextView.Text = alarmTriggerTime.ToShortDateString ();
		}

		public override int ItemCount {
			get {
				return Alarmlist.Count;
			}
		}

		public void AddAlarm (Alarm alarm)
		{
			Alarmlist.Add (alarm);
			Alarmlist.Sort ();
			NotifyDataSetChanged ();
		}

		public void DeleteAlarm (Alarm alarm)
		{
			Alarmlist.Remove (alarm);
			NotifyDataSetChanged ();
		}
	}

	public class AlarmViewHolder : RecyclerView.ViewHolder
	{
		public TextView AlarmTimeTextView { get; set; }
		public TextView AlarmDateTextView { get; set; }
		public ImageView DeleteImageView { get; set; }

		public AlarmViewHolder (View itemView, AlarmAdapter parent)
			: base (itemView)
		{
			AlarmTimeTextView = (TextView)itemView.FindViewById (Resource.Id.text_alarm_time);
			AlarmDateTextView = (TextView)itemView.FindViewById (Resource.Id.text_alarm_date);
			DeleteImageView = (ImageView)itemView.FindViewById (Resource.Id.image_delete_alarm);

			DeleteImageView.Click += delegate {
				Alarm toBeDeleted = parent.Alarmlist [AdapterPosition];
				parent.Alarmlist.RemoveAt (AdapterPosition);
				parent.AlarmStorage.DeleteAlarm (toBeDeleted);
				parent.AlarmUtil.CancelAlarm (toBeDeleted);
				parent.NotifyDataSetChanged ();
				Toast.MakeText (parent.Context, parent.Context.GetString (
					Resource.String.alarm_deleted, toBeDeleted.Hour, toBeDeleted.Minute), ToastLength.Short).Show ();
			};
		}
	}

	/**
     * ItemDecoration that draws an divider between items in a RecyclerView.
     */
	public class DividerItemDecorrection : RecyclerView.ItemDecoration
	{
		Drawable divider;

		public DividerItemDecorrection (Context context)
		{
			divider = context.GetDrawable (Resource.Drawable.divider);
		}

		public override void OnDrawOver (Android.Graphics.Canvas cValue, RecyclerView parent, RecyclerView.State state)
		{
			int left = parent.PaddingLeft;
			int right = parent.Width - parent.PaddingRight;
			for (int i = 0; i < parent.ChildCount; i++) {
				View child = parent.GetChildAt (i);
				var param = (RecyclerView.LayoutParams)child.LayoutParameters;
				int top = child.Bottom + param.BottomMargin;
				int bottom = top + divider.IntrinsicHeight;
				divider.SetBounds (left, top, right, bottom);
				divider.Draw (cValue);
			}
		}
	}
}

