using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace UiTest
{
	[TestFixture]
	public class Tests
	{
		AndroidApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			// TODO: If the Android app being tested is included in the solution then open
			// the Unit Tests window, right click Test Apps, select Add App Project
			// and select the app projects that should be tested.
			app = ConfigureApp
				.Android.ApkFile("/Users/abhishek/Desktop/abhi/monodroid-samples/ViewPagerIndicator/ViewPagerIndicator/bin/Release/ViewPagerIndicator.ViewPagerIndicator-Signed.apk")
				// TODO: Update this path to point to your Android app and uncomment the
				// code if the app is not included in the solution.
				//.ApkFile ("../../../Android/bin/Debug/UITestsAndroid.apk")
				.StartApp();
		}

		[Test]
		public void AppLaunches()
		{
			//app.Repl();
		}

		[Test, TestCaseSource("UiInitialElementList")]
		public void CheckUiElement(string classname, string uiElementText)
		{
			app.Screenshot("Application Started");
			Assert.IsTrue(app.Query(x => x.Class(classname).Text(uiElementText)).Any(), uiElementText + " was not found");
			app.Screenshot("First screen.");
		}

		[Test, TestCaseSource("UiInitialElementList")]
		public void TapUiElement(string classname, string uiElementText)
		{
			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class(classname).Text(uiElementText));

			}, "Unable to tap Element."
				  );
		}

		[Test, TestCaseSource("CirclesUIElement")]
		public void CheckCirclesDefaultElement(string classname, string uiElementText)
		{
			//app.Repl();
			app.Tap(x => x.Class("android.widget.TextView").Text("Circles"));

			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class("android.widget.TextView").Text(uiElementText));

			}, "Unable to tap Element."
				  );
			app.Back();
			app = null;
		}

		[Test, TestCaseSource("UiTabsElementList")]
		public void TapTabsUIElement(string classname, string uiElementText)
		{
			//app.Repl();
			app.Tap(x => x.Class("android.widget.TextView").Text("Tabs"));
			app.Tap(x => x.Class("android.widget.TextView").Text("Default"));

			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class(classname).Text(uiElementText));

			}, "Unable to tap Element."
							   );
			app.Back();
			app = null;
		}



		[Test, TestCaseSource("UiTitlesElementList")]
		public void TapTitlesUIElement(string classname, string uiElementText)
		{
			//app.Repl();
			app.Tap(x => x.Class("android.widget.TextView").Text("Titles"));

			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class(classname).Text(uiElementText));

			}, "Unable to tap Element."
							   );
			app.Back();
			app.Back();
			app = null;
		}


		[Test, TestCaseSource("CirclesUIElement")]
		public void TapCircles(string classname, string uiElementText)
		{
			app.Tap(x => x.Class("android.widget.TextView").Text("Circles"));
			app.Tap(x => x.Class(classname).Text(uiElementText));

			if (uiElementText == "Default")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Initial Page")
			{
				app.ScrollLeft();
				app.ScrollLeft();
				app.ScrollLeft();
				app.Back();
				app = null;

			}
			else if (uiElementText == "Snap")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}

			else if (uiElementText == "Styled (via layout")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Styled (via methods)")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Styled (via theme)")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "With Listener")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
		}

		[Test, TestCaseSource("TsbsDefaultUIElement")]
		public void TapTabsDefault(string classname, string uiElementText)
		{
			//app.Repl();
			app.Tap(x => x.Class("android.widget.TextView").Text("Tabs"));
			app.Tap(x => x.Class("android.widget.TextView").Text("Default"));

			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class(classname).Text(uiElementText));

			}, "Unable to tap Element."
							   );
			app.Back();
			app = null;
		}

		[Test, TestCaseSource("TsbsDefaultUIElement")]
		public void TapTabsStyled(string classname, string uiElementText)
		{
			app.Tap(x => x.Class("android.widget.TextView").Text("Tabs"));
			app.Tap(x => x.Class("android.widget.TextView").Text("Styled"));
			Assert.DoesNotThrow(() =>
			{
				app.Tap(x => x.Class(classname).Text(uiElementText));

			}, "Unable to tap Element."
							   );
			app.Back();
			app.Back();
			app = null;
		}



		[Test, TestCaseSource("UiTitlesElementList")]
		public void TapTitles(string classname, string uiElementText)
		{
			app.Tap(x => x.Class("android.widget.TextView").Text("Titles"));
			app.Tap(x => x.Class(classname).Text(uiElementText));

			if (uiElementText == "Center Click Listener")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}

			else if (uiElementText == "Default")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Initial Page")
			{
				app.ScrollLeft();
				app.ScrollLeft();
				app.ScrollLeft();
				app.Back();
				app = null;

			}
			else if (uiElementText == "Snap")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}

			else if (uiElementText == "Styled (via layout")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Styled (via methods)")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "Styled (via theme)")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}

			else if (uiElementText == "Triangle Style")
			{

				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
			else if (uiElementText == "With Listener")
			{
				app.ScrollRight();
				app.ScrollRight();
				app.ScrollRight();
				app.Back();
				app = null;
			}
		}



		static object[] UiInitialElementList = {
			new []{"android.widget.TextView", "Circles"} ,
			new []{"android.widget.TextView", "Tabs"} ,
			new []{"android.widget.TextView", "Titles"} ,
		};

		static object[] CirclesUIElement = {
			new []{"android.widget.TextView", "Default"} ,
			new []{"android.widget.TextView", "Initial Page"} ,
			new []{"android.widget.TextView", "Snap"} ,
			new []{"android.widget.TextView", "Styled (via layout)"} ,
			new []{"android.widget.TextView", "Styled (via methods)"} ,
			new []{"android.widget.TextView", "Styled (via theme)"} ,
			new []{"android.widget.TextView", "With Listener"} ,

		};


		static object[] TsbsDefaultUIElement = {
			new []{"android.widget.TextView", "RECENT"} ,
			new []{"android.widget.TextView", "ARTISTS"} ,
			new []{"android.widget.TextView", "ALBUMS"} ,
			new []{"android.widget.TextView", "SONGS"} ,
			new []{"android.widget.TextView", "PLAYLISTS"} ,
			new []{"android.widget.TextView", "GENRES"} ,

		};

		static object[] UiTabsElementList = {
			new []{"android.widget.TextView", "Default"} ,
			new []{"android.widget.TextView", "Styled"} ,
		};


		static object[] UiTitlesElementList = {
			new []{"android.widget.TextView", "Center Click Listener"} ,
			new []{"android.widget.TextView", "Default"} ,
			new []{"android.widget.TextView", "Initial Page"} ,
			new []{"android.widget.TextView", "Styled (via Layout)"} ,
			new []{"android.widget.TextView", "Styled (via methods)"} ,
			new []{"android.widget.TextView", "Styled (via theme)"} ,
			new []{"android.widget.TextView", "Triangle Style"} ,
			new []{"android.widget.TextView", "With Listener"} ,

		};
	}
}

