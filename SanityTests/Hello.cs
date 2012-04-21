#define CATCH
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.Runtime;
#if __ANDROID_7__
using Android.Service.Wallpaper;
#endif
using Android.Util;
using Android.Views;
using Javax.Net;
using Javax.Microedition.Khronos.Egl;

using Mono.Data.Sqlite;

using Path = System.IO.Path;

#if ASSEMBLY_APP
[assembly: Application (
		Debuggable=true, 
		Label="insert label here", 
		ManageSpaceActivity=typeof(Mono.Samples.SanityTests.HelloActivity))]
#endif

namespace Mono.Samples.SanityTests
{
	public class MyAsyncTask : Android.OS.AsyncTask {
		protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] values)
		{
			throw new NotImplementedException ();
		}
	}

	public class IgnoreActivity : HelloActivity, ICloneable {
		public object Clone ()
		{
			throw new NotSupportedException ();
		}
	}

	[Service]
	public class MyService : IntentService {
		
		protected override void OnHandleIntent (Intent intent)
		{
			Log.Debug ("*jonp*", "MyService.OnHandleIntent invoked!");
		}
	}

	[Activity]
	public abstract class AbstractActivity : Activity {
	}

	public class NoOverridesApp : Application {
	}

#if !ASSEMBLY_APP
	[Application (
			Debuggable=true, 
			Label="insert app label here", 
			Icon="@drawable/Icon",
#if __ANDROID_8__
			BackupAgent=typeof(Mono.Samples.SanityTests.MyBackupAgent),
#endif
			ManageSpaceActivity=typeof(Mono.Samples.SanityTests.HelloActivity),
			Name="my.StartupApp")]
#endif
	public class OnCreateApp : Application {

		public OnCreateApp (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		public override void OnCreate ()
		{
			Log.Info ("*jonp*", "OnCreateApp.OnCreate");
			base.OnCreate ();
		}
	}

	public class OnLowMemoryApp : Application {
		public override void OnLowMemory ()
		{
			Log.Info ("*jonp*", "OnLowMemoryApp.OnLowMemory");
			base.OnLowMemory ();
		}
	}

	public abstract class Database
	{
		string FileName {get; set;}

		public Database (string fileName)
		{
			FileName = fileName;
		}

		SqliteConnection GetConnection ()
		{
			var dbPath = Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.Personal),
					FileName);
			bool exists = File.Exists (dbPath);
			if (!exists) {
				SqliteConnection.CreateFile (dbPath);
			}
			var conn = new SqliteConnection ("Data Source=" + dbPath);
			if (!exists)
				OnInitDb (conn);
			return conn;
		}

		protected abstract void OnInitDb (SqliteConnection connection);

		public void WithConnection (Action<SqliteConnection> action)
		{
			var connection = GetConnection ();
			try {
				connection.Open ();
				action (connection);
			}
			finally {
				connection.Close ();
			}
		}

		public void WithCommand (Action<SqliteCommand> command)
		{
			WithConnection (conn => {
				using (var cmd = conn.CreateCommand ())
					command (cmd);
			});
		}
	}

	public class ItemsDb : Database {

		public static readonly ItemsDb Instance;

		static ItemsDb ()
		{
			if ((int)Android.OS.Build.VERSION.SdkInt < 8) {
				return;
			}

			Instance = new ItemsDb ();
		}

		public ItemsDb ()
			: this ("items.db3")
		{
		}

		public ItemsDb (string filename)
			: base (filename)
		{
		}

		protected override void OnInitDb (SqliteConnection conn)
		{
			var commands = new[]{
				"CREATE TABLE ITEMS (Key ntext, Value ntext);",
				"INSERT INTO [Items] ([Key], [Value]) VALUES ('sample', 'text')",
			};
			foreach (var cmd in commands)
				WithCommand (c => {
						c.CommandText = cmd;
						c.ExecuteNonQuery ();
				});
		}
	}

	public class MyView1 : View {
		public MyView1 (Context c)
			: base (c)
		{
		}

		// Does _not_ override View.Left (which is `final`); should not elicit a
		// compile error
		public new virtual View Left {
			get {return null;}
		}
	}

	public class MyView2 : MyView1 {
		public MyView2 (Context c)
			: base (c)
		{
		}

		// Does _not_ override View.Left (which is `final`); should not elicit a
		// compile error
		public override View Left {
			get {return null;}
		}
	}

#if __ANDROID_7__
	class CubeWallpaper : WallpaperService {
		public override WallpaperService.Engine OnCreateEngine ()
		{
			return new CubeEngine (this);
		}

		class CubeEngine : WallpaperService.Engine {
			public CubeEngine (CubeWallpaper s)
				: base (s)
			{
			}
		}
	}
#endif

	[Activity (Name="mono.samples.HelloApp", 
			Label="MfA Sanity Tests", 
			MainLauncher=true,
			ConfigurationChanges=ConfigChanges.Mcc|ConfigChanges.Mnc,
			LaunchMode=LaunchMode.Multiple,
			ScreenOrientation=ScreenOrientation.Unspecified,
			WindowSoftInputMode=SoftInput.StateUnspecified|SoftInput.AdjustUnspecified,
			Theme=null)]
	[IntentFilter (new[]{"my.custom.action"},
			Categories = new[]{"my.custom.category"})]
	[MetaData ("foo", Value="bar")]
	public class HelloActivity : LibraryActivity
	{
		[DllImport ("foo")]
		static extern void GoMonodroidGo ();

		TextView textview;

		public HelloActivity ()
		{
		}

		IB bar;

		protected override void OnCreate (Android.OS.Bundle bundle)
		{
#if MONODROID_TIMING
			Log.Info ("MonoDroid-Timing", "HelloActivity.OnCreate: time: {0}", (DateTime.Now - new DateTime (1970, 1, 1)).TotalMilliseconds);
#endif
			base.OnCreate (bundle);
			Console.WriteLine ("this is my\nstdout\nmessage! yay!");
			Console.Error.WriteLine ("this is my\nstderr\nmessage! yay!");

#if __ANDROID_11__
			// Android 11+ _really_ doesn't want you to do networking from the main thread:
			//   http://developer.android.com/reference/android/os/NetworkOnMainThreadException.html
			// I want to do so, so...
			var policy = new Android.OS.StrictMode.ThreadPolicy.Builder ()
				.PermitAll()
				.Build ();
			Android.OS.StrictMode.SetThreadPolicy (policy);
#endif

			if (typeof (OnCreateApp) != ApplicationContext.GetType ())
				throw new InvalidOperationException ("Wrong Application type created!");
			if (typeof (OnCreateApp) != Application.Context.GetType ())
				throw new InvalidOperationException ("Wrong Application.Context type created!");
			if (!object.ReferenceEquals (Application.Context, ApplicationContext))
				throw new InvalidOperationException ("Wrong Application.Context instance created!");

			var ignore = new HelloActivity ();
			var ignore2 = new CompanyAdapter (this, 0, new string [0]);

			RequestWindowFeature (WindowFeatures.NoTitle);

			StartService (new Intent (this, typeof (MyService)));

			textview = new TextView (this);
			textview.Text = "Hello MonoDroid.  Mono loves you.\n\nEmbedded\u0000Nulls";
#region BNC_635129
			textview.Tag = "2";
			if (Convert.ToString (textview.Tag) != "2")
				throw new InvalidOperationException ("Tag(\"2\") != \"2\"!");
#endregion
			TestDllImport (textview);

#region BNC_679599
			bar = new B<A> ();
			bar.Do ();
#endregion

			var tempfile = Path.GetTempFileName ();
			textview.Text += "\n\nPath.GetTempFileName()=" + tempfile;
			File.Delete (tempfile);

			int[] style = global::Mono.Samples.SanityTests.Resource.Styleable.NavBar;

			textview.Text += "\n\nEncoding.GetEncoding(1252).EncodingName=" + 
				Encoding.GetEncoding (1252).EncodingName;

			TestGzip ();
			TestArrays ();
			TestTypeConversion ();
			TestJniInvocation (textview);
			TestManualOverrides ();
			TestBnc631336 ();
			TestBnc632470 ();
			TestBnc654527(textview);
			var db = ItemsDb.Instance;
			TestSqlite (textview);
			TestProxyInstantiation ();
			TestHttps (textview);
			TestExceptions (textview);
			TestJavaManagedJavaManagedInvocation ();
			TestInputStream (textview);
			TestOutputStream (textview);
			TestStreamInvokers (textview);
			TestNonJavaObjectGenerics (textview);
			TestUrlConnectionStream (textview);
			TestConvert (textview);
			TestTimeZoneInfo (textview);
			TestAssets (textview);
			TestUdpSocket (textview);
			TestNonStaticNestedType (textview);
			TestFinalization (textview);
#if MAPS
			TestMaps (textview);
#endif
			TestNumerics (textview);
			TestManagedToJniLookup_Release (textview);

			var die = new TextView (this) {
				Text = "Added!",
			};
			WindowManager.AddView (die, new WindowManagerLayoutParams ());
			WindowManager.RemoveView (die);

			var scrollView = new ScrollView (this);
			scrollView.AddView (textview);
			SetContentView (scrollView);
			textview.Touch += OnTouch;

			ThreadPool.QueueUserWorkItem (o => {
					Log.Info ("*jonp*", "Hello from the thread pool thread!");
					RunOnUiThread (() => textview.Text += "\n\nThreadPool update!");
			});
			GC.Collect ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
		}

		void TestDllImport (TextView textview)
		{
			GoMonodroidGo ();
		}

#if MAPS
		void TestMaps (TextView textview)
		{
			var p = new Android.GoogleMaps.GeoPoint (35, 100);
			textview.Text += "\n\nnew Android.GoogleMaps.GeoPoint (35, 100)=" + p;
		}
#endif

		void TestNumerics (TextView textview)
		{
			var n = System.Numerics.Complex.Abs (new System.Numerics.Complex (1, 2));
			textview.Text += "\n\nSystem.Numerics.Complex.Abs (new System.Numerics.Complex (1, 2))=" + n;
		}

		void TestManagedToJniLookup_Release (TextView textview)
		{
			textview.SetFilters (new Android.Text.IInputFilter[]{new Android.Text.InputFilterAllCaps ()});
			textview.SetFilters (new Android.Text.IInputFilter [0]);
		}

		void TestGzip ()
		{
			const string expected = "Hello, compressed world!";
			var o = new MemoryStream ();

			using (var gzip = new StreamWriter (new GZipStream (o, CompressionMode.Compress)))
				gzip.WriteLine (expected);

			o = new MemoryStream (o.ToArray ());
			o.Position = 0;
			Log.Debug ("HelloApp", "gzip compressed data: {0}",
					string.Join (" ", o.ToArray ().Select (b => b.ToString ("x2"))));

			string result;
			using (var gzip = new StreamReader (new GZipStream (o, CompressionMode.Decompress)))
				result = gzip.ReadLine ();

			if (result != expected)
				throw new InvalidOperationException ("GZip test failed! Got '" + result + "'; expected '" + expected + "'.");
		}

		void TestArrays ()
		{
			using (var byteArray = new Java.Lang.Object (JNIEnv.NewArray (new byte[]{1,2,3}), JniHandleOwnership.TransferLocalRef)) {
				var copy = new byte [3];
				JNIEnv.CopyArray (byteArray.Handle, copy, typeof (byte));
				if (copy [0] != 1 && copy [1] != 2 && copy [2] != 3)
					throw new InvalidOperationException ("CopyArray (IntPtr, Array, Type) byte[] contents not copied!");

				copy = new byte [3];
				JNIEnv.CopyArray<byte> (byteArray.Handle, copy);
				if (copy [0] != 1 && copy [1] != 2 && copy [2] != 3)
					throw new InvalidOperationException ("CopyArray<byte> byte[] contents not copied!");

				JNIEnv.SetArrayItem (byteArray.Handle, 1, (byte) 42);

				copy = new byte [3];
				JNIEnv.CopyArray (byteArray.Handle, (Array) copy);
				if (copy [0] != 1 && copy [1] != 42 && copy [2] != 3)
					throw new InvalidOperationException ("CopyArray(IntPtr, Array) byte[] contents not copied!");

				copy = new byte[]{ 4, 5, 6 };
				JNIEnv.CopyArray (copy, byteArray.Handle);
				copy = JNIEnv.GetArray<byte>(byteArray.Handle);
				if (copy [0] != 4 && copy [1] != 5 && copy [2] != 6)
					throw new InvalidOperationException ("CopyArray(IntPtr, Array) byte[] contents not copied!");


				copy = new byte[]{ 7, 8, 9 };
				JNIEnv.CopyArray<byte> (copy, byteArray.Handle);
				copy = JNIEnv.GetArray<byte>(byteArray.Handle);
				if (copy [0] != 7 && copy [1] != 7 && copy [2] != 6)
					throw new InvalidOperationException ("CopyArray(IntPtr, Array) byte[] contents not copied!");
			}

			using (var byteArray = new Java.Lang.Object (JNIEnv.NewArray (new byte[]{1,2,3}), JniHandleOwnership.TransferLocalRef)) {
				byte[] data = JNIEnv.GetArray<byte>(byteArray.Handle);
				if (data [0] != 1 && data [1] != 2 && data [2] != 3)
					throw new InvalidOperationException ("GetArray<byte> byte[] contents not copied!");

				object[] data2 = JNIEnv.GetObjectArray (byteArray.Handle, new[]{typeof (byte), typeof (byte), typeof (byte)});
				if (((byte) data2 [0]) != 1 && ((byte) data2 [1]) != 2 && ((byte) data2 [2]) != 3)
					throw new InvalidOperationException ("GetObjectArray byte[] contents not copied!");
			}

			var states = new []{
				new[]{1, 2, 3},
				new[]{4, 5, 6},
			};
			var colors = new[]{7, 8};
			var list = new Android.Content.Res.ColorStateList (states, colors);
			if (list.GetColorForState (states [0], Color.Transparent) != 7)
				throw new InvalidOperationException ("list.GetColorForState(states [0]) != 7");
			if (list.GetColorForState (states [1], Color.Transparent) != 8)
				throw new InvalidOperationException ("list.GetColorForState(states [0]) != 7");

			using (var stringArray = new Java.Lang.Object (JNIEnv.NewArray (new[]{new[]{"a", "b"}, new[]{"c", "d"}}), JniHandleOwnership.TransferLocalRef)) {
				string[][] values = JNIEnv.GetArray<string[]>(stringArray.Handle);
				if (values [0][0] != "a" && values [0][1] != "b" && values [1][0] != "c" && values [1][1] != "d")
					throw new InvalidOperationException ("GetArray<string[]> contents not copied!");

				values = new[]{new string [2], new string [2]};
				JNIEnv.CopyArray (stringArray.Handle, values);
				if (values [0][0] != "a" && values [0][1] != "b" && values [1][0] != "c" && values [1][1] != "d")
					throw new InvalidOperationException ("GetArray<string[]> contents not copied!");
			}

			using (var objectArray = new Java.Lang.Object (JNIEnv.NewArray (
							new Java.Lang.Object[]{this, 42L, "string"}, typeof (Java.Lang.Object)), JniHandleOwnership.TransferLocalRef)) {
				object[] values = JNIEnv.GetObjectArray (objectArray.Handle, new[]{typeof(Context), typeof (int)});
				if (values.Length != 3)
					throw new InvalidOperationException ("GetObjectArray returned " + values.Length + " items!");
				if (!object.ReferenceEquals (values [0], this))
					throw new InvalidOperationException ("GetObjectArray wrong values[0]! " + values [0] +
							" [" + values [0].GetType ().FullName + "]" + " Handle=" + ((IJavaObject) values [0]).Handle +
							"; this.Handle=" + this.Handle +
							" ReferenceEquals=" + object.ReferenceEquals (values [0], this));
				if (!(values [1] is int))
					throw new InvalidOperationException ("GetObjectArray wrong values[1]!");
				if (42 != (int) values [1])
					throw new InvalidOperationException ("GetObjectArray wrong values[1]!");
				if ("string" != values [2].ToString ())
					throw new InvalidOperationException ("GetObjectArray wrong values[2]!");
			}
		}

		void TestTypeConversion ()
		{
			// Sanity checking to ensure that our binding stays correct.
			var v = ServerSocketFactory.Default;
			Log.Info ("HelloApp", "ServerSocketFactory.Default={0}", v);
			var egl = EGLContext.EGL;
			Log.Info ("HelloApp", "EGLContext.EGL={0}", egl);
			IEGL10 egl10 = egl.JavaCast<IEGL10>();
			Log.Info ("HelloApp", "(IEGL10) EGLContext.EGL={0}", egl10.GetType().FullName);

			IntPtr lref = JNIEnv.NewString ("Hello, world!");
			using (Java.Lang.Object s = Java.Lang.Object.GetObject<Java.Lang.Object>(lref, JniHandleOwnership.TransferLocalRef)) {
				if (s.GetType () != typeof (Java.Lang.String))
					throw new InvalidOperationException ("Object.GetObject<T>() needs to return a Java.Lang.String!");
			}

			lref = JNIEnv.CreateInstance ("android/gesture/Gesture", "()V");
			using (Java.Lang.Object g = Java.Lang.Object.GetObject<Java.Lang.Object>(lref, JniHandleOwnership.TransferLocalRef))
				if (g.GetType () != typeof (Android.Gestures.Gesture))
					throw new InvalidOperationException ("Object.GetObject<T>() needs to return a Android.Gestures.Gesture!");
		}

		void TestJniInvocation (TextView textview)
		{
			IntPtr Adder = JNIEnv.FindClass ("mono/android/test/Adder");
			if (Adder == IntPtr.Zero)
				throw new InvalidOperationException ("Couldn't find mono.android.test.Adder");
			IntPtr Adder_ctor = JNIEnv.GetMethodID (Adder, "<init>", "()V");
			if (Adder_ctor == IntPtr.Zero)
				throw new InvalidOperationException ("Couldn't find mono.android.test.Adder.#ctor()");
			IntPtr Adder_add = JNIEnv.GetMethodID (Adder, "add", "(II)I");
			if (Adder_add == IntPtr.Zero)
				throw new InvalidOperationException ("Couldn't find mono.android.test.Adder.add(int,int)");
			IntPtr instance = JNIEnv.NewObject (Adder, Adder_ctor);
			int result = JNIEnv.CallIntMethod (instance, Adder_add, new JValue (2), new JValue (3));
			textview.Text += "\n\nnew Adder().add(2,3)=" + result;

			var boundAdder = new Adder (instance, JniHandleOwnership.DoNotTransfer);
			if (boundAdder.Add (3, 4) != 7)
				throw new InvalidOperationException ("Add(3,4) != 7!");
			JNIEnv.DeleteLocalRef (instance);
		}

		void TestManualOverrides ()
		{
			var adder = new Adder ();
			Console.WriteLine ("Adder Class: {0}", adder.Class);

			var managedAdder = new ManagedAdder ();
			int result = Adder.Add (managedAdder, 3, 4);
			if (result != 14)
				throw new InvalidOperationException ("ManagedAdder.Add(3, 4) != 14!");

			var progress = new AdderProgress ();
			int sum = Adder.Sum (adder, progress, 1, 2, 3);
			if (sum != 6)
				throw new InvalidOperationException ("Adder.Sum(adder, 1, 2, 3) != 6! Was: " + sum + ".");
			if (progress.AddInvocations != 3)
				throw new InvalidOperationException ("Adder.Sum(adder, 1, 2, 3) didn't invoke progress 3 times! Was: " + progress.AddInvocations + ".");

			progress.AddInvocations = 0;
			sum = Adder.Sum (managedAdder, progress, 6, 7);
			if (sum != 38)
				throw new InvalidOperationException ("Adder.Sum(managedAdder, 6, 7) != 38! Was: " + sum + ".");
			if (progress.AddInvocations != 2)
				throw new InvalidOperationException ("Adder.Sum(adder, 6, 7) didn't invoke progress 2 times! Was: " + progress.AddInvocations + ".");

			IntPtr javaDefaultProgress = JNIEnv.CreateInstance ("mono/android/test/Adder$DefaultProgress", "()V");
			var progress2 = Java.Lang.Object.GetObject<IAdderProgress>(javaDefaultProgress, JniHandleOwnership.TransferLocalRef);
			Console.WriteLine ("progress2 MCW: {0}", progress2.GetType ().FullName);

			javaDefaultProgress = JNIEnv.CreateInstance ("mono/android/test/Adder$DefaultProgress", "()V");
			var progress3 = new Java.Lang.Object (javaDefaultProgress, JniHandleOwnership.TransferLocalRef)
				.JavaCast<IAdderProgress> ();
			Console.WriteLine ("progress3 MCW: {0}", progress3.GetType ().FullName);
		}

		void TestBnc631336 ()
		{
			var pm = PackageManager;
			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.ActionDefault);
			var list = pm.QueryIntentActivities (mainIntent, 0);
		}

		void TestBnc654527 (TextView textview)
		{
			var dict = new DatabaseVersionMap ();
			var map = dict.JavaCast<Java.Util.IMap>();
			map.Put (new Java.Lang.Integer (42), new JavaList<string> { "Foo" });
			textview.Text += "\n\nBNC_654527: dict[42][0]=" + dict [42][0];
		}

		void TestSqlite (TextView textview)
		{
			if (ItemsDb.Instance == null) {
				textview.Text += "\n\nSQLite is not supported on this platform.";
				return;
			}
			textview.Text += "\n\nSQLite DB contents:";
			ItemsDb.Instance.WithCommand (c => {
					c.CommandText = "SELECT [Key], [Value] FROM [Items]";
					var r = c.ExecuteReader ();
					while (r.Read()) {
						textview.Text += string.Format ("\n\tKey={0}; Value={1}",
							r ["Key"].ToString (), r ["Value"].ToString ());
					}
			});
		}

		void TestProxyInstantiation ()
		{
			var a = new MyCustomGenericType<char> ();
			var b = new MyCustomGenericType<char, int> ();
			var c = new MyCustomGenericType<char, int, long> ();
			var d = new OrderAdapter (this, 0, new[]{"hi"});
			var e = new StringArrayAdapter (this, 0);
		}

		void TestHttps (TextView textview)
		{
			try {
				string url = "https://encrypted.google.com/";
				var request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = "GET";
				var response = (HttpWebResponse) request.GetResponse ();
				int len = 0;
				using (var _r = new StreamReader (response.GetResponseStream ())) {
					char[] buf = new char [4096];
					int n;
					while ((n = _r.Read (buf, 0, buf.Length)) > 0) {
						/* ignore; we just want to make sure we can read */
						len += n;
					}
				}
				textview.Text += "\n\nHTTPS test succeeded! Read " + len + " bytes.";
			}
			catch (Exception e) {
				Log.Info ("*jonp*", "https exception! {0}", e.ToString ());
				textview.Text += "\n\nHTTPS test failed: " + e.Message;
			}
		}

		void TestExceptions (TextView textview)
		{
#if CATCH
			try {
#endif
#if __ANDROID_7__
				Log.Info ("HelloApp", "calling Drawable.SetAlpha...");
				WallpaperManager manager = WallpaperManager.GetInstance (this);
				var drawable = manager.FastDrawable;
				Log.Info ("HelloApp", "drawable? {0}", (drawable != null));
				drawable.SetAlpha (0);
				Log.Info ("HelloApp", "after Drawable.SetAlpha?!; should not be reached...");
#endif
#if CATCH
			}
			catch (Exception e) {
				Log.Info ("HelloApp", "Yay, exception caught!" + e);
				textview.Text += "\n\nEXPECTED EXCEPTION:\n" + e.ToString ();
			}
#endif
		}

		void TestJavaManagedJavaManagedInvocation ()
		{
			var menu = new MyMenu ();
			bool b = OnPreparePanel ((int) WindowFeatures.OptionsPanel, null, menu);
			// b is false because it returns menu.HasVisibleItems
			if (!b && !menu.HasVisibleItemsWasInvoked)
				throw new InvalidOperationException ("MyMenu.HasVisibleItems was not invoked!");
		}

		void TestBnc632470 ()
		{
			MyView mv = new MyView (this);
		}

		void TestInputStream (TextView textview)
		{
			InputStreamAdapter jis = new InputStreamAdapter (new System.IO.MemoryStream (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }));
			byte[] buffer = new byte[10];
			int cnt = jis.Read (buffer);
			bool success = cnt == 8;
			for (int i = 0; i < cnt; i++)
				if (buffer[i] != (byte)i)
					success = false;
			textview.Text += "\n\nTestInputStream round trip success: " + success;
		}

		void TestOutputStream (TextView textview)
		{
			bool success = true;

			MemoryStream ms = new MemoryStream ();
			OutputStreamAdapter osa = new OutputStreamAdapter (ms);

			osa.Write (60);
			osa.Write (65);
			osa.Write (70);

			ms.Position = 0;

			if (ms.ReadByte () != 60 || ms.ReadByte () != 65 || ms.ReadByte () != 70)
				success = false;

			ms.Close ();
			osa.Close ();

			textview.Text += "\n\nOutputStreamAdapter round trip success: " + success;
		}

		void TestStreamInvokers (TextView textview)
		{
			bool success = true;
			var test_string = "hello";

			Stream osa = OpenFileOutput ("test.txt", FileCreationMode.Private);
			var write_bytes = System.Text.Encoding.ASCII.GetBytes (test_string);

			osa.Write (write_bytes, 0, write_bytes.Length);
			osa.Close ();

			Stream isa = OpenFileInput ("test.txt");
			var read_bytes = new byte[5];

			int count = isa.Read (read_bytes, 0, read_bytes.Length);

			if (count != test_string.Length)
				success = false;
			if (System.Text.Encoding.ASCII.GetString (read_bytes) != test_string)
				success = false;

			textview.Text += "\n\nStreamInvoker round trip success: " + success;
		}

		class NonJavaObject
		{

			public override string ToString ()
			{
				return "This object is Sooooo not on java.";
			}
		}

		void TestNonJavaObjectGenerics (TextView textview)
		{
			var adapter = new Android.Widget.ArrayAdapter<NonJavaObject> (this, textview.Id);
			var item = new NonJavaObject ();
			adapter.Add (item);
			textview.Text += "\n\n" + adapter.GetItem (0).ToString ();

			var coll = new JavaDictionary<NonJavaObject, NonJavaObject> ();
			coll.Add (item, item);
			if (!object.ReferenceEquals (item, coll [item]))
				throw new InvalidOperationException ("Unable to lookup non-java.lang.Object item in JavaDictionary!");

#if BXC_2147
			Log.Info ("*jonp*", "A1");
			var jl1 = new Android.Runtime.JavaList<object>();
			Log.Info ("*jonp*", "A2");
			var v1  = new Dictionary<string, object>();
			Log.Info ("*jonp*", "A3");
			v1.Add ("szLongName", "Bla.pdf");
			Log.Info ("*jonp*", "A4");
			jl1.Add (v1);
			Log.Info ("*jonp*", "A5");
			var vo1 = jl1 [0];
			Log.Info ("*jonp*", "A6={0}", vo1);

			var jl2 = new Android.Runtime.JavaList ();
			Log.Info ("*jonp*", "B2");
			var v2  = new Dictionary<string, object>();
			Log.Info ("*jonp*", "B3");
			v2.Add ("szLongName", "Bla.pdf");
			Log.Info ("*jonp*", "B4");
			jl2.Add (v2);
			Log.Info ("*jonp*", "B5");
			var _vo2 = jl2 [0]; // Exception
			Log.Info ("*jonp*", "B6={0} [{1}]", _vo2, _vo2 != null ? _vo2.GetType ().FullName : "<null>");
			var vo2 = (Dictionary<string, object>) jl2 [0]; // Exception
			Log.Info ("*jonp*", "B7={0}", vo2);
#endif
		}

		void TestUrlConnectionStream (TextView textview)
		{
			Java.Net.URL url = new Java.Net.URL("http://www.google.pt/logos/classicplus.png");
        		Java.Net.URLConnection urlConnection = url.OpenConnection();
			Android.Graphics.BitmapFactory.DecodeStream (urlConnection.InputStream);
			textview.Text += "\n\nOpened Url connection stream and loaded image";
		}
 
		static void AssertEqual<T>(T expected, T actual)
			where T : IEquatable<T>
		{
			if (!expected.Equals (actual))
				throw new InvalidOperationException ("Expected " + expected + "; actual " + actual);
		}

		void TestConvert (TextView textview)
		{
			AssertEqual (true,  Convert.ToBoolean (Java.Lang.Boolean.ValueOf (true)));
			AssertEqual (1L,    Convert.ToInt32 (Java.Lang.Long.ValueOf (1L)));
			AssertEqual (2,     Convert.ToInt32 (Java.Lang.Integer.ValueOf (2)));
			AssertEqual (3.0,   Convert.ToDouble (Java.Lang.Double.ValueOf (3.0)));
			AssertEqual (4.0f,  Convert.ToSingle (Java.Lang.Float.ValueOf (4.0f)));
			AssertEqual ('d',   Convert.ToChar (Java.Lang.Character.ValueOf ('d')));
		}

		void TestTimeZoneInfo (TextView textview)
		{
			textview.Text += "\n\nLocal Time Zone: " + TimeZoneInfo.Local.ToString ();
			textview.Text += "\n\nUTC Time Zone: " + TimeZoneInfo.Utc.ToString ();
			Action<TimeZoneInfo> p = tz => {
				textview.Text += string.Format ("\n\nDisplayName: {0}:\n\tStandardName: {1}\n\tDaylightName: {2}\n\tId: {3}\n\tBaseUtfOffset: {4}\n\tSupportsDaylightSavingTime={5}",
						tz.DisplayName, tz.StandardName, tz.DaylightName, tz.Id, tz.BaseUtcOffset, tz.SupportsDaylightSavingTime);
			};
			p (TimeZoneInfo.Local);
			p (TimeZoneInfo.Utc);
			p (TimeZoneInfo.FindSystemTimeZoneById ("GMT-5"));
		}
		
		void TestAssets (TextView textiew)
		{
      Action<string> comp = actual => {
        var expected =
          "Hello out there in TV land!\n" +
          "Hello out there in TV land!\n" +
          "Hello out there in TV land!\n" +
          "Hello out there in TV land!\n" +
          "Hello out there in TV land!\n";
        if (string.CompareOrdinal (actual, expected) != 0)
          throw new InvalidOperationException (
            string.Format ("Invalid asset contents for Assets/Hello.txt!  Expected: '{0}'; Actual: '{1}'", expected, actual));
      };
			using (var r = new StreamReader (Assets.Open ("Hello.txt"))) {
				comp (r.ReadToEnd ());
			}
			using (var r = new StreamReader (Assets.Open ("Subdir/HelloWorld.txt"))) {
				comp (r.ReadToEnd ());
			}
		}

		void TestUdpSocket (TextView textview)
		{
			int Port = 9595;

			var server = new UdpClient(Port);
			server.BeginReceive(result => {
					IPEndPoint sender = null;
					var data = server.EndReceive(result, ref sender);
					var value = Encoding.Unicode.GetString (data);
					if (value != "hello there!")
						throw new InvalidOperationException ("UDP data transfer failed!");
					RunOnUiThread (() => textview.Text += "\n\nRead data from UDP: " + value);
					server.Close ();
			}, null);

			using (var client = new UdpClient()) {
				var bytes = Encoding.Unicode.GetBytes("hello there!");
				client.Send(
						bytes,
						bytes.Length,
						new IPEndPoint(IPAddress.Loopback, Port));
			}
		}

		void TestNonStaticNestedType (TextView textview)
		{
#if __ANDROID_7__
			var wallpaper = new CubeWallpaper ();
			var engine    = wallpaper.OnCreateEngine ();
			var engine2   = new WallpaperService.Engine (wallpaper);
			textview.Text += "\n\nAble to create non-static nested type from managed code.";

			IntPtr TestCubeEngine                   = JNIEnv.FindClass ("mono/android/test/TestCubeEngine");
			IntPtr TestCubeEngine_ctor              = JNIEnv.GetMethodID (TestCubeEngine, "<init>", "()V");
			IntPtr instance                         = JNIEnv.NewObject (TestCubeEngine, TestCubeEngine_ctor);
			IntPtr TestCubeEngine_createCubeEngine  = JNIEnv.GetMethodID (TestCubeEngine, "createCubeEngine", "()V");
			JNIEnv.CallVoidMethod (instance, TestCubeEngine_createCubeEngine);

			textview.Text += "\n\nAble to create non-static nested type from javacode.";
#endif
		}


		class SimpleObj : Java.Lang.Object
		{
			WeakReference wref = new WeakReference (new object (), false);
			public int fin_count;
			~SimpleObj () {
				Log.Info ("kumpera", "FINALIZED!");
				fin_count++;
			}
			public bool CheckRef () {
				return wref.Target != null;
			}
		}
		
		void TestFinalization (TextView textview) {
			Log.Info ("kumpera", "START");
			IntPtr gref = IntPtr.Zero;

			Action x = () => {
				gref = JNIEnv.NewGlobalRef (new SimpleObj ().Handle);
				if (gref == IntPtr.Zero)
					throw new InvalidOperationException ("Couldn not create a GREF to my obj");
			};

			var r = x.BeginInvoke (null, null);
			r.AsyncWaitHandle.WaitOne ();

			GC.Collect ();
			GC.WaitForPendingFinalizers ();

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.WaitForPendingFinalizers ();

			var obj = Java.Lang.Object.GetObject <SimpleObj> (gref, JniHandleOwnership.DoNotTransfer);
			AssertEqual (false, obj.CheckRef ());
			AssertEqual (0, obj.fin_count);
		}

		private class MyView : View {

			public MyView (Context context)
				: base (context)
			{
			}
		}

		void OnTouch (object sender, View.TouchEventArgs e)
		{
			Log.Info ("HelloApp", "OnTouchListener.OnTouch: sender={0} [{1}]; args.E={2} [{3}]; args.V={4} [{5}]", 
					sender, sender == null ? "<null>" : sender.GetType ().FullName,
					e.E, e.E == null ? "<null>" : e.E.GetType ().FullName,
					e.V, e.V == null ? "<null>" : e.V.GetType ().FullName);
		}
	}

#region BNC_633675
	class OrderAdapter : Android.Widget.ArrayAdapter {
		private System.Collections.IList items;

		public OrderAdapter (Context context, int textViewResourceId, System.Collections.IList items)
			: base (context, textViewResourceId, items)
		{
			this.items = items;
		}
	}
#endregion

#region BNC_636465
	class StringArrayAdapter : Android.Widget.ArrayAdapter<string> {
		public StringArrayAdapter (Context context, int textResourceId)
			: base (context, textResourceId)
		{
		}
	}

	class MyCustomGenericType<T> : Java.Lang.Object {}
	class MyCustomGenericType<T1, T2> : MyCustomGenericType<T1> {}
	class MyCustomGenericType<T1, T2, T3> : MyCustomGenericType<T1, T2> {}
#endregion

#region BNC_638998
	public class List1Adapter : BaseAdapter {
		Context context;

		public List1Adapter(Context c) : base() //base(c, -1)
		{
			this.context = c;
			this.items = new List<KeyValuePair<string, string>>();
			this.items.Add(new KeyValuePair<string, string>("One", "More thing..."));
			this.items.Add(new KeyValuePair<string, string>("Two", "Spoons of Sugar"));
			this.items.Add(new KeyValuePair<string, string>("Three", "Is a crowd"));
			this.items.Add(new KeyValuePair<string, string>("Four", "Square"));
			this.items.Add(new KeyValuePair<string, string>("Five", "Pipers piping"));
		}

		public List<KeyValuePair<string, string>> items;

		public override int Count
		{
			get { return this.items.Count; }
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return items[position].Key;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			return null;
		}
	}
#endregion

	[Register ("my.Menu")]
	class MyMenu : Java.Lang.Object, IMenu {
		public MyMenu ()
		{
		}

		public bool HasVisibleItems {
			get {
				HasVisibleItemsWasInvoked = true;
				return false;
			}
		}

		internal bool HasVisibleItemsWasInvoked;

		public IMenuItem Add (Java.Lang.ICharSequence title) {throw new NotImplementedException ();}
		public IMenuItem Add (int titleRes) {throw new NotImplementedException ();}
		public Android.Views.IMenuItem Add (int groupId, int itemId, int order, Java.Lang.ICharSequence title) {throw new NotSupportedException ();}
		public Android.Views.IMenuItem Add (int groupId, int itemId, int order, int titleRes) {throw new NotSupportedException ();}
		public int AddIntentOptions (int groupId, int itemId, int order, Android.Content.ComponentName caller, Android.Content.Intent[] specifics, Android.Content.Intent intent, MenuAppendFlags flags, Android.Views.IMenuItem[] outSpecificItems) {throw new NotSupportedException ();}
		public Android.Views.ISubMenu AddSubMenu (Java.Lang.ICharSequence title) {throw new NotSupportedException ();}
		public Android.Views.ISubMenu AddSubMenu (int titleRes) {throw new NotSupportedException ();}
		public Android.Views.ISubMenu AddSubMenu (int groupId, int itemId, int order, Java.Lang.ICharSequence title) {throw new NotSupportedException ();}
		public Android.Views.ISubMenu AddSubMenu (int groupId, int itemId, int order, int titleRes) {throw new NotSupportedException ();}
		public void Clear () {throw new NotSupportedException ();}
		public void Close () {throw new NotSupportedException ();}
		public Android.Views.IMenuItem FindItem (int id) {throw new NotSupportedException ();}
		public Android.Views.IMenuItem GetItem (int index) {throw new NotSupportedException ();}
		public bool IsShortcutKey (Keycode keyCode, Android.Views.KeyEvent e) {throw new NotSupportedException ();}
		public bool PerformIdentifierAction (int id, MenuPerformFlags flags) {throw new NotSupportedException ();}
		public bool PerformShortcut (Keycode keyCode, Android.Views.KeyEvent e, Android.Views.MenuPerformFlags flags) {throw new NotSupportedException ();}
		public void RemoveGroup (int groupId) {throw new NotSupportedException ();}
		public void RemoveItem (int id) {throw new NotSupportedException ();}
		public void SetGroupCheckable (int group, bool checkable, bool exclusive) {throw new NotSupportedException ();}
		public void SetGroupEnabled (int group, bool enabled) {throw new NotSupportedException ();}
		public void SetGroupVisible (int group, bool visible) {throw new NotSupportedException ();}
		public void SetQwertyMode (bool isQwerty) {throw new NotSupportedException ();}
		public int Size () {throw new NotSupportedException ();}
	}

#region BNC_654527
	class DatabaseVersionMap : JavaDictionary<int, JavaList<string>> {
		public DatabaseVersionMap ()
		{
		}

#region ACW ctor generation
		// enums; JNI Sig: (I)V
		public DatabaseVersionMap (Android.App.Result e)
		{
		}

		// MCW type; JNI Sig: ([Landroid/app/Activity;)V
		public DatabaseVersionMap (Android.App.Activity[] activities)
		{
		}

		// managed ref type; skip constructor
		public DatabaseVersionMap (System.AppDomain domain)
		{
		}

		// managed value type; skip constructor
		public DatabaseVersionMap (DateTime time)
		{
		}
#endregion
	}
#endregion

#region BNC_675179
	public class CompanyAdapter : ArrayAdapter<string>, ISpinnerAdapter {

		public CompanyAdapter (Context context, int textViewResourceId, string[] value)
			: base (context, textViewResourceId, value)
		{
		}

		public override View GetView (int p, View v, ViewGroup parent)
		{
			throw new NotImplementedException ();
		}
	}
#endregion
}

