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
 * limitations under the License
 */

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using AndroidX.Core.View.InputMethod;
using Java.Lang;
using Java.Util;
using SampleApp;

namespace CommitContentSampleApp
{
	[Activity(Label = "CommitContentSampleApp", MainLauncher = true, Theme = "@style/AppTheme")]
	public class MainActivity : Activity
	{
		const string InputContentInfoKey = "COMMIT_CONTENT_INPUT_CONTENT_INFO";
		const string CommitContentFlagsKey = "COMMIT_CONTENT_FLAGS";

		const string Tag = "CommitContentSupport";

		WebView mWebView;
		TextView mLabel;
		TextView mContentUri;
		TextView mLinkUri;
		TextView mMimeTypes;
		TextView mFlags;

		InputContentInfoCompat mCurrentInputContentInfo;
		int mCurrentFlags;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView (Resource.Layout.commit_content);

			var layout = FindViewById<LinearLayout>(Resource.Id.commit_content_sample_edit_boxes);

			// This declares that the IME cannot commit any content with
			// InputConnectionCompat#commitContent().
			layout.AddView(CreateEditTextWithContentMimeTypes(null));

			// This declares that the IME can commit contents with
			// InputConnectionCompat#commitContent() if they match "image/gif".
			layout.AddView(CreateEditTextWithContentMimeTypes(new [] { "image/gif" }));

			// This declares that the IME can commit contents with
			// InputConnectionCompat#commitContent() if they match "image/png".
			layout.AddView(CreateEditTextWithContentMimeTypes(new [] { "image/png" }));

			// This declares that the IME can commit contents with
			// InputConnectionCompat#commitContent() if they match "image/jpeg".
			layout.AddView(CreateEditTextWithContentMimeTypes(new [] { "image/jpeg" }));

			// This declares that the IME can commit contents with
			// InputConnectionCompat#commitContent() if they match "image/webp".
			layout.AddView(CreateEditTextWithContentMimeTypes(new [] { "image/webp" }));

			// This declares that the IME can commit contents with
			// InputConnectionCompat#commitContent() if they match "image/png", "image/gif",
			// "image/jpeg", or "image/webp".
			layout.AddView(CreateEditTextWithContentMimeTypes(new [] { "image/png", "image/gif", "image/jpeg", "image/webp" }));

			mWebView = FindViewById<WebView>(Resource.Id.commit_content_webview);
			mMimeTypes = FindViewById<TextView>(Resource.Id.text_commit_content_mime_types);
			mLabel = FindViewById<TextView>(Resource.Id.text_commit_content_label);
			mContentUri = FindViewById<TextView>(Resource.Id.text_commit_content_content_uri);
			mLinkUri = FindViewById<TextView>(Resource.Id.text_commit_content_link_uri);
			mFlags = FindViewById<TextView>(Resource.Id.text_commit_content_link_flags);

			if (bundle == null) return;
			var previousInputContentInfo = InputContentInfoCompat.Wrap(
				bundle.GetParcelable(InputContentInfoKey));
			var previousFlags = bundle.GetInt(CommitContentFlagsKey);
			if (previousInputContentInfo != null)
			{
				OnCommitContentInternal(previousInputContentInfo, previousFlags);
			}
		}

		bool OnCommitContent(InputContentInfoCompat inputContentInfo, int flags,
			Bundle opts, string[] contentMimeTypes)
		{
			// Clear the temporary permission (if any).  See below about why we do this here.
			try
			{
				if (mCurrentInputContentInfo != null)
				{
					mCurrentInputContentInfo.ReleasePermission();
				}
			}
			catch (Java.Lang.Exception e)
			{
				Log.Error(Tag, "InputContentInfoCompat#releasePermission() failed.", e);
			}
			finally
			{
				mCurrentInputContentInfo = null;
			}

			mWebView.LoadUrl("about:blank");
			mMimeTypes.Text = "";
			mContentUri.Text = "";
			mLabel.Text = "";
			mLinkUri.Text = "";
			mFlags.Text = "";

			var supported = false;
			foreach (var contentMimeType in contentMimeTypes)
			{
				if (inputContentInfo.Description.HasMimeType(contentMimeType))
				{
					supported = true;
					break;
				}

			}
			if (!supported)
			{
				return false;
			}

			return OnCommitContentInternal(inputContentInfo, flags);
		}

		bool OnCommitContentInternal(InputContentInfoCompat inputContentInfo, int flags)
		{
			if ((flags & InputConnectionCompat.InputContentGrantReadUriPermission) != 0)
			{
				try
				{
					inputContentInfo.RequestPermission();
				}
				catch (Java.Lang.Exception e)
				{
					Log.Error(Tag, "InputContentInfoCompat#requestPermission() failed.", e);
					return false;
				}
			}

			mMimeTypes.Text = string.Join(".", inputContentInfo.Description.FilterMimeTypes("*/*"));
			mContentUri.Text = inputContentInfo.ContentUri.ToString();
			mLabel.Text = inputContentInfo.Description.Label;
			Android.Net.Uri linkUri = inputContentInfo.LinkUri;
			mLinkUri.Text = linkUri != null ? linkUri.ToString() : "null";
			mFlags.Text = FlagsToString(flags);
			mWebView.LoadUrl(inputContentInfo.ContentUri.ToString());
			mWebView.SetBackgroundColor(Color.Transparent);

			// Due to the asynchronous nature of WebView, it is a bit too early to call
			// inputContentInfo.releasePermission() here. Hence we call IC#releasePermission() when this
			// method is called next time.  Note that calling IC#releasePermission() is just to be a
			// good citizen. Even if we failed to call that method, the system would eventually revoke
			// the permission sometime after inputContentInfo object gets garbage-collected.
			mCurrentInputContentInfo = inputContentInfo;
			mCurrentFlags = flags;

			return true;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (mCurrentInputContentInfo != null)
			{
				outState.PutParcelable(InputContentInfoKey, (IParcelable) mCurrentInputContentInfo.Unwrap());
				outState.PutInt(CommitContentFlagsKey, mCurrentFlags);
			}
			mCurrentInputContentInfo = null;
			mCurrentFlags = 0;
			base.OnSaveInstanceState(outState);
		}

		/**
		 * Creates a new instance of {@link EditText} that is configured to specify the given content
		 * MIME types to EditorInfo#contentMimeTypes so that developers can locally test how the current
		 * input method behaves for such content MIME types.
		 *
		 * @param contentMimeTypes A {@link String} array that indicates the supported content MIME
		 *                         types
		 * @return a new instance of {@link EditText}, which specifies EditorInfo#contentMimeTypes with
		 * the given content MIME types
		 */
		EditText CreateEditTextWithContentMimeTypes(string[] contentMimeTypes)
		{
			string hintText;
			string[] mimeTypes;  // our own copy of contentMimeTypes.
			if (contentMimeTypes == null || contentMimeTypes.Length == 0)
			{
				hintText = "MIME: []";
				mimeTypes = new string[0];
			}
			else
			{
				hintText = "MIME: " + string.Join(".", contentMimeTypes);
				mimeTypes = new string[contentMimeTypes.Length];
				contentMimeTypes.CopyTo(mimeTypes, 0);
			}

			var editText = new CustomEditText(this)
			{
				Owner = this, MimeTypes = mimeTypes, Hint = hintText
			};
			editText.SetTextColor(Color.White);
			editText.SetHintTextColor(Color.White);
			return editText;
		}

		class CustomEditText : EditText
		{
			public string[] MimeTypes { get; set; }
			public MainActivity Owner { get; set; }
			public CustomEditText(Context context) : base(context)
			{
			}
			
			public override IInputConnection OnCreateInputConnection(EditorInfo editorInfo)
			{
				var ic = base.OnCreateInputConnection(editorInfo);
				EditorInfoCompat.SetContentMimeTypes(editorInfo, MimeTypes);
				var callback = new OnCommitContentListenerImpl() { MimeTypes = MimeTypes, Owner = Owner };
				return InputConnectionCompat.CreateWrapper(ic, editorInfo, callback);
			}

			class OnCommitContentListenerImpl : Java.Lang.Object, InputConnectionCompat.IOnCommitContentListener
			{
				public string[] MimeTypes { get; set; }
				public MainActivity Owner { get; set; }
				public bool OnCommitContent(InputContentInfoCompat inputContentInfo, int flags, Bundle opts)
				{
					return Owner.OnCommitContent(inputContentInfo, flags, opts, MimeTypes);
				}
			}

		}

		/**
		 * Converts {@code flags} specified in {@link InputConnectionCompat#commitContent(
		 * InputConnection, EditorInfo, InputContentInfoCompat, int, Bundle)} to a human readable
		 * string.
		 *
		 * @param flags the 2nd parameter of
		 *              {@link InputConnectionCompat#commitContent(InputConnection, EditorInfo,
		 *              InputContentInfoCompat, int, Bundle)}
		 * @return a human readable string that corresponds to the given {@code flags}
		 */
		static string FlagsToString(int flags)
		{
			var tokens = new ArrayList();
			if ((flags & InputConnectionCompat.InputContentGrantReadUriPermission) != 0)
			{
				tokens.Add("INPUT_CONTENT_GRANT_READ_URI_PERMISSION");
				flags &= ~InputConnectionCompat.InputContentGrantReadUriPermission;
			}
			if (flags != 0)
			{
				tokens.Add("0x" + Integer.ToHexString(flags));
			}
			return TextUtils.Join(" | ", tokens);
		}
	}
}

