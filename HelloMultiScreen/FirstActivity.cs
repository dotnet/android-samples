using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloMultiScreen
{
    [Activity (Label = "HelloMultiScreen", MainLauncher = true)]
    public class FirstActivity : Activity
    {

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Load the UI created in Main.axml          
            SetContentView (Resource.Layout.Main);
                
            // Get a reference to the button
            var showSecond = FindViewById<Button> (Resource.Id.showSecond);
 
            // You can use either this short form of StartActivity, which will create 
            // an intent internally, or the long form shown below.           
//            showSecond.Click += (sender, e) => {           
//                StartActivity (typeof(SecondActivity));
//            };
            
            // Long form of StartActivity with the intent created in code so that
            // data can be added to the message payload using the PutExtra call.          
            showSecond.Click += (sender, e) => {           
                var second = new Intent(this, typeof(SecondActivity));
                second.PutExtra("FirstData", "Data from FirstActivity");
                StartActivity (second);
            };
        }
    }
}