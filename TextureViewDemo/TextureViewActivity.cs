using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;

// ported from sample code at http://developer.android.com/reference/android/view/TextureView.html

// this example requires hardware-acceleration to be enabled
namespace TextureViewDemo
{
    [Activity (Label = "TextureViewDemo", MainLauncher = true)]            
    public class TextureViewActivity : Activity, TextureView.ISurfaceTextureListener
    {
        Camera _camera;
        TextureView _textureView;
        
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Create your application here
            
            _textureView = new TextureView (this);
            _textureView.SurfaceTextureListener = this;
            
            SetContentView (_textureView);
        }

        #region ISurfaceTextureListener implementation
        
        public void OnSurfaceTextureAvailable (Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            _camera = Camera.Open ();
            
            var previewSize = _camera.GetParameters ().PreviewSize;
            _textureView.LayoutParameters = 
                new FrameLayout.LayoutParams (previewSize.Width, previewSize.Height, GravityFlags.Center);
            
            try {
                _camera.SetPreviewTexture (surface);
                _camera.StartPreview ();
            } catch (Java.IO.IOException ex) {
                Console.WriteLine (ex.Message);
            }
            
            // this is the sort of thing TextureView enables
            _textureView.Rotation = 45.0f;
            _textureView.Alpha = 0.5f;
        }

        public bool OnSurfaceTextureDestroyed (Android.Graphics.SurfaceTexture surface)
        {
            _camera.StopPreview ();
            _camera.Release ();
            
            return true;
        }

        public void OnSurfaceTextureSizeChanged (Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            // camera takes care of this
        }

        public void OnSurfaceTextureUpdated (Android.Graphics.SurfaceTexture surface)
        {
            
        }
        
        #endregion


    }
}

