using System;
using System.Globalization;

namespace Topeka
{
	public static class StringHelper
	{
		public static string ToTitleCase(this string value)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase (value.ToLower ());
		}
	}
}

