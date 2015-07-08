using System;
using Android.Content;
using Android.Content.PM;

namespace MediaBrowserService
{
	public static class ResourceHelper
	{
		public static int GetThemeColor(Context context, int attribute, int defaultColor) {
			var themeColor = 0;
			var packageName = context.PackageName;
			try {
				var packageContext = context.CreatePackageContext(packageName, 0);
				var applicationInfo =
					context.PackageManager.GetApplicationInfo(packageName, 0);
				packageContext.SetTheme(applicationInfo.Theme);
				var theme = packageContext.Theme;
				var ta = theme.ObtainStyledAttributes(new [] {attribute});
				themeColor = ta.GetColor(0, defaultColor);
				ta.Recycle();
			} catch (PackageManager.NameNotFoundException e) {
				e.PrintStackTrace();
			}
			return themeColor;
		}
	}
}

