/*
 * Copyright (C) 2014 The Android Open Source Project
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MessagingService
{
	/// <summary>
	/// The main fragment that shows the buttons and the text view containing the log.
	/// </summary>
	public class MessagingFragment : Fragment, View.IOnClickListener
	{
		static readonly string TAG = typeof(MessagingFragment).Name;

		Button mSendSingleConversation;
		Button mSendTwoConversations;
		Button mSendConversationWithThreeMessages;
		TextView mDataPortView;
		Button mClearLogButton;

		Messenger mService;
		bool mBound;

		class MyServiceConnection : Java.Lang.Object, IServiceConnection
		{
			readonly MessagingFragment myFragment;

			public MyServiceConnection (MessagingFragment myFragment)
			{
				this.myFragment = myFragment;
			}

			public void OnServiceConnected (ComponentName name, IBinder service)
			{
				myFragment.mService = new Messenger (service);
				myFragment.mBound = true;
				myFragment.SetButtonsState (true);
			}

			public void OnServiceDisconnected (ComponentName name)
			{
				myFragment.mService = null;
				myFragment.mBound = false;
				myFragment.SetButtonsState (false);
			}
		}

		readonly MyServiceConnection mConnection;

		class MySharedPreferencesOnSharedPreferenceChangeListener : Java.Lang.Object, 
		ISharedPreferencesOnSharedPreferenceChangeListener
		{
			readonly MessagingFragment myFragment;

			public MySharedPreferencesOnSharedPreferenceChangeListener (MessagingFragment myFragment)
			{
				this.myFragment = myFragment;
			}

			public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
			{
				if (MessageLogger.LOG_KEY == key) {
					myFragment.mDataPortView.Text = MessageLogger.GetAllMessages (myFragment.Activity);
				}
			}
		}

		readonly MySharedPreferencesOnSharedPreferenceChangeListener listener;

		public MessagingFragment ()
		{
			mConnection = new MyServiceConnection (this);
			listener = new MySharedPreferencesOnSharedPreferenceChangeListener (this);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
		                                   Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate (Resource.Layout.fragment_message_me, container, false);

			mSendSingleConversation = (Button)rootView.FindViewById (Resource.Id.send_1_conversation);
			mSendSingleConversation.SetOnClickListener (this);

			mSendTwoConversations = (Button)rootView.FindViewById (Resource.Id.send_2_conversations);
			mSendTwoConversations.SetOnClickListener (this);

			mSendConversationWithThreeMessages =
				(Button)rootView.FindViewById (Resource.Id.send_1_conversation_3_messages);
			mSendConversationWithThreeMessages.SetOnClickListener (this);

			mDataPortView = (TextView)rootView.FindViewById (Resource.Id.data_port);
			mDataPortView.MovementMethod = new ScrollingMovementMethod ();

			mClearLogButton = (Button)rootView.FindViewById (Resource.Id.clear);
			mClearLogButton.SetOnClickListener (this);

			SetButtonsState (false);

			return rootView;
		}

		public void OnClick (View v)
		{
			if (v == mSendSingleConversation) {
				SendMessage (1, 1);
			} else if (v == mSendTwoConversations) {
				SendMessage (2, 1);
			} else if (v == mSendConversationWithThreeMessages) {
				SendMessage (1, 3);
			} else if (v == mClearLogButton) {
				MessageLogger.Clear (Activity);
				mDataPortView.Text = MessageLogger.GetAllMessages (Activity);
			}
		}

		public override void OnStart ()
		{
			base.OnStart ();
			Activity.BindService (new Intent (Activity, typeof(MessagingService)), mConnection, Bind.AutoCreate);
		}

		public override void OnPause ()
		{
			base.OnPause ();
			MessageLogger.GetPrefs (Activity).UnregisterOnSharedPreferenceChangeListener (listener);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			mDataPortView.Text = MessageLogger.GetAllMessages (Activity);
			MessageLogger.GetPrefs (Activity).RegisterOnSharedPreferenceChangeListener (listener);
		}

		public override void OnStop ()
		{
			base.OnStop ();
			if (mBound) {
				Activity.UnbindService (mConnection);
				mBound = false;
			}
		}

		void SendMessage (int howManyConversations, int messagesPerConversation)
		{
			if (mBound) {
				Message msg = Message.Obtain (null, MessagingService.MSG_SEND_NOTIFICATION,
					              howManyConversations, messagesPerConversation);
				try {
					mService.Send (msg);
				} catch (RemoteException e) {
					Log.Error (TAG, "Error sending a message", e);
					MessageLogger.LogMessage (Activity, "Error occurred while sending a message.");
				}
			}
		}

		void SetButtonsState (bool enable)
		{
			mSendSingleConversation.Enabled = enable;
			mSendTwoConversations.Enabled = enable;
			mSendConversationWithThreeMessages.Enabled = enable;
		}
	}
}

