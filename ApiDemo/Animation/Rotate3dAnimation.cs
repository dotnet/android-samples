//
// Copyright (C) 2007 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using Android.Graphics;
using Android.Views.Animations;

namespace MonoDroid.ApiDemo
{
	public class Rotate3dAnimation : Animation
	{
		private float from_degrees;
		private float to_degrees;
		private float center_x;
		private float center_y;
		private float depth_z;
		private bool reverse;
		private Camera camera;

		// Creates a new 3D rotation on the Y axis. The rotation is defined by its
		// start angle and its end angle. Both angles are in degrees. The rotation
		// is performed around a center point on the 2D space, definied by a pair
		// of X and Y coordinates, called centerX and centerY. When the animation
		// starts, a translation on the Z axis (depth) is performed. The length
		// of the translation can be specified, as well as whether the translation
		// should be reversed in time.

		// @param fromDegrees the start angle of the 3D rotation
		// @param toDegrees the end angle of the 3D rotation
		// @param centerX the X center of the 3D rotation
		// @param centerY the Y center of the 3D rotation
		// @param reverse true if the translation should be reversed, false otherwise

		public Rotate3dAnimation (float fromDegrees, float toDegrees,
			float centerX, float centerY, float depthZ, bool reverse)
		{
			from_degrees = fromDegrees;
			to_degrees = toDegrees;
			center_x = centerX;
			center_y = centerY;
			depth_z = depthZ;
			this.reverse = reverse;
		}

		public override void Initialize (int width, int height, int parentWidth, int parentHeight)
		{
			base.Initialize (width, height, parentWidth, parentHeight);

			camera = new Camera ();
		}

		protected override void ApplyTransformation (float interpolatedTime, Transformation t)
		{
			float fromDegrees = from_degrees;
			float degrees = fromDegrees + ((to_degrees - fromDegrees) * interpolatedTime);

			float centerX = center_x;
			float centerY = center_y;

			Matrix matrix = t.Matrix;

			camera.Save ();

			if (reverse)
				camera.Translate (0.0f, 0.0f, depth_z * interpolatedTime);
			else
				camera.Translate (0.0f, 0.0f, depth_z * (1.0f - interpolatedTime));

			camera.RotateY (degrees);
			camera.GetMatrix (matrix);
			camera.Restore ();

			matrix.PreTranslate (-centerX, -centerY);
			matrix.PostTranslate (centerX, centerY);
		}
	}
}