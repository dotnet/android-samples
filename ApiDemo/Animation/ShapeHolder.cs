/*
 * Copyright (C) 2010 The Android Open Source Project
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
using System.Text;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace MonoDroid.ApiDemo
{
	// A data structure that holds a Shape and various properties that
	// can be used to define how the shape is drawn.
	public class ShapeHolder : Java.Lang.Object
	{
		private ShapeDrawable shape;
		private float alpha = 1f;

		public ShapeHolder (ShapeDrawable s)
		{
			shape = s;
		}

		public float Alpha { get { return alpha; } set { alpha = value; shape.SetAlpha ((int)((alpha * 255f) + .5f)); } }
		public Paint Paint { get; set; }
		public ShapeDrawable Shape { get { return shape; } }
		public float Width { get { return shape.Shape.Width; } }
		public float X { get; set; }
		public float Y { get; set; }
	}
}
