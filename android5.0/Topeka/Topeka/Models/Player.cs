using System;

using Android.OS;

using Java.Interop;

namespace Topeka.Helpers
{
	public class Player : Java.Lang.Object,  IParcelable
	{
		public string FirstName { get; private set; }

		public string LastInitial { get; private set; }

		public Avatar Avatar { get; private set; }

		[ExportField ("CREATOR")]
		public static Creator<Player> InitializeCreator ()
		{
			var creator = new Creator<Player> ();
			creator.Created += (sender, e) => e.Result = new Player (e.Source);
			return creator;
		}

		public Player (string firstName, string lastInitial, Avatar avatar)
		{
			FirstName = firstName;
			LastInitial = lastInitial;
			Avatar = avatar;
		}

		protected Player (Parcel inObj)
		{
			FirstName = inObj.ReadString ();
			LastInitial = inObj.ReadString ();
			// TODO: something strange
			Avatar = (Avatar)Enum.GetValues (typeof(Avatar)).GetValue (inObj.ReadInt ());
		}

		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			dest.WriteString (FirstName);
			dest.WriteString (LastInitial);
			dest.WriteInt (Avatar.Ordinal ());
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			if (obj == null || GetType () != obj.GetType ())
				return false;

			var player = (Player)obj;

			if (Avatar != player.Avatar || FirstName != player.FirstName)
				return false;

			return LastInitial != player.LastInitial;
		}

		public override int GetHashCode ()
		{
			int result = FirstName.GetHashCode ();
			result = 31 * result + LastInitial.GetHashCode ();
			result = 31 * result + Avatar.GetHashCode ();
			return result;
		}
	}
}

