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
            app = ConfigureApp.Android.StartApp();
        }

        /**
         * Checks the action bar title
         */
        [Test]
        public void TestCheckActionBarTitle()
        {
            // Get the text of action bar
            var result = app.Query(c => c.Class("TextView").Text("TextSwitcher"));

            // Check for the text is not null
            Assert.NotNull(result);
        }

        /**
         * Checks for intro text
         */
        [Test]
        public void TestCheckIntroText()
        {
            // Get the intro text
            var currentText = app.Query(x => x.Id("Intro").Invoke("getText"));

            // Check for the text
            Assert.NotNull(currentText);
            Assert.AreEqual(currentText[0], "This sample illustrates the use of a TextSwitcher to display text. \n\nClick the button below to set new text in the TextSwitcher and observe the in and out fade animations. ");
        }

        /**
         * Checks if the 'Next' buttton is available
         */
        [Test]
        public void TestNextButtonAvailable()
        {
            // Wait for Next button
            app.WaitForElement(c => c.Marked("Button").Text("Next"));

            // Get the text button
            var currentText = app.Query(x => x.Id("Button").Invoke("getText").Value<string>());

            // Checks if the button has 'Next' as text
            Assert.NotNull(currentText);
            Assert.AreEqual(currentText[0], "Next");
        }

        /**
         * Checks if the counter increment after tap on 'Next' button
         */
        [Test]
        public void TestTapOnceOnNextButtonIncrementCounter()
        {
            // Tap on 'Next' button
            app.Tap(c => c.Marked("Button"));

            // Get the text view
            var currentText = app.Query(x => x.Id("Switcher").Child(0).Invoke("getText"));
            
            // Checks if the text is '1'
            Assert.NotNull(currentText);
            Assert.AreEqual(currentText[0], "1");
        }

        [Test]
        public void TestTapTwiceOnNextButtonIncrementCounter()
        {
            // Tap on 'Next' button
            app.Tap(c => c.Marked("Button"));
            app.Tap(c => c.Marked("Button"));

            // Get the text view
            var currentText = app.Query(x => x.Id("Switcher").Child(0).Invoke("getText"));

            // Checks if the text is '2'
            Assert.NotNull(currentText);
            Assert.AreEqual(currentText[0], "2");
        }
    }
}

