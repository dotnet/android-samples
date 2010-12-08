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
using Android.Views;

namespace JetBoy
{
	// A GameEvent subclass for key based user input. Values
	// are those used by the standard OnKey
	public class KeyGameEvent : GameEvent
	{
		public Keycode KeyCode { get; set; }
		public bool Up { get; set; }
		public KeyEvent Msg { get; set; }

		public KeyGameEvent (Keycode keyCode, bool up, KeyEvent msg)
		{
			KeyCode = keyCode;
			Up = up;
			Msg = msg;
		}
	}
}
