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
	class BlogPost
	{
		public string Title { get; set; }
		public string Creator { get; set; }
		public string Link { get; set; }

		public BlogPost ()
		{
			Title = string.Empty;
			Creator = string.Empty;
			Link = string.Empty;
		}

		public static BlogPost GetBlogPost ()
		{
			var entry = new BlogPost ();

			try {
				string url = "http://blog.xamarin.com/feed/";

				XNamespace dc = "http://purl.org/dc/elements/1.1/";

				XDocument doc = XDocument.Load (url);

				XElement latest = doc.Root.Element ("channel").Element ("item");

				entry.Title = latest.Element ("title").Value;
				entry.Creator = latest.Element (dc + "creator").Value;
				entry.Link = latest.Element ("link").Value;
			} catch (Exception ex) {
				entry.Title = "Error";
				entry.Creator = ex.Message;
			}

			return entry;
		}
	}
}
