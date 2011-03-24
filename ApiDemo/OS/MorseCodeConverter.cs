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

namespace MonoDroid.ApiDemo
{
	class MorseCodeConverter
	{
		private static long SPEED_BASE = 100;
		static long DOT = SPEED_BASE;
		static long DASH = SPEED_BASE * 3;
		static long GAP = SPEED_BASE;
		static long LETTER_GAP = SPEED_BASE * 3;
		static long WORD_GAP = SPEED_BASE * 7;

		/** The characters from 'A' to 'Z' */
		private static long[][] LETTERS = new [] {
		/* A */ new [] { DOT, GAP, DASH },
		/* B */ new [] { DASH, GAP, DOT, GAP, DOT, GAP, DOT },
		/* C */ new [] { DASH, GAP, DOT, GAP, DASH, GAP, DOT },
		/* D */ new [] { DASH, GAP, DOT, GAP, DOT },
		/* E */ new [] { DOT },
		/* F */ new [] { DOT, GAP, DOT, GAP, DASH, GAP, DOT },
		/* G */ new [] { DASH, GAP, DASH, GAP, DOT },
		/* H */ new [] { DOT, GAP, DOT, GAP, DOT, GAP, DOT },
		/* I */ new [] { DOT, GAP, DOT },
		/* J */ new [] { DOT, GAP, DASH, GAP, DASH, GAP, DASH },
		/* K */ new [] { DASH, GAP, DOT, GAP, DASH },
		/* L */ new [] { DOT, GAP, DASH, GAP, DOT, GAP, DOT },
		/* M */ new [] { DASH, GAP, DASH },
		/* N */ new [] { DASH, GAP, DOT },
		/* O */ new [] { DASH, GAP, DASH, GAP, DASH },
		/* P */ new [] { DOT, GAP, DASH, GAP, DASH, GAP, DOT },
		/* Q */ new [] { DASH, GAP, DASH, GAP, DOT, GAP, DASH },
		/* R */ new [] { DOT, GAP, DASH, GAP, DOT },
		/* S */ new [] { DOT, GAP, DOT, GAP, DOT },
		/* T */ new [] { DASH },
		/* U */ new [] { DOT, GAP, DOT, GAP, DASH },
		/* V */ new [] { DOT, GAP, DOT, GAP, DASH },
		/* W */ new [] { DOT, GAP, DASH, GAP, DASH },
		/* X */ new [] { DASH, GAP, DOT, GAP, DOT, GAP, DASH },
		/* Y */ new [] { DASH, GAP, DOT, GAP, DASH, GAP, DASH },
		/* Z */ new [] { DASH, GAP, DASH, GAP, DOT, GAP, DOT },
	};

		/** The characters from '0' to '9' */
		private static long[][] NUMBERS = new [] {
		/* 0 */ new [] { DASH, GAP, DASH, GAP, DASH, GAP, DASH, GAP, DASH },
		/* 1 */ new [] { DOT, GAP, DASH, GAP, DASH, GAP, DASH, GAP, DASH },
		/* 2 */ new [] { DOT, GAP, DOT, GAP, DASH, GAP, DASH, GAP, DASH },
		/* 3 */ new [] { DOT, GAP, DOT, GAP, DOT, GAP, DASH, GAP, DASH },
		/* 4 */ new [] { DOT, GAP, DOT, GAP, DOT, GAP, DOT, GAP, DASH },
		/* 5 */ new [] { DOT, GAP, DOT, GAP, DOT, GAP, DOT, GAP, DOT },
		/* 6 */ new [] { DASH, GAP, DOT, GAP, DOT, GAP, DOT, GAP, DOT },
		/* 7 */ new [] { DASH, GAP, DASH, GAP, DOT, GAP, DOT, GAP, DOT },
		/* 8 */ new [] { DASH, GAP, DASH, GAP, DASH, GAP, DOT, GAP, DOT },
		/* 9 */ new [] { DASH, GAP, DASH, GAP, DASH, GAP, DASH, GAP, DOT },
	};

		private static long[] ERROR_GAP = new [] { GAP };

		/** Return the pattern data for a given character */
		static long[] GetPattern (char c)
		{
			if (c >= 'A' && c <= 'Z')
				return LETTERS[c - 'A'];
			if (c >= 'a' && c <= 'z')
				return LETTERS[c - 'a'];
			if (c >= '0' && c <= '9')
				return NUMBERS[c - '0'];
			return ERROR_GAP;
		}

		public static long[] GetPattern (String str)
		{
			bool lastWasWhitespace;
			int strlen = str.Length;

			// Calculate how long our array needs to be.
			int len = 1;
			lastWasWhitespace = true;

			for (int i = 0; i < strlen; i++) {
				char c = str[i];

				if (Java.Lang.Character.IsWhitespace (c)) {
					if (!lastWasWhitespace) {
						len++;
						lastWasWhitespace = true;
					}
				} else {
					if (!lastWasWhitespace) {
						len++;
					}
					lastWasWhitespace = false;
					len += GetPattern (c).Length;
				}
			}

			// Generate the pattern array.  Note that we put an extra element of 0
			// in at the beginning, because the pattern always starts with the pause,
			// not with the vibration.
			long[] result = new long[len + 1];
			result[0] = 0;
			int pos = 1;
			lastWasWhitespace = true;

			for (int i = 0; i < strlen; i++) {
				char c = str[i];
				if (Java.Lang.Character.IsWhitespace (c)) {
					if (!lastWasWhitespace) {
						result[pos] = WORD_GAP;
						pos++;
						lastWasWhitespace = true;
					}
				} else {
					if (!lastWasWhitespace) {
						result[pos] = LETTER_GAP;
						pos++;
					}
					lastWasWhitespace = false;
					long[] letter = GetPattern (c);
					Array.Copy (letter, 0, result, pos, letter.Length);
					pos += letter.Length;
				}
			}

			return result;
		}
	}
}
