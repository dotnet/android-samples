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
using System.Collections.Generic;

namespace ViewPagerIndicator
{
	class ActivityListItem : Java.Lang.Object
	{
		public string LaunchData { get; set; }

		public string Path { get; set; }

		public string Prefix { get; set; }

		public ActivityListItem (string prefix, string path, string launchData)
		{
			Prefix = prefix;
			Path = path;
			LaunchData = launchData;
		}

		public string Component {
			get { return LaunchData.Split (':') [1]; }
		}

		public bool IsMenuItem { get { return !IsSubMenu; } }

		public bool IsSubMenu { get { return NoPrefixPath.Contains ("/"); } }

		public string Name {
			get {
				if (IsMenuItem)
					return NoPrefixPath;

				return NoPrefixPath.Split ('/') [0];
			}
		}

		public string NoPrefixPath {
			get { return Path.Substring (Prefix.Length).TrimStart ('/'); }
		}

		public string Package {
			get { return LaunchData.Split (':') [0]; }
		}

		public bool IsInPrefix (string prefix)
		{
			return Path.StartsWith (prefix, StringComparison.InvariantCultureIgnoreCase);
		}

		public override string ToString ()
		{
			return Name;
		}

		public class NameComparer : IEqualityComparer<ActivityListItem>
		{
			#region IEqualityComparer<ActivityListItem> Members
			public bool Equals (ActivityListItem x, ActivityListItem y)
			{
				return string.Compare (x.Name, y.Name) == 0;
			}

			public int GetHashCode (ActivityListItem obj)
			{
				return obj.Name.GetHashCode ();
			}
			#endregion
		}
	}
}