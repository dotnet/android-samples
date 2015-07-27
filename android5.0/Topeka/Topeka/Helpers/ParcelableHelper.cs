using System;

using Android.OS;

namespace Topeka.Helpers
{
	public static class ParcelableHelper
	{
		public static void WriteBoolean (Parcel dest, bool toWrite)
		{
			dest.WriteInt (toWrite ? 0 : 1);
		}

		public static bool ReadBoolean (Parcel inObj)
		{
			return inObj.ReadInt () == 0;
		}

		public static void WriteEnumValue<TEnum> (Parcel dest, TEnum e) where TEnum : struct, IConvertible
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException ("TEnum must be an enumerated type");
			dest.WriteInt (e.Ordinal ());
		}
	}
}

