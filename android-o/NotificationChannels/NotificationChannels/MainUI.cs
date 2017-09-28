using Android.Widget;
using Android.Views;
using Android.Util;

namespace NotificationChannels
{
    public class MainUI : Java.Lang.Object, View.IOnClickListener
    {
        MainActivity self;

        public MainUI(View root, MainActivity self)
        {
            this.self = self;

            titlePrimary = (TextView)root.FindViewById(Resource.Id.main_primary_title);
            ((Button)root.FindViewById(Resource.Id.main_primary_send1)).SetOnClickListener(this);
            ((Button)root.FindViewById(Resource.Id.main_primary_send2)).SetOnClickListener(this);
            ((ImageButton)root.FindViewById(Resource.Id.main_primary_config)).SetOnClickListener(this);

            titleSecondary = (TextView)root.FindViewById(Resource.Id.main_secondary_title);
            ((Button)root.FindViewById(Resource.Id.main_secondary_send1)).SetOnClickListener(this);
            ((Button)root.FindViewById(Resource.Id.main_secondary_send2)).SetOnClickListener(this);
            ((ImageButton)root.FindViewById(Resource.Id.main_secondary_config)).SetOnClickListener(this);

            ((Button)root.FindViewById(Resource.Id.btnA)).SetOnClickListener(this);
        }

        TextView titlePrimary;
        string TitlePrimaryText
        {
            get
            {
                if (titlePrimary != null)
                {
                    return titlePrimary.Text;
                }
                return string.Empty;
            }
        }

        TextView titleSecondary;
        string TitleSecondaryText
        {
            get
            {
                if (titlePrimary != null)
                {
                    return titleSecondary.Text;
                }
                return "";
            }
        }

        public void OnClick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.main_primary_send1:
                    self.SendNotification(MainActivity.NOTI_PRIMARY1, TitlePrimaryText);
                    break;
                case Resource.Id.main_primary_send2:
                    self.SendNotification(MainActivity.NOTI_PRIMARY2, TitlePrimaryText);
                    break;
                case Resource.Id.main_primary_config:
                    self.GoToNotificationSettings(NotificationHelper.PRIMARY_CHANNEL);
                    break;

                case Resource.Id.main_secondary_send1:
                    self.SendNotification(MainActivity.NOTI_SECONDARY1, TitleSecondaryText);
                    break;
                case Resource.Id.main_secondary_send2:
                    self.SendNotification(MainActivity.NOTI_SECONDARY2, TitleSecondaryText);
                    break;
                case Resource.Id.main_secondary_config:
                    self.GoToNotificationSettings(NotificationHelper.SECONDARY_CHANNEL);
                    break;
                case Resource.Id.btnA:
                    self.GoToNotificationSettings();
                    break;
                default:
                    Log.Error(MainActivity.TAG, "Unknown click event.");
                    break;
            }
        }
    }
}
