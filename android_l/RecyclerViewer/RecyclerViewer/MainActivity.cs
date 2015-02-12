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
               Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
	public class MainActivity : Activity
	{
        // Recyclerview instance that displays the photo album:
		RecyclerView m_recyclerView;

        // Layout manager lays out each card in the RecyclerView:
		RecyclerView.LayoutManager m_layoutManager;

        // Adapter that accesses the data set (a photo album):
		MyAdapter m_adapter;

        // Photo album that is managed by the adapter:
        PhotoAlbum m_album;

        // Get the reference to the recycler widget and configure it.
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            // Create the photo album:
            m_album = new PhotoAlbum();

			// Set our view from the "main" layout resource:
			SetContentView (Resource.Layout.Main);

            // Get our RecyclerView layout:
			m_recyclerView = FindViewById<RecyclerView> (Resource.Id.recyclerView);

            // Changes in adapter content cannot change the size of the RecyclerView:
			m_recyclerView.HasFixedSize = true;

            // Get the button that shuffles the photo album:
            Button shuffleBtn = FindViewById<Button>(Resource.Id.shuffleButton);

            //............................................................
            // Layout Manager Setup:

            // Use the built-in linear layout manager:
			m_layoutManager = new LinearLayoutManager (this);

            // Or use the built-in grid layout manager:
            // m_layoutManager = new GridLayoutManager(this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            m_recyclerView.SetLayoutManager (m_layoutManager);

            //............................................................
            // Adapter Setup:

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the photo album object) to manage:
			m_adapter = new MyAdapter (m_album);

            // Register the item click handler (below) with the adapter:
            m_adapter.ItemClick += OnItemClick;

            // Plug the adapter into the RecyclerView:
			m_recyclerView.SetAdapter (m_adapter);

            //............................................................
            // Handler for the Shuffle Button:

            shuffleBtn.Click += delegate
            {
                if (m_album != null)
                {
                    // Shuffle the order of the photos:
                    m_album.Shuffle();

                    // Update the RecyclerView by notifying the adapter:
                    m_adapter.NotifyDataSetChanged();
                }
            };
		}

        // Handler for the item click event:
        void OnItemClick (object sender, int position)
        {
            // Display a toast that briefly shows the number of the selected photo:
            int photoNum = position + 1;
            Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
        }
	}

    //----------------------------------------------------------------------
    // VIEW HOLDER

    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) of a CardView in 
    // the RecyclerView:
    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; set; }
        public TextView Caption { get; set; }

        // Get the references to the views defined in the CardView layout:
        public MyViewHolder (View itemView, ImageView imageView, TextView textView, Action<int> listener) 
            : base (itemView)
        {
            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the adapter:
            itemView.Click += (s, e) => listener (base.Position);

            // Cache view references:
            Image = imageView;
            Caption = textView;
        }
    }

    //----------------------------------------------------------------------
    // ADAPTER

    // Adapter to connect the data set (photo album) to each CardView 
    // that renders a photo in the album.
    public class MyAdapter : RecyclerView.Adapter
    {
        // Underlying data set (a photo album):
        public PhotoAlbum m_photoAlbum;

        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Load the adapter with the data set at construction time:
        public MyAdapter(PhotoAlbum photoAlbum)
        {
            m_photoAlbum = photoAlbum;
        }

        // Create a new photo card (this method is invoked by the 
        // layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder (
            ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo card:
            View itemView = LayoutInflater.From (parent.Context).
                        Inflate (Resource.Layout.PhotoCardView, parent, false);

            // Locate views inside the inflated layout file:
            ImageView imageView = itemView.FindViewById<ImageView> (Resource.Id.imageView);
            TextView textView = itemView.FindViewById<TextView>(Resource.Id.textView
);

            // Create a ViewHolder to hold these view references, and register a
            // callback (OnClick) with the view holder:
            MyViewHolder vh = new MyViewHolder (itemView, imageView, textView, OnClick); 
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
            vh.Image.SetImageResource (m_photoAlbum.GetImage(position));
            vh.Caption.Text = m_photoAlbum.GetCaption(position);
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return m_photoAlbum.NumPhotos; }
        }

        // Raise the event when the click takes place:
        void OnClick (int position)
        {
            if (ItemClick != null)
                ItemClick (this, position);
        }
    }
}
