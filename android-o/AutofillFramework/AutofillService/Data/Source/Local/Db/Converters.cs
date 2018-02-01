using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutofillService.Data.Source.Local.Db
{
	/**
	 * Type converter for Room database.
	 */
	public class Converters
	{

		/**
		* If database returns a {@link String} containing a comma delimited list of ints, this converts
		* the {@link String} to an {@link IntList}.
		*/
		//[TypeConverter]
		public IntList StoredStringToIntList(string value)
		{
			List<String> strings = value.Split("\\s*,\\s*".ToCharArray()).ToList();
			List<int> ints = strings.Select(x => Convert.ToInt32(x)).ToList();
			return new IntList(ints);
		}

		/**
		* Converts the {@link IntList} back into a String containing a comma delimited list of
		* ints.
		*/
		//[TypeConverter]
		public static String intListToStoredString(IntList list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var integer in list.ints)
			{
				stringBuilder.Append(integer).Append(",");
			}
			return stringBuilder.ToString();
		}

		/**
		* If database returns a {@link String} containing a comma delimited list of Strings, this
		* converts the {@link String} to a {@link StringList}.
		*/
		//[TypeConverter]
		public static StringList storedStringToStringList(String value)
		{
			List<String> strings = value.Split("\\s*,\\s*".ToCharArray()).ToList();
			return new StringList(strings);
		}


		/**
		* Converts the {@link StringList} back into a {@link String} containing a comma delimited
		* list of {@link String}s.
		*/
		//[TypeConverter]
		public static String stringListToStoredString(StringList list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var str in list.strings)
			{
				stringBuilder.Append(str).Append(",");
			}
			return stringBuilder.ToString();
		}

		/**
		 * Wrapper class for {@code List<Integer>} so it can work with Room type converters.
		 */
		public class IntList
		{
			public List<int> ints;

			public IntList(List<int> ints)
			{
				this.ints = ints;
			}
		}

		/**
		 * Wrapper class for {@code List<String>} so it can work with Room type converters.
		 */
		public class StringList
		{
			public List<string> strings;

			public StringList(List<string> ints)
			{
				strings = ints;
			}
		}
	}
}