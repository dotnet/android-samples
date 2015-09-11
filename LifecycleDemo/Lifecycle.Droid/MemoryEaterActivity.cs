
using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Util;


namespace Lifecycle.Droid
{
	[Activity (Label = "Memory Eater")]			
	public class MemoryEaterActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug("MemoryEater", "MemoryEater Activity launched, now consuming memory");
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			// consume memory
			int[] Big = new int[1000000000];
		}
	}
}

