using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace Cheesesquare.UITests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest ()
        {
            app = ConfigureApp.Android.StartApp ();
        }

        [Test]
        public void ClickingHamburgerShouldRevealDrawer ()
        {            
            app.Screenshot ("Launch");
            app.Tap(t => t.Class("ImageButton"));
            app.WaitForElement(t => t.Text("Discussion"));
            app.Screenshot ("Tap Hamburger");
        }

        [Test]
        public void ClickingFloatingActionButtonShouldDisplaySnackbar ()
        {
            app.Screenshot ("Launch");
            app.Tap (q => q.Id ("fab"));
            app.Screenshot ("Tap Button");
            app.WaitForElement (q => q.Id ("snackbar_text"));
            app.Screenshot ("Snackbar");
        }

        [Test]
        public void ClickingItemShouldRevealDetails ()
        {
            app.Screenshot ("Launch");
            app.Tap (q => q.Id ("text1"));
            app.Screenshot ("Tap Item");
            app.WaitForElement (q => q.Text ("Info"));
            app.Screenshot ("Details Page");
        }
    }
}

