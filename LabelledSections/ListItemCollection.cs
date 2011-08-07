using System;
using System.Collections;
using System.Collections.Generic;

using Android.Util;

namespace Mono.Samples.LabelledSections
{
	class ListItemCollection<T> : IEnumerable<T>
		where T : IHasLabel, IComparable<T>
	{
		public ListItemCollection ()
		{
		}
		
		List<T> values = new List<T> ();
		
		public void Add (T value)
		{
			values.Add (value);
		}
		
		public IEnumerator<T> GetEnumerator ()
		{
			return values.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		public Dictionary<string, List<T>> GetSortedData ()
		{
			var results = new Dictionary<string, List<T>>();
			List<T> contacts = null;
			string cur = null;
			
			foreach (var e in values) {
				if (e.Label != cur) {
					contacts = new List<T> ();
					cur = e.Label;
					results.Add (cur, contacts);
				}
				contacts.Add (e);
			}

			foreach (var v in results.Values)
				v.Sort ();
			
			return results;
		}
	}
}

