using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Util;
using Android.Content;
using Android.Runtime;
using System.Collections.Generic;

using Javax.Microedition.Khronos.Egl;

namespace Mono.Samples.GLDiag {

	class PaintingView : AndroidGameView
	{

		public PaintingView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		private void Initialize ()
		{
		}

		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		protected override void CreateFrameBuffer ()
		{
			IEGL10 egl = EGLContext.EGL.JavaCast<IEGL10> ();
			var win = new AndroidWindow (Holder);
			win.InitializeDisplay ();
			
			int[] num_configs = new int[1];
			if (!egl.EglGetConfigs (win.Display, null, 0, num_configs)) {
				throw EglException.GenerateException ("Failed to retrieve GraphicsMode configurations", egl);
			}

			EGLConfig[] configs = new EGLConfig[num_configs[0]];
			if (!egl.EglGetConfigs (win.Display, configs, num_configs[0], num_configs)) {
				throw EglException.GenerateException ("Failed to retrieve GraphicsMode configurations", egl);
			}

			Log.Verbose ("GLDiag", "Testing {0} graphics configurations", num_configs[0]);

			Dictionary<IntPtr, AndroidGraphicsMode> validModes = new Dictionary<IntPtr, AndroidGraphicsMode> ();

			int count = 0;
			foreach (var c in configs) {
				var r = GetAttrib (egl, win.Display, c, EGL11.EglRedSize);
				var g = GetAttrib (egl, win.Display, c, EGL11.EglGreenSize);
				var b = GetAttrib (egl, win.Display, c, EGL11.EglBlueSize);
				var a = GetAttrib (egl, win.Display, c, EGL11.EglAlphaSize);
				var depth = GetAttrib (egl, win.Display, c, EGL11.EglDepthSize);
				var stencil = GetAttrib (egl, win.Display, c, EGL11.EglStencilSize);
				var s = GetAttrib (egl, win.Display, c, EGL11.EglSampleBuffers);
				var samples = GetAttrib (egl, win.Display, c, EGL11.EglSamples);

				Log.Verbose ("AndroidGraphicsMode", "Testing graphics mode: {8} red {0} green {1} blue {2} alpha {3} ({4}) depth {5} stencil {6} samples {7}",
						r, g, b,
						a, r+g+b+a, depth,
						stencil, samples, count++);
			
				try {
					win.CreateSurface (c);
					win.DestroySurface ();
					validModes.Add (c.Handle, new AndroidGraphicsMode (r+g+b+a, depth, stencil, s > 0 ? samples : 0, 2, true));
					Log.Verbose ("AndroidGraphicsMode", "Graphics mode {0} valid", count-1);
				} catch {
					Log.Verbose ("AndroidGraphicsMode", "Graphics mode {0} invalid", count-1);
				}
			}
			
			win.TerminateDisplay ();

			if (validModes.Count == 0)
				throw new EglException ("There is no valid graphics mode, aborting");

			IntPtr key = IntPtr.Zero;
			foreach (var k in validModes) {
				if (key == IntPtr.Zero)
					key = k.Key;
				var a = k.Value;
				Log.Verbose ("AndroidGraphicsMode", "Valid graphics mode: {9} red {0} green {1} blue {2} alpha {3} ({4}) depth {5} stencil {6} samples {7} buffers {8}",
						a.ColorFormat.Red, a.ColorFormat.Green, a.ColorFormat.Blue,
						a.ColorFormat.Alpha, a.ColorFormat.BitsPerPixel, a.Depth,
						a.Stencil, a.Samples, a.Buffers, (int)k.Key);
			}

			GraphicsMode = validModes[key];

			// if you don't call this, the context won't be created
			base.CreateFrameBuffer ();
		}

		int GetAttrib (IEGL10 egl, EGLDisplay display, EGLConfig config, int attrib)
		{
			int[] ret = new int [1];
			try {
				egl.EglGetConfigAttrib (display, config, attrib, ret);
			} catch (Exception e) {
				Log.Warn ("AndroidGraphicsMode", "EglGetConfigAttrib {0} threw exception {1}", attrib, e);
			}
			return ret[0];
		}

		class EglException : InvalidOperationException
		{
			public static EglException GenerateException (string msg, IEGL10 egl)
			{
				if (egl == null)
					return new EglException (msg);
				if (egl.EglGetError () == EGL10.EglSuccess)
					return new EglException (msg);
				return new EglException (String.Format ("{0} failed with error {1} (0x{1:x})", msg, egl.EglGetError()));
			}

			public EglException (string msg) : base(msg)
			{
			}
		}

	}
}
