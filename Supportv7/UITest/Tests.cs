using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace UITest
{
	[TestFixture]
	public class Tests
	{
		AndroidApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			app = ConfigureApp
				.Android
				.InstalledApp("com.xamarin.actionbarcompat_listpopupmenu")
				.StartApp();
		}

		[Test]
		public void TapOnPopupMenu()
		{
			//app.Repl();
			var childCount = app.Query(q => q.Class("ListView").Child()).Length;
			if (childCount <= 0) return;
			// Tap on PopupMenu button of first element
			app.Tap(q => q.Class("ListView").Child(0).Child(1).Id("button_popup"));

			// wait for 'Remove' button element
			app.WaitForElement(p => p.Text("Remove"));
		}

		[Test]
		public void RemoveElement()
		{
			//app.Repl();
			var childCount = app.Query(q => q.Class("ListView").Child()).Length;
			if (childCount <= 0) return;

			// keep the first element name
			var firstOldElement = app.Query(q => q.Class("ListView").Invoke("getAdapter").Invoke("getItem", 0).Value<string>());

			// Tap on PopupMenu button of first element
			app.Tap(q => q.Class("ListView").Child(0).Child(1).Id("button_popup"));

			// wait for 'Remove' button element
			app.WaitForElement(p => p.Text("Remove"));

			// tap on 'Remove' button
			app.Tap(q => q.Text("Remove"));

			// get the first new element
			var firstNewElement = app.Query(q => q.Class("ListView").Invoke("getAdapter").Invoke("getItem", 0).Value<string>());

			Assert.AreNotEqual(firstOldElement, firstNewElement);
		}
	}
}

