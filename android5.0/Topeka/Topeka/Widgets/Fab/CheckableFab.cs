using System;

using Android.Widget;
using Android.Content;
using Android.Util;

namespace Topeka.Widgets.Fab
{
	public class CheckableFab : FloatingActionButton, ICheckable
	{
		static readonly int[] checkedStates = { Android.Resource.Attribute.StateChecked };

		bool isChecked;

		public void Toggle ()
		{
			Checked = !isChecked;
		}

		public bool Checked {
			get {
				return isChecked;
			}
			set {
				if (isChecked == value)
					return;

				isChecked = value;
				RefreshDrawableState ();
			}
		}

		public CheckableFab (Context context) : this (context, null)
		{
		}

		public CheckableFab (Context context, IAttributeSet attrs) : this (context, attrs, 0)
		{
		}

		public CheckableFab (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			SetImageResource (Resource.Drawable.answer_quiz_fab);
		}

		public override int[] OnCreateDrawableState (int extraSpace)
		{
			var drawableState = base.OnCreateDrawableState (++extraSpace);
			if (Checked)
				MergeDrawableStates (drawableState, checkedStates);
			return drawableState;
		}
	}
}

