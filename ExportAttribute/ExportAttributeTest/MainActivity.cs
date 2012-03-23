using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Java.Interop;

using Object = Java.Lang.Object;

namespace ExportAttributeTest
{
	[Activity (Label = "M4ATest5", MainLauncher = true)]
	public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		}
		
		[Export ("MyButton_OnClick")]
		public void MyButton_OnClick (View view)
		{
			((Button)view).Text = "clicked!";
			
			Bundle b = new Bundle ();
			var p = Parcel.Obtain ();
			b.PutSerializable ("dummy", new MySerializable ());
			b.PutParcelable ("dummy2", new MyParcelable ());
			p.WriteBundle (b);
			p.SetDataPosition (0);
			var b2 = p.ReadBundle ();
			Console.WriteLine (b2);
			var p2 = b.GetParcelable ("dummy2");
			Console.WriteLine (p2);
		}
	}
	
	class MyParcelable : Object, IParcelable
	{
		[ExportField ("CREATOR")]
		static MyParcelableCreator InitializeCreator ()
		{
			Console.WriteLine ("I'm in InitializeCreator");
			return new MyParcelableCreator ();
		}

		#region IParcelable implementation
		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			Console.WriteLine ("WriteToParcel");
		}
		#endregion
	}
	
	class MyParcelableCreator : Object, IParcelableCreator
	{
		public Object CreateFromParcel (Parcel source)
		{
			Console.WriteLine ("CreateFromParcel");
			return new MyParcelable ();
		}

		public Object [] NewArray (int size)
		{
			Console.WriteLine ("NewArray");
			return new Object [0];
		}
	}
	
	class MySerializable : Object, Java.IO.ISerializable
	{
		[Export ("readObject", Throws = new [] {
			typeof (Java.IO.IOException),
			typeof (Java.Lang.ClassNotFoundException)})]
		private void ReadObjectDummy (Java.IO.ObjectInputStream source)
		{
			Console.WriteLine ("I'm in ReadObject");
		}
		
		[Export ("writeObject", Throws = new [] {
			typeof (Java.IO.IOException),
			typeof (Java.Lang.ClassNotFoundException)})]
		private void WriteObjectDummy (Java.IO.ObjectOutputStream destination)
		{
			Console.WriteLine ("I'm in WriteObject");
		}
		
		/*
		[Export ("someblah", Throws = new [] {
			typeof (Java.IO.IOException),
			typeof (Java.Lang.ClassNotFoundException)})]
		private void SomeDummyMethod (Action foo)
		{
		}
		*/
	}
}
