using System;

namespace Topeka.Helpers
{
	public class Theme
    {
        public enum Themes
        {
            topeka,
            blue,
            green,
            purple,
            red,
            yellow
        }

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
        readonly int textColorPrimaryId;
        readonly int windowBackgroundId;
        readonly int colorPrimaryId;
        readonly int styleId;
        
		public int TextPrimaryColor { get { return textColorPrimaryId; } }
		public int WindowBackgroundColor { get { return windowBackgroundId; } }
        public int PrimaryColor { get { return colorPrimaryId; } }
        public int StyleId { get { return styleId; } }

        Theme(string name, int colorPrimaryId, int windowBackgroundId, int textColorPrimaryId, int styleId)
        {
            themeName = name;

            this.textColorPrimaryId = textColorPrimaryId;
            this.windowBackgroundId = windowBackgroundId;
            this.colorPrimaryId = colorPrimaryId;
            this.styleId = styleId;
        }

        static readonly Array themes = Enum.GetValues(typeof(Themes));

        public Themes ToEnum()
        {
            foreach (Themes enumName in themes)
            {
                if (themeName == enumName.ToString())
                    return enumName;
            }
            return Themes.topeka;
        }
        
        public static Theme FromString(string value)
        {
            var result = (Themes)Enum.Parse(typeof(Themes), value);
            switch (result)
            {
                case Themes.topeka:
                    return Topeka;
                case Themes.blue:
                    return Blue;
                case Themes.green:
                    return Green;
                case Themes.purple:
                    return Purple;
                case Themes.red:
                    return Red;
                case Themes.yellow:
                    return Yellow;
            }
            return null;
        }

        public int Ordinal()
        {
            return (int)ToEnum();
        }
	}
}

