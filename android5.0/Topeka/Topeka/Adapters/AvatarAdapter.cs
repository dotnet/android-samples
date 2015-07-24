using System;
using Android.Widget;
using Android.Views;
using Android.Content;

namespace Topeka.Adapters
{
	public class AvatarAdapter : BaseAdapter
	{
		static readonly Array avatars = Enum.GetValues(typeof(Avatar));

		readonly LayoutInflater layoutInflater;

        public override int Count
        {
            get
            {
                return avatars.Length;
            }
        }

        public AvatarAdapter(Context context) {
			layoutInflater = LayoutInflater.From(context);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null) {
				convertView = layoutInflater.Inflate(Resource.Layout.item_avatar, parent, false);
			}
			SetAvatar((AvatarView) convertView, (Avatar)avatars.GetValue(position));
			return convertView;
		}

		void SetAvatar(AvatarView mIcon, Avatar avatar) {
			mIcon.SetImageResource((int)avatar);
			mIcon.ContentDescription = avatar.ToString();
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return (Java.Lang.Object)avatars.GetValue(position);
		}

		public override long GetItemId (int position)
		{
			return position;
		}
	}
}

