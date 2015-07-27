using System;

namespace Topeka.Helpers
{
	public class Theme
	{
		public enum Themes {
			Topeka,
			Blue,
			Green,
			Purple,
			Red,
			Yellow
		}

		static readonly Array themes = Enum.GetValues (typeof(Themes));

		public static readonly Theme Topeka = new Theme ("Topeka", 
            Resource.Color.topeka_primary, 
            Resource.Color.theme_blue_background, 
            Resource.Color.theme_blue_text, 
            Resource.Style.Topeka);
		public static readonly Theme Blue = new Theme ("Blue", 
            Resource.Color.theme_blue_primary, 
            Resource.Color.theme_blue_background,
            Resource.Color.theme_blue_text,
            Resource.Style.Topeka_Blue);
		public static readonly Theme Green = new Theme ("Green", 
			Resource.Color.theme_green_primary, 
			Resource.Color.theme_green_background, 
			Resource.Color.theme_green_text, 
			Resource.Style.Topeka_Green);
		public static readonly Theme Purple = new Theme ("Purple", 
			Resource.Color.theme_purple_primary, 
			Resource.Color.theme_purple_background, 
			Resource.Color.theme_purple_text, 
			Resource.Style.Topeka_Purple);
		public static readonly Theme Red = new Theme ("Red", 
			Resource.Color.theme_red_primary, 
			Resource.Color.theme_red_background, 
			Resource.Color.theme_red_text, 
			Resource.Style.Topeka_Red);
		public static readonly Theme Yellow = new Theme ("Yellow", 
			Resource.Color.theme_yellow_primary, 
			Resource.Color.theme_yellow_background, 
			Resource.Color.theme_yellow_text, 
			Resource.Style.Topeka_Yellow);

		readonly string themeName;

		public int TextPrimaryColor {get; private set; }

		public int WindowBackgroundColor {get; private set; }

		public int PrimaryColor {get; private set; }

		public int StyleId {get; private set; }

		Theme (string name, int colorPrimaryId, int windowBackgroundId, int textColorPrimaryId, int styleId)
		{
			themeName = name;

			TextPrimaryColor = textColorPrimaryId;
			WindowBackgroundColor = windowBackgroundId;
			PrimaryColor = colorPrimaryId;
			StyleId = styleId;
		}

		public Themes ToEnum ()
		{
			foreach (Themes enumName in themes)
				if (themeName == enumName.ToString ())
					return enumName;

			return Themes.Topeka;
		}

		public static Theme FromString (string value)
		{
			Themes result;
			Enum.TryParse<Themes> (value.ToTitleCase(), out result);

			switch (result) {
			case Themes.Topeka:
				return Topeka;
			case Themes.Blue:
				return Blue;
			case Themes.Green:
				return Green;
			case Themes.Purple:
				return Purple;
			case Themes.Red:
				return Red;
			case Themes.Yellow:
				return Yellow;
			}
			return null;
		}

		public int Ordinal ()
		{
			return (int)ToEnum ();
		}
	}
}

