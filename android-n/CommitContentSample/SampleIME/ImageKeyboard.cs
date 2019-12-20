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

using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.View.InputMethod;
using Java.IO;
using Java.Lang;
using SampleIME;
using File = Java.IO.File;

namespace CommitContentSampleIME
{
	[Service(Permission = "android.permission.BIND_INPUT_METHOD")]
	[IntentFilter(new [] { "android.view.InputMethod" })]
	[MetaData("android.view.im", Resource = "@xml/method")]
	public class ImageKeyboard : InputMethodService
	{
		const string Tag = "ImageKeyboard";
		const string Authority = "com.xamarin.commitcontentsampleime.inputcontent";
		const string MimeTypeGif = "image/gif";
		const string MimeTypePng = "image/png";
		const string MimeTypeWebp = "image/webp";

		File mPngFile;
		File mGifFile;
		File mWebpFile;
		Button mGifButton;
		Button mPngButton;
		Button mWebpButton;

		bool IsCommitContentSupported(EditorInfo editorInfo, string mimeType)
		{
			if (editorInfo == null)
			{
				return false;
			}
			var ic = CurrentInputConnection;
			if (ic == null)
			{
				return false;
			}

			if (!ValidatePackageName(editorInfo))
			{
				return false;
			}

			var supportedMimeTypes = EditorInfoCompat.GetContentMimeTypes(editorInfo);
			foreach (var supportedMimeType in supportedMimeTypes)
			{
				if (ClipDescription.CompareMimeTypes(mimeType, supportedMimeType))
				{
					return true;
				}
			}
			return false;
		}

		void DoCommitContent(string description, string mimeType, File file)
		{
			var editorInfo = CurrentInputEditorInfo;

			// Validate packageName again just in case.
			if (!ValidatePackageName(editorInfo))
			{
				return;
			}

			var contentUri = FileProvider.GetUriForFile(this, Authority, file);

			// As you as an IME author are most likely to have to implement your own content provider
			// to support CommitContent API, it is important to have a clear spec about what
			// applications are going to be allowed to access the content that your are going to share.
			int flag;
			if ((int) Build.VERSION.SdkInt >= 25)
			{
				// On API 25 and later devices, as an analogy of Intent.FLAG_GRANT_READ_URI_PERMISSION,
				// you can specify InputConnectionCompat.INPUT_CONTENT_GRANT_READ_URI_PERMISSION to give
				// a temporary read access to the recipient application without exporting your content
				// provider.
				flag = InputConnectionCompat.InputContentGrantReadUriPermission;
			}
			else
			{
				// On API 24 and prior devices, we cannot rely on
				// InputConnectionCompat.INPUT_CONTENT_GRANT_READ_URI_PERMISSION. You as an IME author
				// need to decide what access control is needed (or not needed) for content URIs that
				// you are going to expose. This sample uses Context.grantUriPermission(), but you can
				// implement your own mechanism that satisfies your own requirements.
				flag = 0;
				try
				{
					// TODO: Use revokeUriPermission to revoke as needed.
					GrantUriPermission(
							editorInfo.PackageName, contentUri, ActivityFlags.GrantReadUriPermission);
				}
				catch (Exception e)
				{
					Log.Error(Tag, "grantUriPermission failed packageName=" + editorInfo.PackageName
							+ " contentUri=" + contentUri, e);
				}
			}

			var inputContentInfoCompat = new InputContentInfoCompat(
				contentUri,
				new ClipDescription(description, new [] { mimeType }),
				null /* linkUrl */);
			InputConnectionCompat.CommitContent(
					CurrentInputConnection, CurrentInputEditorInfo, inputContentInfoCompat,
					flag, null);
		}

		bool ValidatePackageName(EditorInfo editorInfo)
		{
			if (editorInfo == null)
			{
				return false;
			}
			var packageName = editorInfo.PackageName;
			if (packageName == null)
			{
				return false;
			}

			// In Android L MR-1 and prior devices, EditorInfo.packageName is not a reliable identifier
			// of the target application because:
			//   1. the system does not verify it [1]
			//   2. InputMethodManager.startInputInner() had filled EditorInfo.packageName with
			//      view.getContext().getPackageName() [2]
			// [1]: https://android.googlesource.com/platform/frameworks/base/+/a0f3ad1b5aabe04d9eb1df8bad34124b826ab641
			// [2]: https://android.googlesource.com/platform/frameworks/base/+/02df328f0cd12f2af87ca96ecf5819c8a3470dc8
			if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
			{
				return true;
			}

			var inputBinding = CurrentInputBinding;
			if (inputBinding == null)
			{
				// Due to b.android.com/225029, it is possible that getCurrentInputBinding() returns
				// null even after onStartInputView() is called.
				// TODO: Come up with a way to work around this bug....
				Log.Error(Tag, "inputBinding should not be null here. You are likely to be hitting b.android.com/225029");
				return false;
			}
			var packageUid = inputBinding.Uid;

			if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
			{
				var appOpsManager =  (AppOpsManager)GetSystemService(AppOpsService);
				try
				{
					appOpsManager.CheckPackage(packageUid, packageName);
				}
				catch (Exception e)
				{
					return false;
				}
				return true;
			}

			var packageManager = PackageManager;
			var possiblePackageNames  = packageManager.GetPackagesForUid(packageUid);
			foreach (var possiblePackageName in possiblePackageNames)
			{
				if (packageName.Equals(possiblePackageName))
				{
					return true;
				}
			}
			return false;
		}

		public override void OnCreate()
		{
			base.OnCreate();

			// TODO: Avoid file I/O in the main thread.
			var imagesDir = new File(FilesDir, "images");
			imagesDir.Mkdirs();
			mGifFile = GetFileForResource(this, Resource.Raw.animated_gif, imagesDir, "image.gif");
			mPngFile = GetFileForResource(this, Resource.Raw.dessert_android, imagesDir, "image.png");
			mWebpFile = GetFileForResource(this, Resource.Raw.animated_webp, imagesDir, "image.webp");
		}

		public override View OnCreateInputView()
		{
			mGifButton = new Button(this) {Text = "Insert GIF"};
			mGifButton.Click += delegate
			{
				DoCommitContent("A waving flag", MimeTypeGif, mGifFile);
			};

			mPngButton = new Button(this) {Text = "Insert PNG"};
			mPngButton.Click += delegate
			{
				DoCommitContent("A droid logo", MimeTypePng, mPngFile);
			};

			mWebpButton = new Button(this) {Text = "Insert WebP"};
			mWebpButton.Click += delegate
			{
				DoCommitContent("Android N recovery animation", MimeTypeWebp, mWebpFile);
			};

			var layout = new LinearLayout(this) {Orientation = Orientation.Vertical};
			layout.AddView(mGifButton);
			layout.AddView(mPngButton);
			layout.AddView(mWebpButton);
			return layout;
		}

		public override bool OnEvaluateFullscreenMode()
		{
			return false;
		}

		public override void OnStartInputView(EditorInfo info, bool restarting)
		{
			mGifButton.Enabled = mGifFile != null && IsCommitContentSupported(info, MimeTypeGif);
			mPngButton.Enabled = mPngFile != null && IsCommitContentSupported(info, MimeTypePng);
			mWebpButton.Enabled = mWebpFile != null && IsCommitContentSupported(info, MimeTypeWebp);
		}

		static File GetFileForResource(Context context, int res, File outputDir, string filename)
		{
			var outputFile = new File(outputDir, filename);
			var buffer = new byte[4096];
			using (var resourceReader = context.Resources.OpenRawResource(res))
			using (OutputStream dataWriter = new FileOutputStream(outputFile))
			{
				var hasContent = true;
				while (hasContent)
				{
					var numRead = resourceReader.Read(buffer, 0, buffer.Length);
					hasContent = numRead > 0;
					if (hasContent)
					{
						dataWriter.Write(buffer, 0, numRead);
					}
				}
				return outputFile;
			}
		}
	}
}