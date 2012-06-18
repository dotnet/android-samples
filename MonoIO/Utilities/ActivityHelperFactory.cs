using System;
using System.Collections.Generic;
using System.Text;
using Android.App;

namespace MonoIO.Utilities
{
	class ActivityHelperFactory
	{
		public static ActivityHelper Create(Activity activity)
		{
			return UIUtils.IsHoneycomb ? new ActivityHelperHoneycomb(activity) : new ActivityHelper(activity);
		}
	}
}
