
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V7.Widget;

namespace RecyclerViewSample
{
	public class RecyclerViewFragment : Android.Support.V4.App.Fragment
	{
		private const string TAG = "RecycleViewFragment";
		public RecyclerView recyclerView;
		public RecyclerView.Adapter adapter;
		public RecyclerView.LayoutManager layoutManager;
		public string[] dataSet;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			InitDataSet ();
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate (Resource.Layout.recycler_view_frag, container, false);
			rootView.SetTag (rootView.Id,TAG);
			recyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.recyclerView);

			// A LinearLayoutManager is used here, this will layout the elements in a similar fashion
			// to the way ListView would layout elements. The RecyclerView.LayoutManager defines how the
			// elements are laid out.
			layoutManager = new LinearLayoutManager (Activity);
			recyclerView.SetLayoutManager (layoutManager);


			adapter = new CustomAdapter (dataSet);
			// Set CustomAdapter as the adapter for RecycleView
			recyclerView.SetAdapter (adapter);
			return rootView;
		}
		public void InitDataSet()
		{
			dataSet = new string[60];
			for (int i = 0; i < 60; i++) {
				dataSet [i] = "This is element #" + i;
			}
		}
	}
}

