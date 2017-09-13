using System;
using Android.App;
using Android.Runtime;
using Android.Support.Text.Emoji;
using Android.Support.Text.Emoji.Bundled;
using Android.Support.V4.Provider;
using Android.Util;

namespace EmojiCompatSample
{
	[Application]
	public class EmojiCompatApplication : Application
	{
		protected const string Tag = "EmojiCompatApplication";

		/** Change this to {@code false} when you want to use the downloadable Emoji font. */
		const bool UseBundledEmoji = true;

		public EmojiCompatApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) 
		{}

		public override void OnCreate()
		{
			base.OnCreate();

			EmojiCompat.Config config;
			if (UseBundledEmoji) 
			{
				// Use the bundled font for EmojiCompat
				config = new BundledEmojiCompatConfig(this);
			}
			else 
			{
				// Use a downloadable font for EmojiCompat
				var fontRequest = new FontRequest(
					"com.google.android.gms.fonts",
					"com.google.android.gms",
					"Noto Color Emoji Compat",
					Resource.Array.com_google_android_gms_fonts_certs);
				config = new FontRequestEmojiCompatConfig(this, fontRequest)
					.SetReplaceAll(true)
					.RegisterInitCallback(new InitCallbackImpl());
			}

			EmojiCompat.Init(config);
		}

		class InitCallbackImpl : EmojiCompat.InitCallback
		{
			public override void OnInitialized()
			{
				Log.Info(Tag, "EmojiCompat initialized");
			}

			public override void OnFailed(Java.Lang.Throwable throwable)
			{
				Log.Error(Tag, "EmojiCompat initialization failed", throwable);
			}
		}
	}
}
