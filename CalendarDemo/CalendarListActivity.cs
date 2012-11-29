using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Java.Util;

namespace CalendarDemo
{
    [Activity (Label = "CalendarDemo", MainLauncher = true)]
    public class CalendarListActivity : ListActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.CalendarList);
           
            // List Calendars
            var calendarsUri = CalendarContract.Calendars.ContentUri;
            
            string[] calendarsProjection = {
               CalendarContract.Calendars.InterfaceConsts.Id,
               CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
               CalendarContract.Calendars.InterfaceConsts.AccountName
            };
            
            var cursor = ManagedQuery (calendarsUri, calendarsProjection, null, null, null);
            
            string[] sourceColumns = {CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, 
                CalendarContract.Calendars.InterfaceConsts.AccountName};
            
            int[] targetResources = {Resource.Id.calDisplayName, Resource.Id.calAccountName};       
            
            SimpleCursorAdapter adapter = new SimpleCursorAdapter (this, Resource.Layout.CalListItem, 
                cursor, sourceColumns, targetResources);
            
            ListAdapter = adapter;
            
            ListView.ItemClick += (sender, e) => { 
                int i = (e as Android.Widget.AdapterView.ItemClickEventArgs).Position;
                
                cursor.MoveToPosition(i);
                int calId = cursor.GetInt (cursor.GetColumnIndex (calendarsProjection [0]));
                
                var showEvents = new Intent(this, typeof(EventListActivity));
                showEvents.PutExtra("calId", calId);
                StartActivity(showEvents);
            };
        }
    }
}