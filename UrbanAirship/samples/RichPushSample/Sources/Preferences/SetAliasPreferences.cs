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
 * DialogPreference to set the alias
 *
 */
	public class SetAliasPreference : DialogPreference {

		private EditText editTextView;
		private String currentAlias;

		public SetAliasPreference(Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			currentAlias = PushManager.Shared().Preferences.Alias;
		}

		override
			protected View OnCreateDialogView() {
			editTextView = new EditText(Context);
			editTextView.Text = currentAlias;

			return editTextView;
		}

		override
			protected View OnCreateView(ViewGroup parent) {
			View view = base.OnCreateView(parent);
			view.ContentDescription = "SET_ALIAS";
			return view;
		}

		override
			protected void OnDialogClosed(bool positiveResult) {
			if (positiveResult) {
				String alias = editTextView.Text;
				if (CallChangeListener (alias)) {
					SetAlias (alias);
					NotifyChanged();
				}
			}
		}

		private void SetAlias(string alias) {
			alias = UAStringUtil.IsEmpty(alias) ? null : alias;

			PushManager.Shared().Alias = (alias);

			if (UAirship.Shared().AirshipConfigOptions.RichPushEnabled) {
				RichPushManager.Shared().RichPushUser.Alias = (alias);
				RichPushManager.Shared().UpdateUser();
			}

			currentAlias = alias;
		}

		// FIXME: this cannot be overriden due to generator issue for virtual-getter and non-virtual setter.
		/*
		override
			public String Summary {
			get {
			return currentAlias;
			}
		}
		*/

		override
			protected bool ShouldPersist() {
			return false;
		}
	}
}
