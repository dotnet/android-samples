using System;
using System.Collections.Generic;
using System.Linq;

namespace AndroidLSamples.Utils
{
	public class PhotoItem
	{
		public string Name {get;set;}
		public string Author {get;set;}
		public int Image {get;set;}
		public int Id
		{
			get { return Name.GetHashCode () + Image; }
		}
	}
	public static class Photos 
	{
		public static List<PhotoItem> Items { get; set; }
		static Photos()
		{
			Items = new List<PhotoItem> ();
			Items.Add(new PhotoItem{ Name="Flying in the Light", Author = "Romain Guy", Image = Resource.Drawable.flying_in_the_light_large});
			Items.Add(new PhotoItem{ Name="Caterpillar", Author = "Romain Guy", Image =  Resource.Drawable.caterpiller});
			Items.Add(new PhotoItem{ Name="Look Me in the Eye", Author = "Romain Guy", Image = Resource.Drawable.look_me_in_the_eye});
			Items.Add(new PhotoItem{ Name="Balloons", Author = "Romain Guy", Image =  Resource.Drawable.sample2});
			Items.Add(new PhotoItem{ Name="Rainbow", Author = "Romain Guy", Image =  Resource.Drawable.rainbow});
			Items.Add(new PhotoItem{ Name="Over there", Author = "Romain Guy", Image =  Resource.Drawable.over_there});
			Items.Add(new PhotoItem{ Name="Jelly Fish 2", Author = "Romain Guy", Image =  Resource.Drawable.jelly_fish_2});
			Items.Add(new PhotoItem{ Name="Lone Pine Sunset", Author = "Romain Guy", Image =  Resource.Drawable.lone_pine_sunset});
		}

		public static PhotoItem GetPhoto(int id)
		{
			return Items.FirstOrDefault (i => i.Id == id);
		}
	}
}

