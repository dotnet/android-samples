/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if __ANDROID_11__

using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Provider;

namespace MonoDroid.ApiDemo.App
{
	// Example of how to use an AlertDialog
	[Activity (Label = "App/Alert Dialog Samples")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class AlertDialogSamples : Activity
	{
		private const int DIALOG_YES_NO_MESSAGE = 1;
		private const int DIALOG_YES_NO_LONG_MESSAGE = 2;
		private const int DIALOG_LIST = 3;
		private const int DIALOG_PROGRESS = 4;
		private const int DIALOG_SINGLE_CHOICE = 5;
		private const int DIALOG_MULTIPLE_CHOICE = 6;
		private const int DIALOG_TEXT_ENTRY = 7;
		private const int DIALOG_MULTIPLE_CHOICE_CURSOR = 8;
		private const int DIALOG_YES_NO_ULTRA_LONG_MESSAGE = 9;
		private const int DIALOG_YES_NO_OLD_SCHOOL_MESSAGE = 10;
		private const int DIALOG_YES_NO_HOLO_LIGHT_MESSAGE = 11;
		
		private const int MAX_PROGRESS = 100;

		private ProgressDialog progress_dialog;
		private int progress;
		private Handler progress_handler;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.AlertDialog);

			// Display a text message with yes/no buttons and handle
			// each message as well as the cancel action
			var two_buttons = FindViewById<Button> (Resource.Id.two_buttons);
			two_buttons.Click += delegate { ShowDialog (DIALOG_YES_NO_MESSAGE); };

			// Display a long text message with yes/no buttons and handle
			// each message as well as the cancel action
			var two_buttons2 = FindViewById<Button> (Resource.Id.two_buttons2);
			two_buttons2.Click += delegate { ShowDialog (DIALOG_YES_NO_LONG_MESSAGE); };

			// Display an ultra long text message with yes/no buttons and handle
			// each message as well as the cancel action
			var two_buttons3 = FindViewById<Button> (Resource.Id.two_buttons2ultra);
			two_buttons3.Click += delegate { ShowDialog (DIALOG_YES_NO_ULTRA_LONG_MESSAGE); };

			// Display a list of items
			var select_button = FindViewById<Button> (Resource.Id.select_button);
			select_button.Click += delegate { ShowDialog (DIALOG_LIST); };

			// Display a custom progress bar
			var progress_button = FindViewById<Button> (Resource.Id.progress_button);
			progress_button.Click += delegate {
				ShowDialog (DIALOG_PROGRESS);
				progress = 0;
				progress_dialog.Progress = 0;
				progress_handler.SendEmptyMessage (0);
			};

			// Display a radio button group
			var radio_box = FindViewById<Button> (Resource.Id.radio_button);
			radio_box.Click += delegate { ShowDialog (DIALOG_SINGLE_CHOICE); };

			// Display a list of checkboxes
			var check_box = FindViewById<Button> (Resource.Id.checkbox_button);
			check_box.Click += delegate { ShowDialog (DIALOG_MULTIPLE_CHOICE); };

			// Display a list of checkboxes, backed by a cursor
			var check_box2 = FindViewById<Button> (Resource.Id.checkbox_button2);
			check_box2.Click += delegate { ShowDialog (DIALOG_MULTIPLE_CHOICE_CURSOR); };

			// Display a text entry dialog
			var text_entry = FindViewById<Button> (Resource.Id.text_entry_button);
			text_entry.Click += delegate { ShowDialog (DIALOG_TEXT_ENTRY); };

			// Two points, in the traditional theme
			var two_buttons_old_school = FindViewById<Button> (Resource.Id.two_buttons_old_school);
			two_buttons_old_school.Click += delegate { ShowDialog (DIALOG_YES_NO_OLD_SCHOOL_MESSAGE); };

			// Two points, in the light holographic theme
			var two_buttons_holo = FindViewById<Button> (Resource.Id.two_buttons_holo_light);
			two_buttons_holo.Click += delegate { ShowDialog (DIALOG_YES_NO_HOLO_LIGHT_MESSAGE); };

			progress_handler = new ProgressHandler (this);
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
				case DIALOG_YES_NO_MESSAGE: {
						var builder = new AlertDialog.Builder (this);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_two_buttons_title);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
				case DIALOG_YES_NO_OLD_SCHOOL_MESSAGE: {
						var builder = new AlertDialog.Builder (this, Android.App.AlertDialog.ThemeTraditional);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_two_buttons_title);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
				case DIALOG_YES_NO_HOLO_LIGHT_MESSAGE: {
						var builder = new AlertDialog.Builder (this, Android.App.AlertDialog.ThemeHoloLight);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_two_buttons_title);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
				case DIALOG_YES_NO_LONG_MESSAGE: {
						var builder = new AlertDialog.Builder (this);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_two_buttons_msg);
						builder.SetMessage (Resource.String.alert_dialog_two_buttons_msg);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);
						builder.SetNeutralButton (Resource.String.alert_dialog_something, NeutralClicked);

						return builder.Create ();
					}
				case DIALOG_YES_NO_ULTRA_LONG_MESSAGE: {
						var builder = new AlertDialog.Builder (this);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_two_buttons_msg);
						builder.SetMessage (Resource.String.alert_dialog_two_buttons2ultra_msg);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);
						builder.SetNeutralButton (Resource.String.alert_dialog_something, NeutralClicked);

						return builder.Create ();
					}
				case DIALOG_LIST: {
						var builder = new AlertDialog.Builder (this);
						builder.SetTitle (Resource.String.select_dialog);
						builder.SetItems (Resource.Array.select_dialog_items, ListClicked);

						return builder.Create ();
					}
				case DIALOG_PROGRESS: {
						progress_dialog = new ProgressDialog (this);
						progress_dialog.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						progress_dialog.SetTitle (Resource.String.select_dialog);
						progress_dialog.SetProgressStyle (ProgressDialogStyle.Horizontal);
						progress_dialog.Max = MAX_PROGRESS;

						progress_dialog.SetButton (-1, GetText (Resource.String.alert_dialog_ok), OkClicked);
						progress_dialog.SetButton (-2, GetText (Resource.String.alert_dialog_cancel), CancelClicked);

						return progress_dialog;
					}
				case DIALOG_SINGLE_CHOICE: {
						var builder = new AlertDialog.Builder (this);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_single_choice);
						builder.SetSingleChoiceItems (Resource.Array.select_dialog_items2, 0, ListClicked);

						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
				case DIALOG_MULTIPLE_CHOICE: {
						var builder = new AlertDialog.Builder (this);
						builder.SetIcon (Resource.Drawable.ic_popup_reminder);
						builder.SetTitle (Resource.String.alert_dialog_multi_choice);
						builder.SetMultiChoiceItems (Resource.Array.select_dialog_items3, new bool[] { false, true, false, true, false, false, false }, MultiListClicked);

						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
				case DIALOG_MULTIPLE_CHOICE_CURSOR: {
						var projection = new string[] { BaseColumns.Id, Contacts.PeopleColumns.DisplayName, Contacts.PeopleColumns.SendToVoicemail };
						var cursor = ManagedQuery (ContactsContract.Contacts.ContentUri, projection, null, null, null);

						var builder = new AlertDialog.Builder (this);
						builder.SetIcon (Resource.Drawable.ic_popup_reminder);
						builder.SetTitle (Resource.String.alert_dialog_multi_choice_cursor);
						builder.SetMultiChoiceItems (cursor, Contacts.PeopleColumns.SendToVoicemail, Contacts.PeopleColumns.DisplayName, MultiListClicked);

						return builder.Create ();
					}
				case DIALOG_TEXT_ENTRY: {
						// This example shows how to add a custom layout to an AlertDialog
						var factory = LayoutInflater.From (this);
						var text_entry_view = factory.Inflate (Resource.Layout.alert_dialog_text_entry, null);

						var builder = new AlertDialog.Builder (this);
						builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
						builder.SetTitle (Resource.String.alert_dialog_text_entry);
						builder.SetView (text_entry_view);
						builder.SetPositiveButton (Resource.String.alert_dialog_ok, OkClicked);
						builder.SetNegativeButton (Resource.String.alert_dialog_cancel, CancelClicked);

						return builder.Create ();
					}
			}
			return null;
		}

		private void OkClicked (object sender, DialogClickEventArgs e)
		{
		}

		private void CancelClicked (object sender, DialogClickEventArgs e)
		{
		}

		private void NeutralClicked (object sender, DialogClickEventArgs e)
		{
		}

		private void ListClicked (object sender, DialogClickEventArgs e)
		{
			var items = Resources.GetStringArray (Resource.Array.select_dialog_items);

			var builder = new AlertDialog.Builder (this);
			builder.SetMessage (string.Format ("You selected: {0} , {1}", (int)e.Which, items[(int)e.Which]));

			builder.Show ();
		}

		private void MultiListClicked (object sender, DialogMultiChoiceClickEventArgs e)
		{
			var builder = new AlertDialog.Builder (this);
			builder.SetMessage (string.Format ("You selected: {0}", (int)e.Which));

			builder.Show ();
		}

		private class ProgressHandler : Handler
		{
			private static int MAX_PROGRESS = 100;

			private AlertDialogSamples samples;

			public ProgressHandler (AlertDialogSamples samples)
			{
				this.samples = samples;
			}

			public override void HandleMessage (Message msg)
			{
				base.HandleMessage (msg);

				if (samples.progress > MAX_PROGRESS) {
					samples.progress_dialog.Dismiss ();
				} else {
					samples.progress++;
					samples.progress_dialog.IncrementProgressBy (1);
					samples.progress_handler.SendEmptyMessageDelayed (0, 100);
				}
			}
		}
	}
}
#endif