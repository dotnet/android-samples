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
				.InstalledApp("AppShortcuts.AppShortcuts")
				.StartApp();
		}

		[Test]
		public void TestOpenAddNewWebsiteDialog()
		{
			// tap on "ADD NEW WEBSITE" button
			app.Tap(c => c.Marked("add"));

			// wait until AlertDialog is shown with the title "Add new website"
			app.WaitForElement("Add new website");
		}

		[Test]
		public void TestAddShortcutSuccess()
		{
			var website = "http://www.xamarin.com";
			AddShortcut(website);

			// wait for the list item
			app.WaitForElement(c => c.Id("line1"));
			var itemText = app.Query(c => c.Id("line1").Invoke("getText").Value<string>());

			Assert.NotNull(itemText);
			Assert.AreEqual(itemText[0], website);
		}

		[Test]
		public void TestRemoveShortcut()
		{
			var childCount = app.Query(q => q.Class("ListView").Child()).Length;
			Assert.NotNull(childCount);

			// if there is no added shortcut, add one
			if (childCount == 0)
			{
				AddShortcut("http://www.xamarin.com");
			}

			// tap on "REMOVE" button
			app.Tap(c => c.Marked("remove"));

			// get the list size
			childCount = app.Query(q => q.Class("ListView").Child()).Length;

			Assert.NotNull(childCount);
			Assert.AreEqual(childCount, 0);
		}

		void AddShortcut(string website)
		{
			// tap on "ADD NEW WEBSITE" button
			app.Tap(c => c.Marked("add"));

			// wait until AlertDialog is shown with the title "Add new website"
			app.WaitForElement("Add new website");

			// enter text on edit text
			app.EnterText(c => c.Class("EditText"), website);

			// tap on "ADD" button
			app.Tap(c => c.Class("Button"));
		}

	}
}

