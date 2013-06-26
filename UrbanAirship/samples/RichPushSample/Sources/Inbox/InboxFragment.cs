/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.UrbanAirship.RichPush;
using Java.Lang;
using System.Collections.Generic;
using Xamarin.ActionbarSherlockBinding.App;
using ActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
	 * A list fragment that shows rich push messages.
	 *
	 */
	public abstract class InboxFragment : SherlockListFragment
	{
		public const string EMPTY_COLUMN_NAME = "";
		public const string ROW_LAYOUT_ID_KEY = "row_layout_id";
		public const string EMPTY_LIST_STRING_KEY = "empty_list_string";
		private IOnMessageListener listener;
		private RichPushMessageAdapter adapter;
		private IList<string> selectedMessageIds = new List<string> ();
		private IList<RichPushMessage> messages;

		override
				public void OnAttach (Activity activity)
		{
			base.OnAttach (activity);
			this.SetActivityAsListener (activity);
		}

		override
				public void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set the RichPushMessageAdapter
			this.adapter = new RichPushMessageAdapter (Activity, RowLayoutId);
			adapter.SetViewBinder (CreateMessageBinder ());
			this.ListAdapter = (adapter);

			// Retain the instance so we keep list position and selection on activity re-creation
			RetainInstance = true;
		}

		override
				public void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);
			this.SetEmptyText (GetString (EmptyListStringId));
		}

		override
				public void OnListItemClick (ListView list, View view, int position, long id)
		{
			this.listener.OnMessageOpen (this.adapter.GetItem (position));
		}

		/**
	     * Sets the rich push messages to display
	     * @param messages Current list of rich push messages
	     */
		public void SetMessages (IList<RichPushMessage> messages)
		{
			this.messages = messages;
			adapter.SetMessages (messages);
		}

		/**
	     * @return The list of ids of the selected messages
	     */
		public IList<string> SelectedMessages {
			get {
				return selectedMessageIds;
			}
		}

		/**
	     * Clears the selected messages
	     */
		public void ClearSelection ()
		{
			selectedMessageIds.Clear ();
			adapter.NotifyDataSetChanged ();
			listener.OnSelectionChanged ();
		}

		/**
	     * Selects all the messages in the inbox
	     */
		public void SelectAll ()
		{
			selectedMessageIds.Clear ();
			foreach (RichPushMessage message in messages) {
				selectedMessageIds.Add (message.MessageId);
			}
			adapter.NotifyDataSetChanged ();
			listener.OnSelectionChanged ();
		}

		/**
	     * @return The layout id to use in the RichPushMessageAdapter
	     */
		public abstract int RowLayoutId { get; }

		/**
	     * @return The string id of the message to display when no messages are available
	     */
		public abstract int EmptyListStringId { get; }

		/**
	     * Tries to set the activity as an OnMessageListener
	     * @param activity The specified activity
	     */
		private void SetActivityAsListener (Activity activity)
		{
			try {
				this.listener = (IOnMessageListener)activity;
			} catch (ClassCastException) {
				throw new IllegalStateException ("Activities using InboxFragment must implement " +
					"the InboxFragment.OnMessageListener interface.");
			}
		}

		/**
	     * Listens for message selection and selection changes
	     *
	     */
		public interface IOnMessageListener
		{
			void OnMessageOpen (RichPushMessage message);

			void OnSelectionChanged ();
		}

		/**
	     * Sets a message is selected or not
	     * @param messageId The id of the message
	     * @param isChecked Boolean indicating if the message is selected or not
	     */
		protected void OnMessageSelected (string messageId, bool isChecked)
		{
			if (isChecked && !selectedMessageIds.Contains (messageId)) {
				selectedMessageIds.Add (messageId);
			} else if (!isChecked && selectedMessageIds.Contains (messageId)) {
				selectedMessageIds.Remove (messageId);
			}

			listener.OnSelectionChanged ();
		}

		/**
	     * Returns if a message is selected
	     * @param messageId The id of the message
	     * @return <code>true</code> If the message is selected, <code>false</code> otherwise.
	     */
		protected bool IsMessageSelected (string messageId)
		{
			return selectedMessageIds.Contains (messageId);
		}

		/**
	     * @return RichPushMessageAdapter.ViewBinder to bind messages to a list view item
	     * in the list adapter.
	     */
		protected abstract RichPushMessageAdapter.IViewBinder CreateMessageBinder ();
	}
}
