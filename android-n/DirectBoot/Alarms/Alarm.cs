using System;

using Java.Interop;

using Android.OS;

using Org.Json;

namespace DirectBoot
{
	/**
 	* Class represents a single alarm.
 	*/
	public class Alarm : Java.Lang.Object, IComparable<Alarm>, IParcelable
	{
		public int Id { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public int Hour { get; set; }
		public int Minute { get; set; }

		public Alarm () { }

		public Alarm (Parcel inObj)
		{
			Id = inObj.ReadInt ();
			Year = inObj.ReadInt ();
			Month = inObj.ReadInt ();
			Day = inObj.ReadInt ();
			Hour = inObj.ReadInt ();
			Minute = inObj.ReadInt ();
		}

		public DateTime GetTriggerTime ()
		{
			return new DateTime (Year, Month, Day, Hour, Minute, 0);
		}

		[ExportField ("CREATOR")]
		public static AlarmCreator InitializeCreator ()
		{
			return new AlarmCreator ();
		}

		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel parcel, ParcelableWriteFlags flags)
		{
			parcel.WriteInt (Id);
			parcel.WriteInt (Year);
			parcel.WriteInt (Month);
			parcel.WriteInt (Day);
			parcel.WriteInt (Hour);
			parcel.WriteInt (Minute);
		}

		public string ToJson ()
		{
			var jsonObject = new JSONObject ();
			jsonObject.Put ("id", Id);
			jsonObject.Put ("year", Year);
			jsonObject.Put ("month", Month);
			jsonObject.Put ("day", Day);
			jsonObject.Put ("hour", Hour);
			jsonObject.Put ("minute", Minute);
			return jsonObject.ToString ();
		}

		public static Alarm FromJson (String json)
		{
			var jsonObject = new JSONObject (json);
			return new Alarm {
				Id = jsonObject.GetInt ("id"),
				Year = jsonObject.GetInt ("year"),
				Month = jsonObject.GetInt ("month"),
				Day = jsonObject.GetInt ("day"),
				Hour = jsonObject.GetInt ("hour"),
				Minute = jsonObject.GetInt ("minute")
			};
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			if (!(obj is Alarm))
				return false;

			var alarm = (Alarm)obj;
			return Id == alarm.Id && Year == alarm.Year && Month == alarm.Month && Day == alarm.Day && Hour == alarm.Hour && Minute == alarm.Minute;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public int CompareTo (Alarm other)
		{
			return GetTriggerTime ().CompareTo (other.GetTriggerTime ());
		}

		public class AlarmCreator : Java.Lang.Object, IParcelableCreator
		{
			public Java.Lang.Object CreateFromParcel (Parcel source)
			{
				return new Alarm (source);
			}

			public Java.Lang.Object [] NewArray (int size)
			{
				return new Alarm [size];
			}
		}
	}
}

