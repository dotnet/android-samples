using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Mono.Samples.SkeletonApp
{
	public class SkeletonActivity : Activity
	{
		EditText mEditor;

		public SkeletonActivity (IntPtr handle) : base (handle)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (R.layout.skeleton_activity);

			// Find the text editor view inside the layout, because we
			// want to do various programmatic things with it.
			mEditor = (EditText)FindViewById (R.id.editor);

			// Hook up button presses to the appropriate event handler.
			FindViewById (R.id.back).Click += delegate { Finish (); };
			FindViewById (R.id.clear).Click += delegate { mEditor.Text = String.Empty; };

			mEditor.Text = GetText (R.@string.main_label);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			// We are going to create two menus. Note that we assign them
			// unique integer IDs, labels from our string resources, and
			// given them shortcuts.

			menu.Add (0, (int)MenuItems.Back, 0, R.@string.back).SetShortcut ('0', 'b');
			menu.Add (0, (int)MenuItems.Clear, 0, R.@string.clear).SetShortcut ('1', 'c');

			return true;
		}

		// Called right before your activity's option menu is displayed.
		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);

			// Before showing the menu, we need to decide whether the clear
			// item is enabled depending on whether there is text to clear.
			menu.FindItem ((int)MenuItems.Clear).SetVisible (((string)mEditor.Text).Length > 0);

			return true;
		}

		// Called when a menu item is selected.
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch ((MenuItems)item.ItemId) {
				case MenuItems.Back:
					Finish ();
					return true;
				case MenuItems.Clear:
					mEditor.Text = String.Empty;
					return true;
			}
			
			return base.OnOptionsItemSelected (item);
		}
	}
}
