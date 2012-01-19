using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;

namespace ShareActionProviderDemo
{
    [Activity (Label = "ShareActionProviderDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            
            CopyToPublic ("monkey.png");

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
        }
        
        public override bool OnCreateOptionsMenu (IMenu menu)
        {
            MenuInflater.Inflate (Resource.Menu.ActionBarMenu, menu);        
            
            var shareMenuItem = menu.FindItem (Resource.Id.shareMenuItem);            
            var shareActionProvider = (ShareActionProvider)shareMenuItem.ActionProvider;
            shareActionProvider.SetShareIntent (CreateIntent ());
            
            var overflow_item = menu.FindItem (Resource.Id.overflowMenuItem);
            var overflow_provider = (ShareActionProvider)overflow_item.ActionProvider;
            overflow_provider.SetShareIntent (CreateIntent ());
            
            return true;
        }
        
        Intent CreateIntent ()
        {   
            var sendPictureIntent = new Intent (Intent.ActionSend);
            sendPictureIntent.SetType ("image/*");            
            var uri = Android.Net.Uri.FromFile (GetFileStreamPath ("monkey.png"));           
            sendPictureIntent.PutExtra (Intent.ExtraStream, uri);

            return sendPictureIntent;
        }
        
        void CopyToPublic (String fileName)
        {     
            using (Stream fromStream = Assets.Open (fileName)) {

                string filePath = Path.Combine (new string[]{"data", "data", PackageName, "files", fileName});
                
                int size = 32 * 1024;

                using (FileStream toStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {

                    int n = -1;
                    byte[] buffer = new byte[size];

                    while ((n = fromStream.Read(buffer, 0, size)) > 0) {
                        toStream.Write (buffer, 0, n);
                    }
                }
            }
        }
        
    }
}