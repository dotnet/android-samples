using System;
using Android.Gms.Common.Apis;

namespace DataLayer
{
	public class InterfaceClasses
	{
		public InterfaceClasses ()
		{
		}
	}

	public class ResultCallback : Java.Lang.Object, IResultCallback
	{
		public Action<Java.Lang.Object> OnResultAction;
		public void OnResult (Java.Lang.Object result)
		{
			if (OnResultAction != null)
				OnResultAction (result);
		}
	}
}

