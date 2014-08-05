using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Org.Json;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace RecipeAssistant
{
	public class RecipeListAdapter : Java.Lang.Object, IListAdapter
	{
		private const string Tag = "RecipleListAdapter";

		public class Item : Java.Lang.Object
		{
			public string Title, Name, Summary;
			public Bitmap Image;
		}

		List<Item> items = new List<Item>();
		Context context;
		DataSetObserver observer;

		public RecipeListAdapter (Context context)
		{
			this.context = context;
			LoadRecipleList ();
		}

		void LoadRecipleList() {
			JSONObject jsonObject = AssetUtils.LoadJSONAsset (context, Constants.RecipeListFile);
			if (jsonObject != null) {
				List<Item> items = ParseJson (jsonObject);
				AppendItemsToList (items);
			}
		}

		List<Item> ParseJson(JSONObject json)
		{
			List<Item> result = new List<Item> ();
			try 
			{
				JSONArray items = json.GetJSONArray(Constants.RecipeFieldList);
				for (int i = 0; i < items.Length(); i ++)
				{
					JSONObject item = items.GetJSONObject(i);
					Item parsed = new Item();
					parsed.Name = item.GetString(Constants.RecipeFieldName);
					parsed.Title = item.GetString(Constants.RecipeFieldTitle);
					if (item.Has(Constants.RecipeFieldImage)) {
						String imageFile = item.GetString(Constants.RecipeFieldImage);
						parsed.Image = AssetUtils.LoadBitmapAsset(context, imageFile);
					}
					parsed.Summary = item.GetString(Constants.RecipeFieldSummary);
					result.Add(parsed);
				}
			} catch (Exception ex) {
				Log.Error (Tag, "Failed to parse recipe list: " + ex);
			}
			return result;
		}

		public void AppendItemsToList(List<Item> items)
		{
			this.items.AddRange (items);
			if (observer != null) {
				observer.OnChanged ();
			}
		}

		public bool AreAllItemsEnabled ()
		{
			return true;
		}

		public bool IsEnabled (int position)
		{
			return true;
		}

		public Java.Lang.Object GetItem (int position)
		{
			return items [position];
		}

		public long GetItemId (int position)
		{
			return 0;
		}

		public int GetItemViewType (int position)
		{
			return 0;
		}

		public Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var view = convertView;
			if (view == null) {
				var inf = LayoutInflater.From (context);
				view = inf.Inflate (Resource.Layout.list_item, null);
			}
			var item = (Item)GetItem (position);
			var titleView = (TextView)view.FindViewById (Resource.Id.textTitle);
			var summaryView = (TextView)view.FindViewById (Resource.Id.textSummary);
			var iv = (ImageView)view.FindViewById (Resource.Id.imageView);

			titleView.Text = item.Title;
			summaryView.Text = item.Summary;
			if (item.Image != null) {
				iv.SetImageBitmap (item.Image);
			} else {
				iv.SetImageDrawable (context.Resources.GetDrawable (Resource.Drawable.ic_noimage));
			}
			return view;
		}

		public void RegisterDataSetObserver (DataSetObserver observer)
		{
			observer = observer;
		}

		public void UnregisterDataSetObserver (DataSetObserver observer)
		{
			observer = null;
		}

		public int Count {
			get {
				return items.Count;
			}
		}

		public bool HasStableIds {
			get {
				return false;
			}
		}

		public bool IsEmpty {
			get {
				return items.Count == 0;
			}
		}

		public int ViewTypeCount {
			get {
				return 1;
			}
		}

		public String GetItemName(int position) {
			return items [position].Name;
		}
	}
}

