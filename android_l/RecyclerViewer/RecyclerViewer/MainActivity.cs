using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace RecyclerViewer
{
	[Activity (Label = "RecyclerViewer", MainLauncher = true, 
               Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light")]
	public class MainActivity : Activity
	{
        // Recyclerview instance that will display the photo album:
		RecyclerView recyclerView;

        // Layout manager that will lay out each card in the RecyclerView:
		RecyclerView.LayoutManager layoutManager;

        // Get the reference to the recycler widget and configure it.
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource:
			SetContentView (Resource.Layout.Main);

            // Get our RecyclerView layout:
			recyclerView = FindViewById<RecyclerView> (Resource.Id.recyclerView);
            // Changes in adapter content cannot change the size of the RecyclerView:
			recyclerView.HasFixedSize = true;

            // Create and plug in the built-in (Android-provided) linear layout 
            // manager for the RecyclerView:
			layoutManager = new LinearLayoutManager (this);
            recyclerView.SetLayoutManager (layoutManager);

            // Create and plug in my photo album adapter for the RecyclerView:
			MyAdapter myAdapter = new MyAdapter ();
			recyclerView.SetAdapter (myAdapter);
		}
	}

    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) of a CardView in 
    // the RecyclerView:
    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; set; }
        public TextView Text { get; set; }

        // Get the references to the views defined in the CardView layout:
        public MyViewHolder (View view) : base (view)
        {
            Image = view.FindViewById<ImageView> (Resource.Id.imageView);
            Text = view.FindViewById<TextView> (Resource.Id.textView);
        }
    }

    // Adapter to connect my data model (photo album) to each CardView 
    // that renders a photo in the album.
    public class MyAdapter : RecyclerView.Adapter
    {
        // Create a new photo card (this method is invoked by the 
        // layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder (
            ViewGroup viewGroup, int viewType)
        {
            // Inflate the CardView for the photo card:
            View view = LayoutInflater.From (viewGroup.Context).
                        Inflate (Resource.Layout.PhotoCardView, null);

            // Create a ViewHolder to hold this CardView:
            MyViewHolder vh = new MyViewHolder (view); 
            return vh;
        }

        // Fill in the contents of the photo card (this method is invoked 
        // by the layout manager):
        public override void OnBindViewHolder (
            RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder vh = holder as MyViewHolder;

            // Set the photo image and title of this ViewHolder's CardView 
            // from this position in the photo album:
            vh.Image.SetImageResource (PhotoAlbum.ImageIds[position]);
            vh.Text.Text = PhotoAlbum.Titles[position];
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return PhotoAlbum.NumPhotos; }
        }
    }
}
