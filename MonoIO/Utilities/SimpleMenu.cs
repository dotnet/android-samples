using System;
using System.Collections.Generic;
using System.Text;
using Android.Views;
using Android.App;
using Java.Lang;
using Android.Content;
using Android.Content.Res;

namespace MonoIO.Utilities
{
	class SimpleMenu : Java.Lang.Object, IMenu
	{

		public Context Context {get; set;}
		public Resources Resources { get; set; }
		private List<SimpleMenuItem> items = new List<SimpleMenuItem> ();

		public SimpleMenu (Context context)
		{
			Context = context;
			Resources = context.Resources;
		}
		
		public IMenuItem Add (Java.Lang.ICharSequence title)
		{
			return AddInternal (0, 0, title);
		}

		public IMenuItem Add (int titleRes)
		{
			return AddInternal (0, 0, new Java.Lang.String(Resources.GetString (titleRes)));
		}
		
		public IMenuItem Add (int groupId, int itemId, int order, Java.Lang.ICharSequence title)
		{
			return AddInternal (itemId, order, title);
		}
		
		public IMenuItem Add (int groupId, int itemId, int order, int titleRes)
		{
			return AddInternal (itemId, order, new Java.Lang.String(Resources.GetString (titleRes)));
		}

		private IMenuItem AddInternal (int itemId, int order, ICharSequence title)
		{
			var item = new SimpleMenuItem (this, itemId, order, title);
			items.Insert (FindInsertIndex (items, order), item);
			return item;
		}

		private static int FindInsertIndex (List<SimpleMenuItem> items, int order)
		{
			for (int i = items.Count - 1; i >= 0; i--) {
				var item = items[i];

				if (item.Order <= order)
					return i + 1;
			}

			return	0;
		}

		public int FindItemIndex(int id) {
	        int size = Size();
	
	        for (int i = 0; i < size; i++) {
	            SimpleMenuItem item = items[i];
	            if (item.ItemId == id) {
	                return i;
	            }
	        }
	
	        return -1;
	    }
		
		public void RemoveItem (int itemId)
		{
			RemoveItemAtInt (FindItemIndex (itemId));
		}
		
		private void RemoveItemAtInt(int index) {
	        if ((index < 0) || (index >= items.Count)) {
	            return;
	        }
	        items.RemoveAt(index);
	    }
		
		public void Clear ()
		{
			items.Clear ();
		}
		
		public IMenuItem FindItem (int id)
		{
			int size = Size();
	        for (int i = 0; i < size; i++) {
	            SimpleMenuItem item = items[i];
	            if (item.ItemId == id) {
	                return item;
	            }
	        }
	
	        return null;
		}
		
		public int Size ()
		{
			return items.Count;
		}
		
		public IMenuItem GetItem (int index)
		{
			return items[index];
		}

		#region IMenu implementation
		
		public int AddIntentOptions (int groupId, int itemId, int order, Android.Content.ComponentName caller, Intent[] specifics, Android.Content.Intent intent, MenuAppendFlags flags, IMenuItem[] outSpecificItems)
		{
			throw new System.NotImplementedException ();
		}

		public ISubMenu AddSubMenu (int titleRes)
		{
			throw new System.NotImplementedException ();
		}

		public ISubMenu AddSubMenu (int groupId, int itemId, int order, int titleRes)
		{
			throw new System.NotImplementedException ();
		}

		public ISubMenu AddSubMenu (int groupId, int itemId, int order, ICharSequence title)
		{
			throw new System.NotImplementedException ();
		}

		public ISubMenu AddSubMenu (ICharSequence title)
		{
			throw new System.NotImplementedException ();
		}

		public void Close ()
		{
			throw new System.NotImplementedException ();
		}

		public bool IsShortcutKey (Keycode keyCode, KeyEvent e)
		{
			throw new System.NotImplementedException ();
		}

		public bool PerformIdentifierAction (int id, MenuPerformFlags flags)
		{
			throw new System.NotImplementedException ();
		}

		public bool PerformShortcut (Keycode keyCode, KeyEvent e, MenuPerformFlags flags)
		{
			throw new System.NotImplementedException ();
		}

		public void RemoveGroup (int groupId)
		{
			throw new System.NotImplementedException ();
		}

		public void SetGroupCheckable (int group, bool checkable, bool exclusive)
		{
			throw new System.NotImplementedException ();
		}

		public void SetGroupEnabled (int group, bool enabled)
		{
			throw new System.NotImplementedException ();
		}

		public void SetGroupVisible (int group, bool visible)
		{
			throw new System.NotImplementedException ();
		}

		public void SetQwertyMode (bool isQwerty)
		{
			throw new System.NotImplementedException ();
		}

		public bool HasVisibleItems {
			get {
				throw new System.NotImplementedException ();
			}
		}
		#endregion
	}
}
