/*
 * Copyright (C) 2007 The Android Open Source Project
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

//
// C# port Copyright (C) 2012 Xamarin Inc. http://xamarin.com
//

using System;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;
using Android.Opengl;

using Object = Java.Lang.Object;

namespace MonoDroid.ApiDemo
{
	/**
	 * Render a pair of tumbling cubes.
	 */
	class CubeRenderer : Object, GLSurfaceView.IRenderer
	{
		public CubeRenderer (bool useTranslucentBackground) 
		{
			mTranslucentBackground = useTranslucentBackground;
			mCube = new Cube ();
		}

		public void OnDrawFrame (IGL10 gl) 
		{
			/*
			 * Usually, the first thing one might want to do is to clear
			 * the screen. The most efficient way of doing this is to use
			 * glClear().
			 */
			gl.GlClear (GL10.GlColorBufferBit | GL10.GlDepthBufferBit);

			/*
			 * Now we're ready to draw some 3D objects
			 */
			
			gl.GlMatrixMode (GL10.GlModelview);
			gl.GlLoadIdentity ();
			gl.GlTranslatef (0, 0, -3.0f);
			gl.GlRotatef (mAngle,        0, 1, 0);
			gl.GlRotatef (mAngle*0.25f,  1, 0, 0);
			
			gl.GlEnableClientState (GL10.GlVertexArray);
			gl.GlEnableClientState (GL10.GlColorArray);
			
			mCube.Draw (gl);
			
			gl.GlRotatef (mAngle*2.0f, 0, 1, 1);
			gl.GlTranslatef (0.5f, 0.5f, 0.5f);
			
			mCube.Draw (gl);
			
			mAngle += 1.2f;
		}

		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			gl.GlViewport (0, 0, width, height);

			/*
			 * Set our projection matrix. This doesn't have to be done
			 * each time we draw, but usually a new projection needs to
			 * be set when the viewport is resized.
			 */

			float ratio = (float) width / height;
			gl.GlMatrixMode (GL10.GlProjection);
			gl.GlLoadIdentity ();
			gl.GlFrustumf (-ratio, ratio, -1, 1, 1, 10);
		}

		public void OnSurfaceCreated (IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config) 
		{
			/*
			 * By default, OpenGL enables features that improve quality
			 * but reduce performance. One might want to tweak that
			 * especially on software renderer.
			 */
			gl.GlDisable (GL10.GlDither);

			/*
			 * Some one-time OpenGL initialization can be made here
			 * probably based on features of this particular context
			 */
			gl.GlHint (GL10.GlPerspectiveCorrectionHint, GL10.GlFastest);

			if (mTranslucentBackground)
				gl.GlClearColor (0,0,0,0);
			else
				gl.GlClearColor (1,1,1,1);
			
			// FIXME: Mono.Android.dll misses this constant. Filed as #3531.
			gl.GlEnable(2884);//GL10.GlCullFace);
			gl.GlShadeModel(GL10.GlSmooth);
			gl.GlEnable(GL10.GlDepthTest);
		}
		bool mTranslucentBackground;
		Cube mCube;
		float mAngle;
	}
}


