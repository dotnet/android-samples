using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SwitchDemo
{
    [Activity (Label = "SwitchDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Main);

            Switch s = FindViewById<Switch> (Resource.Id.monitored_switch);
            
            s.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
                
               
                var toast = Toast.MakeText (this, "Your answer is " + (e.IsChecked ? "correct" : "incorrect"), 
                                            ToastLength.Short);
                toast.Show ();
            };
       
        }

    }
}