/*
 * Copyright (C) 2009 The Android Open Source Project
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
 * 
 */

using System;

namespace JetBoy
{
	public class Asteroid
	{
		public int AnimationIndex { get; set; }
		public int DrawY { get; set; }
		public int DrawX { get; set; }
		public bool IsExploding { get; set; }
		public bool Missed { get; set; }
		public long StartTime { get; set; }
	}
}
