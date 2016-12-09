using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;

namespace BoundServiceDemo
{
	public interface IGetTimestamp
	{
		string GetFormattedTimestamp();
	}
	
}
