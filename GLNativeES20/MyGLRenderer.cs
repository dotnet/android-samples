using System;

using Android.Views;
using Android.Content;
using Android.Util;
using Android.Opengl;
using Android.OS;

using Java.Lang;

namespace GLNativeES20
{
	class MyGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		private static string TAG = "MyGLRenderer";
		private Triangle mTriangle;
		private Square   mSquare;
		private float[] mMVPMatrix = new float[16];
		private float[] mProjMatrix = new float[16];
		private float[] mVMatrix = new float[16];
		private float[] mRotationMatrix = new float[16];

		#region IRenderer implementation
		public void OnDrawFrame (Javax.Microedition.Khronos.Opengles.IGL10 gl)
		{
			// Draw background color
			GLES20.GlClear ((int)GLES20.GlColorBufferBit);

			// Set the camera position (View matrix)
			Matrix.SetLookAtM (mVMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1.0f, 0.0f);

			// Calculate the projection and view transformation
			Matrix.MultiplyMM (mMVPMatrix, 0, mProjMatrix, 0, mVMatrix, 0);

			// Draw square
			mSquare.Draw (mMVPMatrix);

			// Create a rotation for the triangle
			// long time = SystemClock.UptimeMillis() % 4000L;
			// float angle = 0.090f * ((int) time);
			Matrix.SetRotateM (mRotationMatrix, 0, Angle, 0, 0, -1.0f);

			// Combine the rotation matrix with the projection and camera view
			Matrix.MultiplyMM (mMVPMatrix, 0, mRotationMatrix, 0, mMVPMatrix, 0);

			// Draw triangle
			mTriangle.Draw (mMVPMatrix);
		}

		public void OnSurfaceChanged (Javax.Microedition.Khronos.Opengles.IGL10 gl, int width, int height)
		{
			// Adjust the viewport based on geometry changes,
			// such as screen rotation
			GLES20.GlViewport (0, 0, width, height);

			float ratio = (float)width / height;

			// this projection matrix is applied to object coordinates
			// in the onDrawFrame() method
			Matrix.FrustumM (mProjMatrix, 0, -ratio, ratio, -1, 1, 3, 7);
		}

		public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
		{
			// Set the background frame color
			GLES20.GlClearColor (0.0f, 0.0f, 0.0f, 1.0f);

			mTriangle = new Triangle ();
			mSquare = new Square ();
		}
		#endregion

		public static int LoadShader (int type, string shaderCode)
		{
			// create a vertex shader type (GLES20.GL_VERTEX_SHADER)
			// or a fragment shader type (GLES20.GL_FRAGMENT_SHADER)
			int shader = GLES20.GlCreateShader (type);

			// add the source code to the shader and compile it
			GLES20.GlShaderSource (shader, shaderCode);
			GLES20.GlCompileShader (shader);

			return shader;
		}

		/**
		* Utility method for debugging OpenGL calls. Provide the name of the call
		* just after making it:
		*
		* <pre>
		* mColorHandle = GLES20.glGetUniformLocation(mProgram, "vColor");
		* MyGLRenderer.checkGlError("glGetUniformLocation");</pre>
		*
		* If the operation is not successful, the check throws an error.
		*
		* @param glOperation - Name of the OpenGL call to check.
		*/
		public static void CheckGlError (string glOperation)
		{
			int error;
			while ((error = GLES20.GlGetError ()) != GLES20.GlNoError) {
				Log.Error (TAG, glOperation + ": glError " + error);
				throw new RuntimeException (glOperation + ": glError " + error);
			}
		}

		public float Angle {
			get;
			set;
		}

	}

}

