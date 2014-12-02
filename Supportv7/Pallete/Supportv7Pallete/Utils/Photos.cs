using System;
using System.Collections.Generic;
using System.Linq;

namespace Supportv7Pallete.Utils
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
			Items.Add(new PhotoItem{ Name="Immersive Music Mixtape Side Two", Author = "Bassnectar", Image = Resource.Drawable.bass1});
			Items.Add(new PhotoItem{ Name="Noise vs Beauty", Author = "Bassnectar", Image =  Resource.Drawable.bass2});
			Items.Add(new PhotoItem{ Name="Immersive Music Mixtape Side Two", Author = "Bassnectar", Image = Resource.Drawable.bass3});
			Items.Add(new PhotoItem{ Name="Freestyle Mixtape", Author = "Bassnectar", Image =  Resource.Drawable.bass4});
			Items.Add(new PhotoItem{ Name="Vava Voom", Author = "Bassnectar", Image =  Resource.Drawable.bass5});
			Items.Add(new PhotoItem{ Name="Cozza Frenzy", Author = "Bassnectar", Image = Resource.Drawable.bass6});
			Items.Add(new PhotoItem{ Name="Divergent Spectrum", Author = "Bassnectar", Image =  Resource.Drawable.bass7});
			}

		public static PhotoItem GetPhoto(int id)
		{
			return Items.FirstOrDefault (i => i.Id == id);
		}
	}
}

