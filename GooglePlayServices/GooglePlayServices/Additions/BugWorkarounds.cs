using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Android.Gms.Drives {
	public abstract partial class Metadata
	{
		// FIXME: For some reason, IFreezable<T> methods are not generated as abstract in Metadata.
		// Once this bug got fixed, this should go away (otherwise it will fail to build).
		public abstract Java.Lang.Object Freeze ();
	}
	internal partial class MetadataInvoker : Metadata
	{
		IntPtr id_freeze;
		// FIXME: For some reason, IFreezable<T> methods are not generated as abstract in Metadata.
		// Once this bug got fixed, this should go away (otherwise it will fail to build).
		public override Java.Lang.Object Freeze ()
		{
			if (id_freeze == IntPtr.Zero)
				id_freeze = JNIEnv.GetMethodID (class_ref, "freeze", "()Ljava/lang/Object;");
			return (Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod  (Handle, id_freeze), JniHandleOwnership.TransferLocalRef);
		}
	}
}
