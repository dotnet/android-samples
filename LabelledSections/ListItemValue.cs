using System;
using System.Collections.Generic;

namespace Mono.Samples.LabelledSections
{
	class ListItemValue : Java.Lang.Object, IHasLabel, IComparable<ListItemValue>
	{
		public ListItemValue (string name)
		{
			Name = name;
		}
		
		public string Name {get; private set;}
		
		int IComparable<ListItemValue>.CompareTo (ListItemValue value)
		{
			return Name.CompareTo (value.Name);
		}
		
		public override string ToString ()
		{
			 return Name;
		}
		
		public string Label {
			get {return Name [0].ToString ();}
		}
	}
}

