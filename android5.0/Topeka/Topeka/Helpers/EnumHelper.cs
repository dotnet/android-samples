using System;

namespace Topeka.Helpers
{
	public static class EnumHelper
	{
		public static int Ordinal<TEnum> (this TEnum enumVal) where TEnum : struct, IConvertible
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException ("T must be an enumerated type");
			return Array.IndexOf (Enum.GetValues (enumVal.GetType ()), enumVal);
		}
	}
}