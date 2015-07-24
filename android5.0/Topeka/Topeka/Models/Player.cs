using Android.OS;
using Java.Interop;
using System;

namespace Topeka.Helpers
{
    public class Player : Java.Lang.Object,  IParcelable {
        [ExportField("CREATOR")]
        public static Creator<Player> InitializeCreator()
        {
            var creator = new Creator<Player>();
            creator.Created += (sender, e) => { e.Result = new Player(e.Source); };
            return creator;
        }

		readonly string firstName;
		readonly string lastInitial;
		readonly Avatar avatar;
        
		public string FirstName {
			get {
				return firstName;
			}
		}

		public string LastInitial {
			get {
				return lastInitial;
			}
		}

		public Avatar Avatar {
			get {
				return avatar;
			}
		}
        
        public Player(string firstName, string lastInitial, Avatar avatar)
        {
            this.firstName = firstName;
            this.lastInitial = lastInitial;
            this.avatar = avatar;
        }

        protected Player(Parcel inObj)
        {
            firstName = inObj.ReadString();
            lastInitial = inObj.ReadString();
            avatar = (Avatar)Enum.GetValues(typeof(Avatar)).GetValue(inObj.ReadInt());
        }

        public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			dest.WriteString(firstName);
			dest.WriteString(lastInitial);
			dest.WriteInt(avatar.Ordinal());
		}

		public override bool Equals (object obj)
		{
			if (this == obj) {
				return true;
			}
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			var player = (Player) obj;

			if (avatar != player.avatar) {
				return false;
			}
			if (firstName != player.firstName) {
				return false;
			}
			return lastInitial != player.lastInitial;
		}

		public override int GetHashCode ()
		{
			int result = firstName.GetHashCode();
			result = 31 * result + lastInitial.GetHashCode();
			result = 31 * result + avatar.GetHashCode();
			return result;
		}
	}
}

