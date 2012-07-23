using System;

using Android.Views;
using Android.Content;
using Android.Util;
using Android.Opengl;

using Java.Nio;

namespace GLNativeES20
{
	class Square
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
		private ShortBuffer drawListBuffer;
		private int mProgram;
		private int mPositionHandle;
		private int mColorHandle;
		private int mMVPMatrixHandle;

		// number of coordinates per vertex in this array
		static int COORDS_PER_VERTEX = 3;
		static float[] squareCoords = new float[] { 
			-0.5f,  0.5f, 0.0f,   // top left
			-0.5f, -0.5f, 0.0f,   // bottom left
			0.5f, -0.5f, 0.0f,    // bottom right
			0.5f,  0.5f, 0.0f };  // top right

		private short[] drawOrder = new short[] { 
			0, 
			1, 
			2, 
			0, 
			2, 
			3
		}; // order to draw vertices

		private int vertexStride = COORDS_PER_VERTEX * 4; // 4 bytes per vertex

		// Set color with red, green, blue and alpha (opacity) values
		float[] color = new float[] { 
			0.2f, 
			0.709803922f, 
			0.898039216f, 
			1.0f
		};

		public Square ()
		{
			// initialize vertex byte buffer for shape coordinates
			ByteBuffer bb = ByteBuffer.AllocateDirect (
			// (# of coordinate values * 4 bytes per float)
			        squareCoords.Length * 4);
			bb.Order (ByteOrder.NativeOrder ());
			vertexBuffer = bb.AsFloatBuffer ();
			vertexBuffer.Put (squareCoords);
			vertexBuffer.Position (0);

			// initialize byte buffer for the draw list
			ByteBuffer dlb = ByteBuffer.AllocateDirect (
			// (# of coordinate values * 2 bytes per short)
			        drawOrder.Length * 2);
			dlb.Order (ByteOrder.NativeOrder ());
			drawListBuffer = dlb.AsShortBuffer ();
			drawListBuffer.Put (drawOrder);
			drawListBuffer.Position (0);

			// prepare shaders and OpenGL program
			int vertexShader = MyGLRenderer.LoadShader (GLES20.GlVertexShader,
			                                           vertexShaderCode);
			int fragmentShader = MyGLRenderer.LoadShader (GLES20.GlFragmentShader,
			                                             fragmentShaderCode);

			mProgram = GLES20.GlCreateProgram ();             // create empty OpenGL Program
			GLES20.GlAttachShader (mProgram, vertexShader);   // add the vertex shader to program
			GLES20.GlAttachShader (mProgram, fragmentShader); // add the fragment shader to program
			GLES20.GlLinkProgram (mProgram);                  // create OpenGL program executables
		}

		public void Draw (float[] mvpMatrix)
		{
			// Add program to OpenGL environment
			GLES20.GlUseProgram (mProgram);

			// get handle to vertex shader's vPosition member
			mPositionHandle = GLES20.GlGetAttribLocation (mProgram, "vPosition");

			// Enable a handle to the triangle vertices
			GLES20.GlEnableVertexAttribArray (mPositionHandle);

			// Prepare the triangle coordinate data
			GLES20.GlVertexAttribPointer (mPositionHandle, COORDS_PER_VERTEX,
			                             GLES20.GlFloat, false,
			                             vertexStride, vertexBuffer);

			// get handle to fragment shader's vColor member
			mColorHandle = GLES20.GlGetUniformLocation (mProgram, "vColor");

			// Set color for drawing the triangle
			GLES20.GlUniform4fv (mColorHandle, 1, color, 0);

			// get handle to shape's transformation matrix
			mMVPMatrixHandle = GLES20.GlGetUniformLocation (mProgram, "uMVPMatrix");
			MyGLRenderer.CheckGlError ("glGetUniformLocation");

			// Apply the projection and view transformation
			GLES20.GlUniformMatrix4fv (mMVPMatrixHandle, 1, false, mvpMatrix, 0);
			MyGLRenderer.CheckGlError ("glUniformMatrix4fv");

			// Draw the square
			GLES20.GlDrawElements (GLES20.GlTriangles, drawOrder.Length,
			                      GLES20.GlUnsignedShort, drawListBuffer);

			// Disable vertex array
			GLES20.GlDisableVertexAttribArray (mPositionHandle);
		}
	}
}

