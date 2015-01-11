using System;

namespace FragmentTransition
{
	public class Meat
	{
		public int resourceId {
			get;
			set;
		}

		public string title {
			get;
			set;
		}

		public readonly static Meat[] MEATS = new Meat[] {
			new Meat (Resource.Drawable.p1, "First"),
			new Meat (Resource.Drawable.p2, "Second"),
			new Meat (Resource.Drawable.p3, "Third"),
			new Meat (Resource.Drawable.p4, "Fourth"),
			new Meat (Resource.Drawable.p5, "Fifth"),
			new Meat (Resource.Drawable.p6, "Sixth"),
			new Meat (Resource.Drawable.p7, "Seventh"),
			new Meat (Resource.Drawable.p8, "Eighth"),
			new Meat (Resource.Drawable.p9, "Ninth"),
			new Meat (Resource.Drawable.p10, "Tenth"),
			new Meat (Resource.Drawable.p11, "Eleventh"),

		};

		public Meat (int resourceId, string title)
		{
			this.resourceId = resourceId;
			this.title = title;
		}
	}
}

