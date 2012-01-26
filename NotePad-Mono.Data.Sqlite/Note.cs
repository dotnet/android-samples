/*
 * Copyright (C) 2012 Xamarin Inc.
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

namespace Mono.Samples.Notepad
{
	class Note : Java.Lang.Object
	{
		public long Id { get; set; }
		public string Body { get; set; }
		public DateTime ModifiedTime { get; set; }

		public Note ()
		{
			Id = -1;
			Body = string.Empty;
		}

		public Note (long id, string body, DateTime modified)
		{
			Id = id;
			Body = body;
			ModifiedTime = modified;
		}

		public override string ToString ()
		{
			return ModifiedTime.ToString ();
		}
	}
}