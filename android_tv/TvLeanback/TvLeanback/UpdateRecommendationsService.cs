using System;

using Android.App;
using Android.Content;
using Android.Util;

using System.Collections.Generic;


namespace TvLeanback
{
	[Service (Label = "UpdateRecommendationsService", Enabled = true)]
	public class UpdateRecommendationsService : IntentService
	{
		private static readonly String TAG = "UpdateRecommendationsService";
		private static readonly int MAX_RECOMMENDATIONS = 3;

		public UpdateRecommendationsService () : base("RecommendationService"){}

		protected override void OnHandleIntent(Intent intent) {
			Log.Debug(TAG, "Updating recommendation cards");
			var recommendations = VideoProvider.MovieList;

			int count = 0;

			try {
				var builder = new RecommendationBuilder()
					.SetContext(Application.ApplicationContext)
					.SetSmallIcon(Resource.Drawable.videos_by_google_icon);
				foreach(var entry in recommendations){
					foreach(var movie in entry.Value){
						Log.Debug(TAG, "Recommendation - " + movie.Title);
						builder.SetBackground(movie.CardImageUrl)
							.SetId(count + 1)
							.SetPriority(MAX_RECOMMENDATIONS - count)
							.SetTitle(movie.Title)
							.SetDescription(GetString(Resource.String.popular_header))
							.SetImage(movie.CardImageUrl)//was movie.GetCardImageURI()
							.SetIntent(BuildPendingIntent(movie))
							.Build();

						if (++count >= MAX_RECOMMENDATIONS) {
							break;
						}
					}
					if (++count >= MAX_RECOMMENDATIONS) {
						break;
					}
				}
			} catch (Java.IO.IOException e) {
				Log.Error(TAG, "Unable to update recommendation", e);
			}
		}

		private PendingIntent BuildPendingIntent(Movie movie) {
			var detailsIntent = new Intent(this, typeof( DetailsActivity));
			detailsIntent.PutExtra(GetString(Resource.String.movie), Utils.Serialize(movie));

			var stackBuilder = TaskStackBuilder.Create(this);
			stackBuilder.AddParentStack(Utils.ToJavaClass(typeof(DetailsActivity)));
			stackBuilder.AddNextIntent(detailsIntent);
			// Ensure a unique PendingIntents, otherwise all recommendations end up with the same
			// PendingIntent
			detailsIntent.SetAction(Java.Lang.Long.ToString(movie.Id));

			var intent = stackBuilder.GetPendingIntent (0, PendingIntentFlags.UpdateCurrent);
			return intent;
		}

	}
}

