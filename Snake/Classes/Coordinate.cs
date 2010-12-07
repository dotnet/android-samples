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
using System.Collections.Generic;

namespace Mono.Samples.Snake
{
	// Simple class containing two integer values and a comparison function.
	public class Coordinate
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Coordinate (int x, int y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals (object obj)
		{
			Coordinate other = (Coordinate)obj;

			return X == other.X && Y == other.Y;
		}

		public override String ToString ()
		{
			return string.Format ("Coordinate: [{0}, {1}]", X, Y);
		}

		// Given a List of coordinates, we need to flatten them into an array of
		// ints before we can stuff them into a map for flattening and storage.
		public static int[] ListToArray (List<Coordinate> list)
		{
			List<int> array = new List<int> ();

			foreach (Coordinate c in list) {
				array.Add (c.X);
				array.Add (c.Y);
			}

			return array.ToArray ();
		}

		// Given a flattened array of ordinate pairs, we reconstitute
		//  them into a List of Coordinate objects
		public static List<Coordinate> ArrayToList (int[] array)
		{
			List<Coordinate> list = new List<Coordinate> ();

			for (int index = 0; index < array.Length; index += 2)
				list.Add (new Coordinate (array[index], array[index + 1]));

			return list;
		}
	}
}
