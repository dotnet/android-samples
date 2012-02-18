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

using System;
using Javax.Microedition.Khronos.Opengles;
using Java.Nio;

namespace MonoDroid.ApiDemo
{
	/**
	 * A vertex shaded cube.
	 */
	class Cube
	{
		public Cube ()
		{
			int one = 0x10000;
			var vertices = new int [] {
				-one, -one, -one,
				one, -one, -one,
				one,  one, -one,
				-one,  one, -one,
				-one, -one,  one,
				one, -one,  one,
				one,  one,  one,
				-one,  one,  one,
			};

			var colors = new int [] {
				0,    0,    0,  one,
				one,    0,    0,  one,
				one,  one,    0,  one,
				0,  one,    0,  one,
				0,    0,  one,  one,
				one,    0,  one,  one,
				one,  one,  one,  one,
				0,  one,  one,  one,
			};

			var indices = new byte [] {
				0, 4, 5,    0, 5, 1,
				1, 5, 6,    1, 6, 2,
				2, 6, 7,    2, 7, 3,
				3, 7, 4,    3, 4, 0,
				4, 7, 6,    4, 6, 5,
				3, 0, 1,    3, 1, 2
			};

			// Buffers to be passed to gl*Pointer() functions
			// must be direct, i.e., they must be placed on the
			// native heap where the garbage collector cannot
			// move them.
			//
			// Buffers with multi-byte datatypes (e.g., short, int, float)
			// must have their byte order set to native order
			
			ByteBuffer vbb = ByteBuffer.AllocateDirect (vertices.Length * 4);
			vbb.Order (ByteOrder.NativeOrder ());
			mVertexBuffer = vbb.AsIntBuffer ();
			mVertexBuffer.Put (vertices);
			mVertexBuffer.Position (0);
			
			ByteBuffer cbb = ByteBuffer.AllocateDirect (colors.Length * 4);
			cbb.Order (ByteOrder.NativeOrder ());
			mColorBuffer = cbb.AsIntBuffer ();
			mColorBuffer.Put (colors);
			mColorBuffer.Position (0);
			
			mIndexBuffer = ByteBuffer.AllocateDirect (indices.Length);
			mIndexBuffer.Put (indices);
			mIndexBuffer.Position (0);
		}

		public void Draw (IGL10 gl)
		{
			gl.GlFrontFace (GL10.GlCw);
			gl.GlVertexPointer (3, GL10.GlFixed, 0, mVertexBuffer);
			gl.GlColorPointer (4, GL10.GlFixed, 0, mColorBuffer);
			gl.GlDrawElements (GL10.GlTriangles, 36, GL10.GlUnsignedByte, mIndexBuffer);
		}

		IntBuffer   mVertexBuffer;
		IntBuffer   mColorBuffer;
		ByteBuffer  mIndexBuffer;
	}
}
