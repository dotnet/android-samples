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

		static IntPtr id_ssum;
		public static int Sum (Adder self, IAdderProgress progress, params int[] values)
		{
			if (id_ssum == IntPtr.Zero)
				id_ssum = JNIEnv.GetStaticMethodID (class_ref, "sum", "(Lmono/android/test/Adder;Lmono/android/test/Adder$Progress;[I)I");
			IntPtr native_values = JNIEnv.NewArray (values);
			try {
				return JNIEnv.CallStaticIntMethod (class_ref, id_ssum,
						new JValue (JNIEnv.ToJniHandle (self)), new JValue (JNIEnv.ToJniHandle (progress)), new JValue (native_values));
			} finally {
				JNIEnv.DeleteLocalRef (native_values);
			}
		}
	}

	[Register (IAdderProgressInvoker.IAdderProgress_JniName, DoNotGenerateAcw=true)]
	public interface IAdderProgress : IJavaObject {
		[Register ("onAdd", "([III)V", "GetOnAddHandler:Mono.Samples.SanityTests.IAdderProgressInvoker, SanityTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
		void OnAdd (JavaArray<int> values, int currentIndex, int currentSum);
	}

	class IAdderProgressInvoker : Java.Lang.Object, IAdderProgress {

		public const string IAdderProgress_JniName = "mono/android/test/Adder$Progress";

		static IntPtr java_class_ref = JNIEnv.FindClass (IAdderProgress_JniName);

		IntPtr class_ref;

		public IAdderProgressInvoker (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			IntPtr lref = JNIEnv.GetObjectClass (Handle);
			class_ref = JNIEnv.NewGlobalRef (lref);
			JNIEnv.DeleteLocalRef (lref);
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		protected override Type ThresholdType {
			get {return typeof (IAdderProgressInvoker);}
		}

		protected override IntPtr ThresholdClass {
			get {return class_ref;}
		}

		public static IAdderProgress GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return new IAdderProgressInvoker (handle, transfer);
		}

#region OnAdd
		IntPtr id_onAdd;
		public void OnAdd (JavaArray<int> values, int currentIndex, int currentSum)
		{
			if (id_onAdd == IntPtr.Zero)
				id_onAdd = JNIEnv.GetMethodID (class_ref, "onAdd", "([III)V");
			JNIEnv.CallVoidMethod (Handle, id_onAdd,
					new JValue (JNIEnv.ToJniHandle (values)), new JValue (currentIndex), new JValue (currentSum));
		}

#pragma warning disable 0169
		static Delegate cb_onAdd;
		static Delegate GetOnAddHandler ()
		{
			if (cb_onAdd == null)
				cb_onAdd = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, int, int>) n_OnAdd);
			return cb_onAdd;
		}

		static void n_OnAdd (IntPtr jnienv, IntPtr lrefThis, IntPtr values, int currentIndex, int currentSum)
		{
			IAdderProgress __this = Java.Lang.Object.GetObject<IAdderProgress>(lrefThis, JniHandleOwnership.DoNotTransfer);
			using (var _values = new JavaArray<int>(values, JniHandleOwnership.DoNotTransfer)) {
				__this.OnAdd (_values, currentIndex, currentSum);
			}
		}
#pragma warning restore 0169
#endregion
	}

	class ManagedAdder : Adder {

		public override int Add (int a, int b)
		{
			return (a*2) + (b*2);
		}
	}

	class AdderProgress : Java.Lang.Object, IAdderProgress {
		public int AddInvocations;

		public void OnAdd (JavaArray<int> values, int currentIndex, int currentSum)
		{
			Android.Util.Log.Info ("*jonp*", "AdderProgress.OnAdd: Invocations={0}; currentIndex={1}; currentSum={2}", AddInvocations, currentIndex, currentSum);
			AddInvocations++;
		}
	}
}

