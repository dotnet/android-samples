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

namespace Mono.Samples.Snake
{
	/**
	 * Simple class containing two integer values and a comparison function.
	 * There's probably something I should use instead, but this was quick and
	 * easy to build.
	 */
	class Coordinate
	{
		public int x;
		public int y;

		public Coordinate (int newX, int newY)
		{
			x = newX;
			y = newY;
		}

		public override bool Equals (object obj)
		{
			Coordinate other = (Coordinate)obj;

			return x == other.x && y == other.y;
		}

		public override String ToString ()
		{
			return string.Format ("Coordinate: [{0},{1}]", x, y);
		}
	}
}
