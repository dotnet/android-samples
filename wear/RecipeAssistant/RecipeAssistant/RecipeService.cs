using System;
using Android.App;
using Android.Support.V4.App;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using Android.Graphics;

namespace RecipeAssistant
{
	[Service()]
	public class RecipeService : Service
	{
		public static RecipeService Last { get; private set; }

		NotificationManagerCompat notificationManager;
		private LocalBinder binder = new LocalBinder();
		Recipe recipe;

		public class LocalBinder : Binder {
			public RecipeService RService;
			public RecipeService Service {
				get {
					return RService;
				}
			}
		}
		public override void OnCreate ()
		{
			binder.RService = this;
			notificationManager = NotificationManagerCompat.From (this);
			Last = this;
		}

		public override IBinder OnBind (Android.Content.Intent intent)
		{
			return binder;
		}

		public override StartCommandResult OnStartCommand (Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
			if (intent.Action.Equals (Constants.ActionStartCooking)) {
				CreateNotification (intent);
				return StartCommandResult.Sticky;
			}
			return StartCommandResult.NotSticky;
		}

		private void CreateNotification(Intent intent) {
			recipe = Recipe.FromBundle(intent.GetBundleExtra(Constants.ExtraRecipe));
			List<Notification> notificationPages = new List<Notification> ();

			int stepCount = recipe.RecipeSteps.Count;

			for (int i = 0; i < stepCount; i++) {
				Recipe.RecipeStep recipeStep = recipe.RecipeSteps [i];
				var style = new NotificationCompat.BigTextStyle ();
				style.BigText (recipeStep.StepText);
				style.SetBigContentTitle (String.Format (Resources.GetString (Resource.String.step_count), i + 1, stepCount));
				style.SetSummaryText ("");
				var builder = new NotificationCompat.Builder (this);
				builder.SetStyle (style);
				notificationPages.Add (builder.Build ());
			}

			var notifBuilder = new NotificationCompat.Builder(this);

			if (recipe.RecipeImage != null) {
				Bitmap recipeImage = Bitmap.CreateScaledBitmap(
					AssetUtils.LoadBitmapAsset(this, recipe.RecipeImage),
					Constants.NotificationImageWidth, Constants.NotificationImageHeight, false);
				notifBuilder.SetLargeIcon(recipeImage);
			}
			notifBuilder.SetContentTitle (recipe.TitleText);
			notifBuilder.SetContentText (recipe.SummaryText);
			notifBuilder.SetSmallIcon (Resource.Mipmap.ic_notification_recipe);

			Notification notification = notifBuilder.Extend(new NotificationCompat.WearableExtender().AddPages(notificationPages)).Build();
			notificationManager.Notify (Constants.NotificationId, notification);
		}
	}
}

