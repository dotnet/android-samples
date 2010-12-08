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
	public class Scoreboard
	{
		// total number asteroids you need to hit.
		public int HitTotal { get; set; }
		// the number of asteroids that must be destroyed
		public int SuccessThreshold { get; set; }
		// used to calculate level for mutes and trigger clip
		public int HitStreak { get; set; }

		public Scoreboard ()
		{
			SuccessThreshold = 50;
		}
	}
}
