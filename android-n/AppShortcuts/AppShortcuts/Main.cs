/*
 * Copyright (C) 2016 The Android Open Source Project
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

using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Java.Lang;
using System;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Android.Content.PM;
using Context = Android.Content.Context;
using StringBuilder = Java.Lang.StringBuilder;

namespace AppShortcuts
{
	[Activity(Label = "AppShortcuts", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
	[MetaData("android.app.shortcuts", Resource = "@xml/shortcuts")]
	public class Main : ListActivity, View.IOnClickListener
	{
		public static string TAG = "ShortcutSample";

		static string ID_ADD_WEBSITE = "add_website";

		static string ACTION_ADD_WEBSITE = "com.example.android.shortcutsample.ADD_WEBSITE";
		MyAdapter mAdapter;

		ShortcutHelper mHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.ActivityMain);

			mHelper = new ShortcutHelper(this);

			mHelper.MaybeRestoreAllDynamicShortcuts();

			mHelper.RefreshShortcuts(/*force=*/ false);

			if (ACTION_ADD_WEBSITE.Equals(Intent.Action))
			{
				// Invoked via the manifest shortcut.
				AddWebSite();
			}

			mAdapter = new MyAdapter(this);
			ListAdapter = mAdapter;
		}

		protected override void OnResume()
		{
			base.OnResume();
			RefreshList();
		}

		/**
		 * Handle the add button.
		 */
		[Java.Interop.Export("OnAddPressed")]
		public void OnAddPressed(View v)
		{
			AddWebSite();
		}

		void AddWebSite()
		{
			Log.Info(TAG, "addWebSite");

			// This is important.  This allows the launcher to build a prediction model.
			mHelper.ReportShortcutUsed(ID_ADD_WEBSITE);

			EditText editUri = new EditText(this);

			editUri.Hint = "http://www.xamarin.com/";
			editUri.InputType = InputTypes.TextVariationUri;

			new AlertDialog.Builder(this)
				.SetTitle("Add new website")
				.SetMessage("Type URL of a website")
				.SetView(editUri)
				.SetPositiveButton("Add", (dialog, whichButton) =>
				{
					var url = editUri.Text.Trim();
					if (url.Length > 0)
					{
						AddUriAsync(url);
					}
				})
				.Show();
		}

		void AddUriAsync(string url)
		{
			new AddUriTask(this, mHelper).Execute(url);
		}

		class AddUriTask : AsyncTask<Java.Lang.Object, Java.Lang.Object, Java.Lang.Void>
		{
			private ShortcutHelper Helper { get; set; }
			private Main Owner { get; set; }

			public AddUriTask(Main owner, ShortcutHelper helper)
			{
				Owner = owner;
				Helper = helper;
			}

			protected override void OnPostExecute(Java.Lang.Void result)
			{
				Owner.RefreshList();
			}

			protected override Java.Lang.Void RunInBackground(params Java.Lang.Object[] @params)
			{
				var uri = (string)@params[0];
				Helper.AddWebSiteShortcut(uri);
				return null;
			}
		}

		void RefreshList()
		{
			mAdapter.SetShortcuts(mHelper.GetShortcuts());
		}

		public void OnClick(View view)
		{
			ShortcutInfo shortcut = (ShortcutInfo) ((View) view.Parent).Tag;

			switch (view.Id)
			{
				case Resource.Id.disable:
					if (shortcut.IsEnabled)
					{
						mHelper.DisableShortcut(shortcut);
					}
					else
					{
						mHelper.EnableShortcut(shortcut);
					}
					RefreshList();
					break;
				case Resource.Id.remove:
					mHelper.RemoveShortcut(shortcut);
					RefreshList();
					break;
			}
		}

		static List<ShortcutInfo> EMPTY_LIST = new List<ShortcutInfo>();

		string GetType(ShortcutInfo shortcut)
		{
			StringBuilder sb = new StringBuilder();
			string sep = "";
			if (shortcut.IsDynamic)
			{
				sb.Append(sep);
				sb.Append("Dynamic");
				sep = ", ";
			}
			if (shortcut.IsPinned)
			{
				sb.Append(sep);
				sb.Append("Pinned");
				sep = ", ";
			}
			if (!shortcut.IsEnabled)
			{
				sb.Append(sep);
				sb.Append("Disabled");
				sep = ", ";
			}
			return sb.ToString();
		}

		class MyAdapter : BaseAdapter
		{
			private Context Context { get; set; }
			private LayoutInflater Inflater;
			private List<ShortcutInfo> mList = EMPTY_LIST;
			private Main Owner { get; set; }

			public MyAdapter(Context context)
			{
				Context = context;
				Inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
				Owner = (Main) context;
			}

			public override int Count
			{
				get
				{
					return mList.Count;
				}
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return (Java.Lang.Object) mList[position];
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override bool HasStableIds { get { return false; } }

			public override bool IsEnabled(int position)
			{
				return true;
			}

			public void SetShortcuts(List<ShortcutInfo> list)
			{
				mList = list;
				NotifyDataSetChanged();
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View view;
				if (convertView != null)
				{
					view = convertView;
				}
				else
				{
					view = Inflater.Inflate(Resource.Layout.ListItem, null);
				}

				BindView(view, position, mList[position]);

				return view;
			}

			public void BindView(View view, int position, ShortcutInfo shortcut)
			{
				view.Tag = shortcut;

				TextView line1 = view.FindViewById<TextView>(Resource.Id.line1);
				TextView line2 = view.FindViewById<TextView>(Resource.Id.line2);

				line1.Text = shortcut.LongLabel;

				line2.Text = Owner.GetType(shortcut);

				Button remove = view.FindViewById<Button>(Resource.Id.remove);
				Button disable = view.FindViewById<Button>(Resource.Id.disable);

				disable.Text = shortcut.IsEnabled
					? Owner.GetString(Resource.String.disable_shortcut)
					: Owner.GetString(Resource.String.enable_shortcut);

				remove.SetOnClickListener(Owner);
				disable.SetOnClickListener(Owner);
			}
		}
	}
}

