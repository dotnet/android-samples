using System;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.Opengl;
using Java.Nio;

namespace GLNativeES30
{
	class Triangle
	{
		private string vertexShaderCode =
	        // This matrix member variable provides a hook to manipulate
	        // the coordinates of the objects that use this vertex shader
			"uniform mat4 uMVPMatrix;" +

			"attribute vec4 vPosition;" +
			"void main() {" +
			// the matrix must be included as a modifier of gl_Position
			"  gl_Position = vPosition * uMVPMatrix;" +
			"}";
		private string fragmentShaderCode =
			"precision mediump float;" +
			"uniform vec4 vColor;" +
			"void main() {" +
			"  gl_FragColor = vColor;" +
			"}";
		private FloatBuffer vertexBuffer;
		private int mProgram;
		private int mPositionHandle;
		private int mColorHandle;
		private int mMVPMatrixHandle;
		// number of coordinates per vertex in this array
		static int COORDS_PER_VERTEX = 3;
		static float[] triangleCoords = new float [] { // in counterclockwise order:
			0.0f,  0.622008459f, 0.0f,   // top
			-0.5f, -0.311004243f, 0.0f,   // bottom left
			0.5f, -0.311004243f, 0.0f    // bottom right
		};
		private int vertexCount = triangleCoords.Length / COORDS_PER_VERTEX;
		private int vertexStride = COORDS_PER_VERTEX * 4;
		// 4 bytes per vertex
		// Set color with red, green, blue and alpha (opacity) values
		float[] color = new float[] { 
			0.63671875f, 
			0.76953125f, 
			0.22265625f, 
			1.0f
		};

		public Triangle ()
		{
			// initialize vertex byte buffer for shape coordinates
			ByteBuffer bb = ByteBuffer.AllocateDirect (
	                // (number of coordinate values * 4 bytes per float)
				                triangleCoords.Length * 4);
			// use the device hardware's native byte order
			bb.Order (ByteOrder.NativeOrder ());

			// create a floating point buffer from the ByteBuffer
			vertexBuffer = bb.AsFloatBuffer ();
			// add the coordinates to the FloatBuffer
			vertexBuffer.Put (triangleCoords);
			// set the buffer to read the first coordinate
			vertexBuffer.Position (0);

			// prepare shaders and OpenGL program
			int vertexShader = MyGLRenderer.LoadShader (GLES30.GlVertexShader,
				                   vertexShaderCode);
			int fragmentShader = MyGLRenderer.LoadShader (GLES30.GlFragmentShader,
				                     fragmentShaderCode);

			mProgram = GLES30.GlCreateProgram ();             // create empty OpenGL Program
			GLES30.GlAttachShader (mProgram, vertexShader);   // add the vertex shader to program
			GLES30.GlAttachShader (mProgram, fragmentShader); // add the fragment shader to program
			GLES30.GlLinkProgram (mProgram);                  // create OpenGL program executables
		}

		public void Draw (float[] mvpMatrix)
		{
			// Add program to OpenGL environment
			GLES30.GlUseProgram (mProgram);

			// get handle to vertex shader's vPosition member
			mPositionHandle = GLES30.GlGetAttribLocation (mProgram, "vPosition");

			// Enable a handle to the triangle vertices
			GLES30.GlEnableVertexAttribArray (mPositionHandle);

			// Prepare the triangle coordinate data
			GLES30.GlVertexAttribPointer (mPositionHandle, COORDS_PER_VERTEX,
				GLES30.GlFloat, false,
				vertexStride, vertexBuffer);

			// get handle to fragment shader's vColor member
			mColorHandle = GLES30.GlGetUniformLocation (mProgram, "vColor");

			// Set color for drawing the triangle
			GLES30.GlUniform4fv (mColorHandle, 1, color, 0);

			// get handle to shape's transformation matrix
			mMVPMatrixHandle = GLES30.GlGetUniformLocation (mProgram, "uMVPMatrix");
			MyGLRenderer.CheckGlError ("glGetUniformLocation");

			// Apply the projection and view transformation
			GLES30.GlUniformMatrix4fv (mMVPMatrixHandle, 1, false, mvpMatrix, 0);
			MyGLRenderer.CheckGlError ("glUniformMatrix4fv");

			// Draw the triangle
			GLES30.GlDrawArrays (GLES30.GlTriangles, 0, vertexCount);

			// Disable vertex array
			GLES30.GlDisableVertexAttribArray (mPositionHandle);
		}
	}
}

