using System;
using Android.Views;
using Android.Graphics;

namespace Camera2VideoSample
{
	public class MySurfaceTextureListener: Java.Lang.Object,TextureView.ISurfaceTextureListener
	{
		Camera2VideoFragment fragment;
		public MySurfaceTextureListener(Camera2VideoFragment frag)
		{
			fragment = frag;
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface_texture,int width, int height)
		{
			fragment.openCamera (width,height);
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface_texture, int width, int height)
		{
			fragment.configureTransform (width, height);
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface_texture)
		{
			return true;
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface_texture)
		{
		}

	}
}

