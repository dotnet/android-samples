
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace AndroidLSamples
{
	[Activity (Label = "RecyclerView with Animations", ParentActivity=typeof(HomeActivity))]			
	public class RecyclerViewActivityAddRemove : Activity
	{
		RecyclerView recyclerView;
		RecyclerAdapter adapter;
		RecyclerView.LayoutManager layoutManager;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_recycler_view_add_remove);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			recyclerView = FindViewById<RecyclerView> (Resource.Id.recycler_view);
			recyclerView.HasFixedSize = true;


			layoutManager = new LinearLayoutManager (this);
			recyclerView.SetLayoutManager (layoutManager);

			adapter = new RecyclerAdapter (new List<RecyclerItem>());
			recyclerView.SetAdapter (adapter);
			recyclerView.SetItemAnimator (new DefaultItemAnimator ());

			FindViewById<Button>(Resource.Id.add).Click += (sender, e) => {
				adapter.Add(new RecyclerItem { Title = "New Item: " + adapter.ItemCount });
			};

			adapter.OnItemClick = (view, item) => {
				adapter.Remove(adapter.Items.IndexOf(item));
			};
		}

	}

}

