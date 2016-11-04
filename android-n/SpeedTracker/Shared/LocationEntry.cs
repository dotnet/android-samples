/*
 * Copyright (C) 2014 Google Inc. All Rights Reserved.
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

using Java.Util;

namespace Shared
{
	/**
	 * A class that models a GPS location point with additional information about the time that the data
	 * was obtained.
	 */
	public class LocationEntry
	{
		public double latitude;
		public double longitude;
		public Calendar calendar;
		public string day;

		public LocationEntry(Calendar calendar, double latitude, double longitude)
		{
			this.calendar = calendar;
			this.latitude = latitude;
			this.longitude = longitude;
			this.day = Utils.GetHashedDay(calendar);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var that = (LocationEntry) obj;

			return calendar.TimeInMillis == that.calendar.TimeInMillis;
		}

		public override int GetHashCode()
		{
			return calendar.GetHashCode();
		}
	}
}