using System;

using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V17.Leanback.App;
using Squareup.Picasso;

namespace TvLeanback
{
	//Extend from Java.Lang.Object and not System.Object so the Handler and Dispose methods are taken care of
	public class PicassoBackgroundManagerTarget : Java.Lang.Object, ITarget
	{
		BackgroundManager mBackgroundManager;

		public PicassoBackgroundManagerTarget (BackgroundManager backgroundManager)
		{
			this.mBackgroundManager = backgroundManager;
		}

		public void OnBitmapLoaded (Bitmap bitmap, Picasso.LoadedFrom loadedFrom)
		{
			this.mBackgroundManager.SetBitmap (bitmap);
		}

		public void OnBitmapFailed (Drawable drawable)
		{
			this.mBackgroundManager.Drawable = drawable;
		}

		public void OnPrepareLoad (Drawable drawable)
		{
		}

	}
}

