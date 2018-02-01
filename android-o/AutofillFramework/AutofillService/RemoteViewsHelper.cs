using Android.Support.Annotation;
using Android.Widget;

namespace AutofillService
{
	/**
	 * This is a class containing helper methods for building Autofill Datasets and Responses.
	 */
	public class RemoteViewsHelper
	{
		private RemoteViewsHelper()
		{
		}
		public static RemoteViews ViewsWithAuth(string packageName, string text)
		{
			return SimpleRemoteViews(packageName, text, Resource.Drawable.ic_lock_black_24dp);
		}

		public static RemoteViews ViewsWithNoAuth(string packageName, string text)
		{
			return SimpleRemoteViews(packageName, text, Resource.Drawable.ic_person_black_24dp);
		}

		private static RemoteViews SimpleRemoteViews(string packageName, string remoteViewsText, [DrawableRes] int drawableId)
		{
			RemoteViews presentation = new RemoteViews(packageName,
				Resource.Layout.multidataset_service_list_item);
			presentation.SetTextViewText(Resource.Id.text, remoteViewsText);
			presentation.SetImageViewResource(Resource.Id.icon, drawableId);
			return presentation;
		}
	}
}