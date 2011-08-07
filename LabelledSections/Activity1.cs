using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Mono.Samples.LabelledSections
{
	[Activity (Label = "Labelled Sections", MainLauncher = true)]
	public class ListWithHeaders: ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var data = new ListItemCollection<ListItemValue> () {
				new ListItemValue ("Babbage"),
				new ListItemValue ("Boole"),
				new ListItemValue ("Berners-Lee"),
				new ListItemValue ("Atanasoff"),
				new ListItemValue ("Allen"),
				new ListItemValue ("Cormack"),
				new ListItemValue ("Cray"),
				new ListItemValue ("Dijkstra"),
				new ListItemValue ("Dix"),
				new ListItemValue ("Dewey"),
				new ListItemValue ("Erdos"),
			};
			
			var sortedContacts = data.GetSortedData ();
			var adapter = CreateAdapter (sortedContacts);
			ListAdapter = adapter;
			SetContentView (Resource.Layout.Main);
		}
		
		SeparatedListAdapter CreateAdapter<T> (Dictionary<string, List<T>> sortedObjects)
			where T : IHasLabel, IComparable<T>
		{
			var adapter = new SeparatedListAdapter (this);
			foreach (var e in sortedObjects.OrderBy (de => de.Key)) {
				var label   = e.Key;
				var section = e.Value;
				adapter.AddSection (label, new ArrayAdapter<T> (this, Resource.Layout.ListItem, section));
			}
			return adapter;
		}
	}
}


