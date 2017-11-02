using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Java.Util;

namespace AutofillFramework.app
{
	public class Line
	{
		static string Tag = "Line";

		/// <summary>
		/// Boundaries of the text field, relative to the CustomView
		/// </summary>
		/// <value>The bounds.</value>
		public Rect Bounds { get; set; }
		public Item LabelItem;
		public Item FieldTextItem;
		public string IdEntry { get; }
		public CustomVirtualView CustomVirtualView { get; set; }
		public AutofillManager AutofillManager { get; set; }

		public Line(string idEntry, string label, string[] hints, string text, bool sanitized)
		{
			IdEntry = idEntry;
			Bounds = new Rect();
			LabelItem = new Item(this, ++CustomVirtualView.sNextId, null, AutofillType.None, label,
				false, true);
			FieldTextItem = new Item(this, ++CustomVirtualView.sNextId, hints, AutofillType.Text, text,
					true, sanitized);
		}

		public void ChangeFocus(bool focused)
		{
			FieldTextItem.Focused = focused;
			if (focused)
			{
				Rect absBounds = GetAbsCoordinates();
				Log.Debug(Tag, "focus gained on " + FieldTextItem.Id + "; absBounds=" + absBounds);
				AutofillManager.NotifyViewEntered(CustomVirtualView, FieldTextItem.Id, absBounds);
			}
			else
			{
				Log.Debug(Tag, "focus lost on " + FieldTextItem.Id);
				AutofillManager.NotifyViewExited(CustomVirtualView, FieldTextItem.Id);
			}
		}

		Rect GetAbsCoordinates()
		{
			// Must offset the boundaries so they're relative to the CustomView.
			int[] offset = new int[2];
			CustomVirtualView.GetLocationOnScreen(offset);
			Rect absBounds = new Rect(Bounds.Left + offset[0],
					Bounds.Top + offset[1],
					Bounds.Right + offset[0], Bounds.Bottom + offset[1]);
			Log.Verbose(Tag, "getAbsCoordinates() for " + FieldTextItem.Id + ": bounds=" + Bounds
					+ " offset: " + Arrays.ToString(offset) + " absBounds: " + absBounds);
			return absBounds;
		}

		public void Reset()
		{
			FieldTextItem.Text = "        ";
		}

		public override string ToString()
		{
			return "Label: " + LabelItem + " Text: " + FieldTextItem + " Focused: " +
				FieldTextItem.Focused;
		}
	}
}
