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
	[Activity (Label = "ExportAttributeTest", MainLauncher = true)]
	public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		}
		
		[Export]
		public void MyButton_OnClick (View view)
		{
			((Button)view).Text = "clicked!";
			
			Console.WriteLine ("Activity1.MyButton_OnClick: Writing into Bundle...");

			Bundle b = new Bundle ();
			var p = Parcel.Obtain ();
			b.PutSerializable ("dummy", new MySerializable ("foo"));
			b.PutParcelable ("dummy2", new MyParcelable ("bar"));
			p.WriteBundle (b);
			p.SetDataPosition (0);

			Console.WriteLine ("Activity1.MyButton_OnClick: Reading from Parcel...");
			var b2 = p.ReadBundle ();
			Console.WriteLine ("Read Bundle: {0}", b2);
			var s  = b.GetSerializable ("dummy");
			Console.WriteLine ("Read Serializable: {0}", s);
			var p2 = b.GetParcelable ("dummy2");
			Console.WriteLine ("Read Parcelable: {0}", p2);
		}
	}
	
	class MyParcelable : Object, IParcelable
	{
		[ExportField ("CREATOR")]
		static MyParcelableCreator InitializeCreator ()
		{
			Console.WriteLine ("MyParcelable.InitializeCreator");
			return new MyParcelableCreator ();
		}

		public string Value {get; private set;}

		public MyParcelable (string value)
		{
			Value = value;
		}

		#region IParcelable implementation
		public int DescribeContents ()
		{
			Console.WriteLine ("MyParcelable.DescribeContents");
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			Console.WriteLine ("MyParcelable.WriteToParcel");
			dest.WriteString (Value);
		}
		#endregion

		public override string ToString ()
		{
			return base.ToString () + " [Value=" + Value + "]";
		}
	}
	
	class MyParcelableCreator : Object, IParcelableCreator
	{
		public Object CreateFromParcel (Parcel source)
		{
			Console.WriteLine ("MyParcelableCreator.CreateFromParcel");
			return new MyParcelable (source.ReadString ());
		}

		public Object [] NewArray (int size)
		{
			Console.WriteLine ("MyParcelableCreator.NewArray");
			return new Object [size];
		}
	}
	
	class MySerializable : Object, Java.IO.ISerializable
	{
		public string Value {get; private set;}

		public MySerializable ()
		{
		}

		public MySerializable (string value)
		{
			Value = value;
		}

		[Export ("readObject", Throws = new [] {
			typeof (Java.IO.IOException),
			typeof (Java.Lang.ClassNotFoundException)})]
		private void ReadObjectDummy (Java.IO.ObjectInputStream source)
		{
			Console.WriteLine ("MySerializable.ReadObjectDummy");
			Value = source.ReadUTF ();
		}
		
		[Export ("writeObject", Throws = new [] {
			typeof (Java.IO.IOException),
			typeof (Java.Lang.ClassNotFoundException)})]
		private void WriteObjectDummy (Java.IO.ObjectOutputStream destination)
		{
			Console.WriteLine ("MySerializable.WriteObjectDummy");
			destination.WriteUTF (Value);
		}

		public override string ToString ()
		{
			return base.ToString () + "[Value=" + Value + "]";
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
