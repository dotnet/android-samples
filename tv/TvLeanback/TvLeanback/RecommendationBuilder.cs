using System;

using Android.Content;
using Android.App;
using Android.OS;
using Android.Graphics;
using Android.Util;

using Android.Support.V4.App;

using Squareup.Picasso;

namespace TvLeanback
{
	public class RecommendationBuilder
	{
		private static readonly string TAG = "RecommendationBuilder";

		private static int CARD_WIDTH = 200;
		private static int CARD_HEIGHT = 100;

		public static readonly string EXTRA_BACKGROUND_IMAGE_URL = "background_image_url";

		private NotificationManager mNotificationManager;
		private Context mContext;
		private int mId;
		private int mPriority;
		private int mSmallIcon;
		private String mTitle;
		private String mDescription;
		private String mImageUri;
		private String mBackgroundUri;
		private PendingIntent mIntent;

		public RecommendationBuilder ()
		{
		}

		public RecommendationBuilder SetContext (Context context)
		{
			mContext = context;
			return this;
		}

		public RecommendationBuilder SetId (int id)
		{
			mId = id;
			return this;
		}

		public RecommendationBuilder SetPriority (int priority)
		{
			mPriority = priority;
			return this;
		}

		public RecommendationBuilder SetTitle (String title)
		{
			mTitle = title;
			return this;
		}

		public RecommendationBuilder SetDescription (String description)
		{
			mDescription = description;
			return this;
		}

		public RecommendationBuilder SetImage (String uri)
		{
			mImageUri = uri;
			return this;
		}

		public RecommendationBuilder SetBackground (String uri)
		{
			mBackgroundUri = uri;
			return this;
		}

		public RecommendationBuilder SetIntent (PendingIntent intent)
		{
			mIntent = intent;
			return this;
		}

		public RecommendationBuilder SetSmallIcon (int resourceId)
		{
			mSmallIcon = resourceId;
			return this;
		}

		public Notification Build ()
		{
			Log.Debug (TAG, "Building notification - " + this.ToString ());

			if (mNotificationManager == null) {
				mNotificationManager = (NotificationManager)mContext
					.GetSystemService (Context.NotificationService);
			}

			Bundle extras = new Bundle ();
			if (mBackgroundUri != null) {
				extras.PutString (EXTRA_BACKGROUND_IMAGE_URL, mBackgroundUri);
			}
			var image = Picasso.With (mContext)
				.Load (mImageUri)
				.Resize (Utils.dpToPx (CARD_WIDTH, mContext), Utils.dpToPx (CARD_HEIGHT, mContext))
				.Get ();

			var notification = new NotificationCompat.BigPictureStyle (
				                            new NotificationCompat.Builder (mContext)
				.SetContentTitle (mTitle)
				.SetContentText (mDescription)
				.SetPriority (mPriority)
				.SetLocalOnly (true)
				.SetOngoing (true)
				                            //.SetColor(mContext.Resources.GetColor(Resource.Color.fastlane_background)) FIXME no binding
				                            //.SetCategory(Notification.CATEGORY_RECOMMENDATION) Commented out in example
				                            //.SetCategory("recommendation") FIXME no binding
				.SetLargeIcon (image)
				.SetSmallIcon (mSmallIcon)
				.SetContentIntent (mIntent)
				.SetExtras (extras)).Build ();
			mNotificationManager.Notify (mId, notification);
			mNotificationManager = null;
			return notification;

		}

		public override string ToString ()
		{
			return "RecommendationBuilder{" +
			", mId=" + mId +
			", mPriority=" + mPriority +
			", mSmallIcon=" + mSmallIcon +
			", mTitle='" + mTitle + '\'' +
			", mDescription='" + mDescription + '\'' +
			", mImageUri='" + mImageUri + '\'' +
			", mBackgroundUri='" + mBackgroundUri + '\'' +
			", mIntent=" + mIntent +
			'}';
		}
	}
}

