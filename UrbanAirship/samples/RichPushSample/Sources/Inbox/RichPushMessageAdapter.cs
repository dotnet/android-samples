using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.UrbanAirship.RichPush;
using Xamarin.UrbanAirship;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * ArrayAdapter for rich push messages
 *
 */
	public class RichPushMessageAdapter : ArrayAdapter<RichPushMessage> {

		int layout;
		IViewBinder binder;
		private IList<RichPushMessage> messages;

		/**
     * Creates a new RichPushMessageAdapter
     * @param context Application context
     * @param layout The layout for the created views
     * @param mapping The mapping for message value to view
     */
		public RichPushMessageAdapter(Context context, int layout)
			: this(context, layout, new List<RichPushMessage>()) {
		}

		RichPushMessageAdapter(Context context, int layout, IList<RichPushMessage> messages) 
			: base (context, layout, messages)
		{
			this.layout = layout;
			this.messages = messages;
		}

		private View createView(ViewGroup parent) {
			LayoutInflater layoutInflater = (LayoutInflater) Context.GetSystemService(Context.LayoutInflaterService);
			View view = layoutInflater.Inflate(layout, parent, false);
			return view;
		}

		override
			public View GetView(int position, View convertView, ViewGroup parent) {

			// Use either the convertView or create a new view
			View view = convertView == null ? createView(parent) : convertView;
			RichPushMessage message = this.GetItem(position);

			if (message == null) {
				Logger.Error("Message at " + position + " is null!");
				return view;
			}

			binder.BindView(view, message);
			return view;
		}

		/**
     * Sets the view binder
     * @param binder The specified view binder
     */
		public void SetViewBinder(IViewBinder binder) {
			this.binder = binder;
		}

		/**
     * Sets the current list of rich push messages and notifies data set changed
     *
     * Must be called on the ui thread
     *
     * @param messages Current list of rich push messages
     */
		public void SetMessages(IList<RichPushMessage> messages) {
			this.messages.Clear();
			foreach (var item in messages)
				this.messages.Add (item);
			this.NotifyDataSetChanged();
		}

		/**
     * View binder interface
     *
     */
		public interface IViewBinder {
			void BindView(View view, RichPushMessage message);
		}
	}
}
