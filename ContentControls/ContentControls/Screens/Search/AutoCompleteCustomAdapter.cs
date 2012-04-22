using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace ContentControls {
	/// <summary>
	/// Adapter with custom IFilterable implementation to react
	/// to changing AutoComplete input, with custom matching algorithm
	/// </summary>
	public class AutoCompleteCustomAdapter : ArrayAdapter, IFilterable {
		LayoutInflater inflater;
		Filter filter;
		Activity context;
		public string[] AllItems;
		public string[] MatchItems;

		public AutoCompleteCustomAdapter (Activity context, int txtViewResourceId, string[] items)
			: base(context, txtViewResourceId, items)
		{
			inflater = context.LayoutInflater;
			filter = new SuggestionsFilter(this);
			AllItems = items;
			MatchItems = items; // the matching results; changes with each keypress
		}
		public override int Count {
			get {
				return MatchItems.Length;
			}
		}
		public override Java.Lang.Object GetItem (int position)
		{
			return MatchItems[position];
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null)
				view = inflater.Inflate(Android.Resource.Layout.SimpleDropDownItem1Line, null);

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = MatchItems[position];

            return view;
		}
	
		public override Filter Filter {
			get {
				return filter;
			}
		}

		class SuggestionsFilter : Filter
		{
			AutoCompleteCustomAdapter a;
			public SuggestionsFilter (AutoCompleteCustomAdapter adapter) : base() {
				a = adapter;
			}
			protected override Filter.FilterResults PerformFiltering (Java.Lang.ICharSequence constraint)
			{
				FilterResults results = new FilterResults();
				if (constraint != null) {
					var searchFor = constraint.ToString ();
Console.WriteLine ("searchFor:" + searchFor);
					var matchList = new List<string>();
					
					// find matches, IndexOf means look for the input anywhere in the items
					// but it isn't case-sensitive by default!
					var matches = from i in a.AllItems
								where i.IndexOf(searchFor) >= 0
								select i;
	
					foreach (var match in matches) {
						matchList.Add (match);
					}
		
					a.MatchItems = matchList.ToArray ();
Console.WriteLine ("resultCount:" + matchList.Count);

// not sure if the Java array/FilterResults are used
Java.Lang.Object[] matchObjects;
matchObjects = new Java.Lang.Object[matchList.Count];
for (int i = 0; i < matchList.Count; i++) {
	matchObjects[i] = new Java.Lang.String(matchList[i]);
}

					results.Values = matchObjects;
					results.Count = matchList.Count;
				}
				return results;
			}
			protected override void PublishResults (Java.Lang.ICharSequence constraint, Filter.FilterResults results)
			{
				a.NotifyDataSetChanged();
			}
		}
	}
}