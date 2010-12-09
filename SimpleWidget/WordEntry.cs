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
 */

using System;
using System.Xml.Linq;

namespace SimpleWidget
{
	class WordEntry
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Link { get; set; }

		public WordEntry ()
		{
			Title = string.Empty;
			Description = string.Empty;
			Link = string.Empty;
		}

		public static WordEntry GetWordOfTheDay ()
		{
			var entry = new WordEntry ();

			try {
				string url = "http://toolserver.org/~enwikt/wotd/";

				XDocument doc = XDocument.Load (url);

				XElement today = doc.Root.Element ("channel").Element ("item");

				entry.Title = today.Element ("title").Value;
				entry.Description = today.Element ("description").Value;
				entry.Link = today.Element ("link").Value;

				// Remove the date from the title
				entry.Title = entry.Title.Substring (entry.Title.IndexOf (':') + 1).Trim ();

			} catch (Exception ex) {
				entry.Title = "Error";
				entry.Description = ex.Message;
			}

			return entry;
		}
	}
}
