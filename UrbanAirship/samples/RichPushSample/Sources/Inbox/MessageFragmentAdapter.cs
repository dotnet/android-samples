using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Xamarin.UrbanAirship.RichPush;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * Pager adapter that manages the message fragments
 *
 */
	public class MessageFragmentAdapter :FragmentStatePagerAdapter {

		private IList<RichPushMessage> messages;

		public MessageFragmentAdapter(FragmentManager manager) 
			: base (manager)
		{
		}

		override
			public Fragment GetItem(int position) {
			if (messages == null || position >= messages.Count) {
				return null;
			}

			String messageId = messages [position].MessageId;
			MessageFragment fragment = MessageFragment.NewInstance(messageId);
			return fragment;
		}

		override
			public int Count {
			get {
				if (messages == null) {
					return 0;
				}
				return messages.Count;
			}
		}

		override
			public int GetItemPosition(Java.Lang.Object item) {

			// The default implementation of this method returns POSITION_UNCHANGED, which effectively
			// assumes that fragments will never change position or be destroyed
			return PositionNone;
		}

		/**
     * Set the list of rich push messages
     * @param messages The current list of rich push messages to display
     */
		public void SetRichPushMessages(IList<RichPushMessage> messages) {
			this.messages = messages;
			this.NotifyDataSetChanged();
		}
	}
}
