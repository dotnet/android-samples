using System;
using Android.App;
using Android.Util;
using TinyIoC;
using Android.Content;

namespace FingerprintDialog
{
	[Application]
	public class InjectedApplication : Application
	{
		static readonly string TAG = "InjectedApplication";

		public FingerprintModule FingerprintModule {get;set;}

		public override void OnCreate() {
			base.OnCreate();
			FingerprintModule = new FingerprintModule (this);
		}

		public void Inject(Context obj) {
			FingerprintModule.Context = obj;
		}
	}
}

