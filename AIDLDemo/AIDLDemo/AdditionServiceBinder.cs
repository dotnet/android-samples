using Android.Util;
using System;
using Com.Xamarin.Aidldemo;

namespace Xamarin.AidlDemo
{
	public class AdditionServiceBinder: IAdditionServiceStub, IAdditionService
	{
		public static readonly string Tag = "AdditionServiceBinder";
		public override int Add (int value1, int value2)
		{
			Log.Debug (Tag, "AdditionService.Add({0}, {1})", value1, value2);
			return value1 + value2;
		}

	}
}

