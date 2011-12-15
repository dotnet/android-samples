using System;

using Android.Runtime;

namespace Mono.Samples.SanityTests {
	[Register ("mono/android/test/Adder", DoNotGenerateAcw=true)]
	public class Adder : Java.Lang.Object {

		static IntPtr class_ref = JNIEnv.FindClass ("mono/android/test/Adder");

		public Adder ()
		{
		}

		public Adder (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		protected override Type ThresholdType {
			get {return typeof (Adder);}
		}

		protected override IntPtr ThresholdClass {
			get {return class_ref;}
		}

#region Add
		static IntPtr id_add;

		[Register ("add", "(II)I", "GetAddHandler")]
		public virtual int Add (int a, int b)
		{
			if (id_add == IntPtr.Zero)
				id_add = JNIEnv.GetMethodID (class_ref, "add", "(II)I");
			if (GetType () == ThresholdType)
				return JNIEnv.CallIntMethod (Handle, id_add, new JValue (a), new JValue (b));
			return JNIEnv.CallNonvirtualIntMethod (Handle, ThresholdClass, id_add, new JValue (a), new JValue (b));
		}

#pragma warning disable 0169
		static Delegate cb_add;
		static Delegate GetAddHandler ()
		{
			if (cb_add == null)
				cb_add = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int, int, int>) n_Add);
			return cb_add;
		}

		static int n_Add (IntPtr jnienv, IntPtr lrefThis, int a, int b)
		{
			Adder __this = Java.Lang.Object.GetObject<Adder>(lrefThis, JniHandleOwnership.DoNotTransfer);
			return __this.Add (a, b);
		}
#pragma warning restore 0169
#endregion

		static IntPtr id_sadd;
		public static int Add (Adder self, int a, int b)
		{
			if (id_sadd == IntPtr.Zero)
				id_sadd = JNIEnv.GetStaticMethodID (class_ref, "add", "(Lmono/android/test/Adder;II)I");
			return JNIEnv.CallStaticIntMethod (class_ref, id_sadd,
					new JValue (JNIEnv.ToJniHandle (self)), new JValue (a), new JValue (b));
		}
	}

	class ManagedAdder : Adder {

		public override int Add (int a, int b)
		{
			return (a*2) + (b*2);
		}
	}
}

