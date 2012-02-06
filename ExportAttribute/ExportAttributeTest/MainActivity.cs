using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

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
			p.WriteBundle (b);
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
