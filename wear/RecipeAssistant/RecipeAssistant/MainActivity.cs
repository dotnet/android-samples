using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace RecipeAssistant
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		const string Tag = "RecipleAssistant";
		RecipeListAdapter adapter;

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			if (Log.IsLoggable (Tag, LogPriority.Debug)) {
				Log.Debug (Tag, "OnListItemClick " + position);
			}
			String itemName = adapter.GetItemName (position);
			Intent intent = new Intent (ApplicationContext, typeof(RecipeActivity));
			intent.PutExtra (Constants.RecipeNameToLoad, itemName);
			StartActivity (intent);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Android.Resource.Layout.ListContent);

			adapter = new RecipeListAdapter (this);
			ListAdapter = adapter;
		}
	}
}


