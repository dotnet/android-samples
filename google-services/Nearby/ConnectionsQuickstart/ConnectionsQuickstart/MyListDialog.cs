using System;
using System.Linq;
using Android.App;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;

namespace ConnectionsQuickstart
{
	public class MyListDialog
	{
		readonly AlertDialog mDialog;
		ArrayAdapter<string> mAdapter;
		Dictionary<string, string> mItemMap;

		public MyListDialog (Context context, AlertDialog.Builder builder,
			EventHandler<DialogClickEventArgs> listener)
		{

			mItemMap = new Dictionary<string, string> ();
			mAdapter = new ArrayAdapter<string> (context, Android.Resource.Layout.SelectDialogSingleChoice);

			// Create dialog from builder
			builder.SetAdapter (mAdapter, listener);
			mDialog = builder.Create ();
		}

		public void AddItem (string title, string value)
		{
			mItemMap.Add (title, value);
			mAdapter.Add (title);
		}

		public void RemoveItemByTitle (string title)
		{
			mItemMap.Remove (title);
			mAdapter.Remove (title);
		}

		public void RemoveItemByValue (string value)
		{
			foreach (var key in mItemMap.Keys) {
				if (mItemMap [key] == value) {
					mItemMap.Remove (key);
					mAdapter.Remove (key);
					mAdapter.NotifyDataSetChanged ();
				}
			}
		}

		public string GetItemKey (int index)
		{
			return mAdapter.GetItem (index);
		}

		public string GetItemValue (int index)
		{
			return mItemMap[GetItemKey (index)];
		}

		public void Show ()
		{
			mDialog.Show ();
		}

		public void Dismiss ()
		{
			if (mDialog.IsShowing) {
				mDialog.Dismiss ();
			}
		}
	}
}

