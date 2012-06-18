using System;
using System.Collections.Generic;
using System.Text;
using Android.Views;
using Java.Lang;
using Android.Graphics.Drawables;

namespace MonoIO.Utilities
{
	class SimpleMenuItem : Java.Lang.Object, IMenuItem
	{
		private SimpleMenu mMenu;
		private int mId;
		private int mOrder;
		private ICharSequence mTitle;
		private ICharSequence mTitleCondensed;
		private Drawable mIconDrawable;
		private int mIconResId = 0;
		private bool mEnabled = true;
		
		public SimpleMenuItem (SimpleMenu menu, int id, int order, ICharSequence title)
		{
			mMenu = menu;
			mId = id;
			mOrder = order;
			mTitle = title;
		}
		
		public int ItemId {
			get {
				return mId;
			}
		}
		
		public int Order {
			get {
				return mOrder;
			}
		}
		
		public IMenuItem SetTitle (ICharSequence title)
		{
			mTitle = title;
			return this;
		}
		
		public IMenuItem SetTitle (int title)
		{
			return SetTitle (new Java.Lang.String(mMenu.Context.GetString (title)));
		}
		
		
		public ICharSequence TitleFormatted {
			get {
				return mTitle;
			}
		}
		
		public IMenuItem SetTitleCondensed (ICharSequence title)
		{
			mTitleCondensed = title;
			return this;
		}
		
		public ICharSequence TitleCondensedFormatted {
			get {
				return mTitleCondensed != null ? mTitleCondensed : mTitle;
			}
		}

		public IMenuItem SetIcon (Android.Graphics.Drawables.Drawable icon)
		{
			mIconResId = 0;
			mIconDrawable = icon;
			return this;
		}

		public IMenuItem SetIcon (int iconResId)
		{
			mIconDrawable = null;
			mIconResId = iconResId;
			return this;
		}
		
		public Android.Graphics.Drawables.Drawable Icon {
			get {
				if (mIconDrawable != null) {
					return mIconDrawable;
				}

				if (mIconResId != 0) {
					return mMenu.Resources.GetDrawable (mIconResId);
				}

				return null;
			}
		}
		
		public IMenuItem SetEnabled (bool enabled)
		{
			mEnabled = enabled;
			return this;
		}
		
		public bool IsEnabled {
			get {
				return mEnabled;
			}
		}
		
		#region IMenuItem implementation
		public bool CollapseActionView ()
		{
			throw new System.NotImplementedException ();
		}

		public bool ExpandActionView ()
		{
			throw new System.NotImplementedException ();
		}

		public IMenuItem SetActionProvider (ActionProvider actionProvider)
		{
			throw new System.NotImplementedException ();
		}

		public IMenuItem SetActionView (View view)
		{
			return this;
		}

		public IMenuItem SetActionView (int resId)
		{
			return this;
		}

		public IMenuItem SetAlphabeticShortcut (char alphaChar)
		{
			return this;
		}

		public IMenuItem SetCheckable (bool checkable)
		{
			return this;
		}

		public IMenuItem SetChecked (bool checkedd)
		{
			return this;
		}

		public IMenuItem SetIntent (Android.Content.Intent intent)
		{
			return this;
		}

		public IMenuItem SetNumericShortcut (char numericChar)
		{
			return this;
		}

		public IMenuItem SetOnActionExpandListener (IMenuItemOnActionExpandListener listener)
		{
			throw new System.NotImplementedException ();
		}

		public IMenuItem SetOnMenuItemClickListener (IMenuItemOnMenuItemClickListener menuItemClickListener)
		{
			return this;
		}

		public IMenuItem SetShortcut (char numericChar, char alphaChar)
		{
			return this;
		}

		public void SetShowAsAction (ShowAsAction actionEnum)
		{
			
		}

		public IMenuItem SetShowAsActionFlags (ShowAsAction actionEnum)
		{
			throw new System.NotImplementedException ();
		}

		public IMenuItem SetVisible (bool visible)
		{
			return this;
		}

		public ActionProvider ActionProvider {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public View ActionView {
			get {
				return null;
			}
		}

		public char AlphabeticShortcut {
			get {
				return '0';
			}
		}

		public int GroupId {
			get {
				return 0;
			}
		}

		public bool HasSubMenu {
			get {
				return false;
			}
		}


		public Android.Content.Intent Intent {
			get {
				return null;
			}
		}

		public bool IsActionViewExpanded {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public bool IsCheckable {
			get {
				return false;
			}
		}

		public bool IsChecked {
			get {
				return false;
			}
		}


		public bool IsVisible {
			get {
				return true;
			}
		}

		public IContextMenuContextMenuInfo MenuInfo {
			get {
				return null;
			}
		}

		public char NumericShortcut {
			get {
				return '0';
			}
		}

		public ISubMenu SubMenu {
			get {
				return null;
			}
		}

		#endregion
	}
}
