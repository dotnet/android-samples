using System;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;

namespace MediaBrowserService
{
	public static class ResourceHelper
	{
		public static int GetThemeColor (Context context, int attribute, int defaultColor)
		{
			int themeColor = 0;
			string packageName = context.PackageName;

			try {
				Context packageContext = context.CreatePackageContext (packageName, 0);
				ApplicationInfo applicationInfo = context.PackageManager.GetApplicationInfo (packageName, 0);
				packageContext.SetTheme (applicationInfo.Theme);

				Resources.Theme theme = packageContext.Theme;
				TypedArray ta = theme.ObtainStyledAttributes (new [] { attribute });
				themeColor = ta.GetColor (0, defaultColor);
				ta.Recycle ();
			} catch (PackageManager.NameNotFoundException e) {
				e.PrintStackTrace ();
			}

			return themeColor;
		}
	}
}

