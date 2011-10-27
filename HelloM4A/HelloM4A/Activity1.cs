using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloM4A
{
    [Activity (Label = "Hello M4A", MainLauncher = true)]
    public class Activity1 : Activity
    {

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            
//            //Create UI in code
//            var layout = new LinearLayout (this);
//            layout.Orientation = Orientation.Vertical;
//            
//            var aLabel = new TextView (this);
//            //aLabel.Text = "Hello, Mono for Android";
//            aLabel.SetText(Resource.String.helloLabelText);
//            
//            var aButton = new Button (this);       
//            //aButton.Text = "Say Hello";
//            aButton.SetText(Resource.String.helloButtonText);
//            
//            aButton.Click += (sender, e) => {
//                aLabel.Text = "Hello from the button";
//            };
//            
//            layout.AddView (aLabel);
//            layout.AddView (aButton);
//            
//            SetContentView (layout);
            
            
            //Use UI created in Main.axml
            SetContentView(Resource.Layout.Main);
            
            var aButton = FindViewById<Button> (Resource.Id.aButton);
            var aLabel = FindViewById<TextView> (Resource.Id.helloLabel);
            
            aButton.Click += (sender, e) => {aLabel.Text = "Hello from the button";};          
            
            //can also use anonymous method
//            aButton.Click += delegate(object sender, EventArgs e) {
//                aLabel.Text = "Hello from the button";
//            };
            
        }
    }
}


