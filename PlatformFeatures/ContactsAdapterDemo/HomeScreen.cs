using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ContactsAdapterDemo {
    [Activity (Label = "ContactsProvider", MainLauncher = true)]
    public class HomeScreen : Activity {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Main);
            
            // Create an use a custom adapter (displays the photo if available)
            var contactsAdapter = new ContactsAdapter(this);
            var contactsListView = FindViewById<ListView>(Resource.Id.ContactsListView);
            contactsListView.Adapter = contactsAdapter;
        }
    }
}


