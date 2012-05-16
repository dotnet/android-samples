/*
 * Copyright (C) 2011 The Android Open Source Project
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

