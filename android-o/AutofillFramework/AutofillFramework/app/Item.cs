using System;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AutofillFramework.app
{
	public class Item
	{
		public Line Line { get; set; }
		public int Id { get; set; }
		public bool Editable { get; set; }
		public bool Sanitized { get; set; }
		public string[] Hints { get; set; }
		public AutofillType Type { get; set; }
		public string Text { get; set; }
		public bool Focused { get; set; }

		public Item(Line line, int id, string[] hints, AutofillType type, string text, bool editable,
			bool sanitized)
		{
			Line = line;
			Id = id;
			Text = text;
			Editable = editable;
			Sanitized = sanitized;
			Hints = hints;
			Type = type;
		}

		public override string ToString()
		{
			return Id + ": " + Text + (Editable ? " (editable)" : " (read-only)"
				+ (Sanitized ? " (sanitized)" : " (sensitive"));
		}

		public string getClassName() => Editable ? typeof(EditText).Name : typeof(TextView).Name;
	}
}
