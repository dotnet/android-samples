/*
 * Copyright 2013 Urban Airship and Contributors
 */
//
// C# Port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.UrbanAirship.RichPush;
using Java.Text;
using Android.Graphics;
using Android.Appwidget;
using Android.Preferences;
using Android.Views.InputMethods;
using Xamarin.UrbanAirship.Utils;
using Xamarin.UrbanAirship.Push;
using Xamarin.UrbanAirship;

namespace Xamarin.Samples.UrbanAirship.RichPush
{
	/**
 * DialogPreference to set the tags
 *
 */
	public class AddTagsPreference : DialogPreference  {

		private ListView listView;
		private JavaList<String> tags = new JavaList<String>();
		private ICollection<string>  currentTags;
		private TagsAdapter adapter;

		public AddTagsPreference(Context context, IAttributeSet attrs) 
			: base (context, attrs)
		{

			currentTags = PushManager.Shared().Tags;
		}

		override
			protected View OnCreateDialogView() {
			tags.Clear();
			foreach (var currentTag in currentTags)
				tags.Add (currentTag);

			View view = base.OnCreateDialogView();
			listView = (ListView) view.FindViewById(Resource.Id.tags_list);
			adapter = new TagsAdapter(this, Context, Resource.Layout.tag_preference_item);
			listView.Adapter = adapter;

			EditText editText = (EditText) view.FindViewById(Resource.Id.new_tag_text);

			ImageButton button = (ImageButton) view.FindViewById(Resource.Id.new_tag_button);
			button.Click += delegate {
					String newTag = editText.Text.ToString();
					editText.Text = null;
					InputMethodManager imm = (InputMethodManager) Context.GetSystemService(
						Context.InputMethodService);
					imm.HideSoftInputFromWindow(editText.WindowToken, 0);

					if (!UAStringUtil.IsEmpty(newTag)) {
						if (tags.Contains(newTag)) {
							ShowDuplicateItemToast();
						} else {
							tags.Insert(0, newTag);
							adapter.NotifyDataSetChanged();
						}
					}
				};

			return view;
		}

		override
			protected View OnCreateView(ViewGroup parent) {
			View view = base.OnCreateView(parent);
			view.ContentDescription = "ADD_TAGS";
			return view;
		}

		override
			protected void OnDialogClosed(bool positiveResult) {
			if (positiveResult) {
				if (CallChangeListener(tags)) {
					SetTags(tags);

					NotifyChanged();
				}
			}
		}

		// FIXME: this cannot be overriden because of the virtual/non-virtual mismatch in generator.
		/*
		override
			public string Summary { get {
				return string.Join (", ", currentTags);
			}
		}
		*/

		override
			protected bool ShouldPersist() {
			return false;
		}

		private void SetTags(IList<String> tags) {
			currentTags.Clear();
			foreach (var tag in tags)
			currentTags.Add(tag);

			PushManager.Shared().Tags = currentTags;

			if (UAirship.Shared().AirshipConfigOptions.RichPushEnabled) {
				// FIXME: not sure if this use of ToArray() is safe...
				RichPushManager.Shared().RichPushUser.Tags = new Java.Util.HashSet (currentTags.ToArray ());
				RichPushManager.Shared().UpdateUser();
			}
		}

		private void ShowDuplicateItemToast() {
			Toast.MakeText(Context, Resource.String.duplicate_tag_warning, ToastLength.Short).Show();
		}

		private class TagsAdapter : ArrayAdapter<String> {
			AddTagsPreference owner;
			private int layout;

			public TagsAdapter(AddTagsPreference owner, Context context, int layout)
				: base (context, layout, owner.tags)
			{
				this.owner = owner;
				this.layout = layout;
			}

			private View CreateView(ViewGroup parent) {
				LayoutInflater layoutInflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);
				View view = layoutInflater.Inflate(layout, parent, false);
				return view;
			}

			override
				public View GetView(int position, View convertView, ViewGroup parent) {

				// Use either the convertView or create a new view
				View view = convertView == null ? CreateView(parent) : convertView;
				string tag = this.GetItem(position);

				TextView textView = (TextView) view.FindViewById(Resource.Id.tag_text);
				textView.Text = (tag);


				ImageButton button = (ImageButton) view.FindViewById(Resource.Id.delete_tag_button);
				button.Click += delegate {
						owner.tags.Remove(tag);
						NotifyDataSetChanged();
				};

				return view;
			}
		}

	}
}