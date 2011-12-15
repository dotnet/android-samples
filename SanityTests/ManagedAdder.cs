using System;

using Android.Runtime;

namespace Mono.Samples.SanityTests {
	[Register ("mono/android/test/Adder", DoNotGenerateAcw=true)]
	public class Adder : Java.Lang.Object {

		public Adder ()
		{
		}

		public Adder (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}
	}

	class ManagedAdder : Adder {
	}
}

