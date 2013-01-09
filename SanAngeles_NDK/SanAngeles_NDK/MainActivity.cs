/*
 * Copyright (C) 2009 The Android Open Source Project
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
 *
 * This is a small port of the "San Angeles Observation" demo
 * program for OpenGL ES 1.x. For more details, see:
 *
 *    http://jet.ro/visuals/san-angeles-observation/
 *
 * This program demonstrates how to use a GLSurfaceView from Java
 * along with native OpenGL calls to perform frame rendering.
 *
 * Touching the screen will start/stop the animation.
 *
 * Note that the demo runs much faster on the emulator than on
 * real devices, this is mainly due to the following facts:
 *
 * - the demo sends bazillions of polygons to OpenGL without
 *   even trying to do culling. Most of them are clearly out
 *   of view.
 *
 * - on a real device, the GPU bus is the real bottleneck
 *   that prevent the demo from getting acceptable performance.
 *
 * - the software OpenGL engine used in the emulator uses
 *   the system bus instead, and its code rocks :-)
 *
 * Fixing the program to send less polygons to the GPU is left
 * as an exercise to the reader. As always, patches welcomed :-)
 */
using System;

using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;

using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Views;
using System.Runtime.InteropServices;

namespace SanAngles
{
	[Activity (Label = "Mono SanAngeles sample", MainLauncher = true)]
	public class DemoActivity : Activity {
		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
			mGLView = new DemoGLSurfaceView (this);
			SetContentView (mGLView);
		}
		
		protected override void OnPause () 
		{
			base.OnPause();
			mGLView.OnPause ();
		}
		
		protected override void OnResume () 
		{
			base.OnResume ();
			mGLView.OnResume ();
		}
		
		private GLSurfaceView mGLView;
	}

	class DemoGLSurfaceView : GLSurfaceView
	{
		public DemoGLSurfaceView (Context context) 
				: base (context)
		{
			mRenderer = new DemoRenderer ();
			SetRenderer (mRenderer);
		}
		
		public override bool OnTouchEvent (MotionEvent evt) 
		{
			if (evt.Action == MotionEventActions.Down)
				nativePause (IntPtr.Zero);

			return true;
		}
		
		DemoRenderer mRenderer;

		[DllImport ("sanangeles", EntryPoint = "Java_com_example_SanAngeles_DemoGLSurfaceView_nativePause")]
		static extern void nativePause (IntPtr jnienv);
	}

	class DemoRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		public void OnSurfaceCreated (IGL10 gl, EGLConfig config) 
		{
			nativeInit (IntPtr.Zero);
		}
		
		public void OnSurfaceChanged (IGL10 gl, int w, int h) 
		{
			//gl.glViewport(0, 0, w, h);
			nativeResize (IntPtr.Zero, IntPtr.Zero, w, h);
		}
		
		public void OnDrawFrame (IGL10 gl) 
		{
			nativeRender (IntPtr.Zero);
		}
		
		[DllImport ("sanangeles", EntryPoint = "Java_com_example_SanAngeles_DemoRenderer_nativeInit")]
		private static extern void nativeInit (IntPtr jnienv);
		[DllImport ("sanangeles", EntryPoint = "Java_com_example_SanAngeles_DemoRenderer_nativeResize")]
		private static extern void nativeResize (IntPtr jnienv, IntPtr thiz, int w, int h);
		[DllImport ("sanangeles", EntryPoint = "Java_com_example_SanAngeles_DemoRenderer_nativeRender")]
		private static extern void nativeRender (IntPtr jnienv);
		[DllImport ("sanangeles", EntryPoint = "Java_com_example_SanAngeles_DemoRenderer_nativeDone")]
		private static extern void nativeDone (IntPtr jnienv);
	}
}
