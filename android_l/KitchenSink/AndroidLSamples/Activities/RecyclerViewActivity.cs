
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace AndroidLSamples
{

	public class RecyclerItem : Java.Lang.Object
	{
		static int[] Ids = new int[]{Resource.Drawable.caterpiller, Resource.Drawable.evolve, Resource.Drawable.flying_in_the_light_large, Resource.Drawable.jelly_fish_2, Resource.Drawable.lone_pine_sunset, Resource.Drawable.look_me_in_the_eye, Resource.Drawable.over_there, Resource.Drawable.rainbow, Resource.Drawable.rainbow, Resource.Drawable.sample1, Resource.Drawable.sample2};
		static int nextId;
		public RecyclerItem()
		{
			Image = Ids[nextId];
			nextId++;
			if (nextId >= Ids.Length)
				nextId = 0;
		}
		public string Title {get;set;}
		public int Image {get;set;}
	}

	[Activity (Label = "RecyclerView Example", ParentActivity=typeof(HomeActivity))]			
	public class RecyclerViewActivity : Activity
	{
		RecyclerView recyclerView;
		RecyclerView.Adapter adapter;
		RecyclerView.LayoutManager layoutManager;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_recycler_view);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetDisplayShowHomeEnabled (true);
			ActionBar.SetIcon (Android.Resource.Color.Transparent);

			recyclerView = FindViewById<RecyclerView> (Resource.Id.recycler_view);
			recyclerView.HasFixedSize = true;


			layoutManager = new LinearLayoutManager (this);
			recyclerView.SetLayoutManager (layoutManager);

			var items = new List<RecyclerItem> (100);
			for(int i = 0; i < 100; i++)
				items.Add(new RecyclerItem{Title = "Item: " + i});

			adapter = new RecyclerAdapter (items);
			recyclerView.SetAdapter (adapter);
		}
			
	}

	public class ViewHolder : RecyclerView.ViewHolder
	{
		public TextView Text {get;set;}
		public ImageView Image {get;set;}

		public ViewHolder(View view) :
		base(view)
		{
			Text = view.FindViewById<TextView> (Resource.Id.textView);
			Image = view.FindViewById<ImageView> (Resource.Id.imageView);
		}
	}

	public class RecyclerAdapter : RecyclerView.Adapter, View.IOnClickListener
	{

		public List<RecyclerItem> Items { get; set; }
		/// <summary>
		/// Generice constructor but you would want ot base this on your data.
		/// </summary>
		/// <param name="items">Items.</param>
		public RecyclerAdapter(List<RecyclerItem> items)
		{
			this.Items = items;
		}

		/// <summary>
		/// Create a new view which is invoked by the layout manager
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="viewType">View type.</param>
		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			var view = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.recycleritem_card, null);
			view.SetOnClickListener (this);
			return new ViewHolder (view);
		}

		/// <summary>
		/// Replaces the content of the view. Invoked by the layout manager
		/// </summary>
		/// <param name="holder">Holder.</param>
		/// <param name="position">Position.</param>
		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			var holder2 = holder as ViewHolder;
			holder2.Text.Text = Items [position].Title;
			holder2.Image.SetImageResource (Items [position].Image);
			holder2.ItemView.Tag = Items [position];
		}

		public override int ItemCount {
			get {
				return Items.Count;
			}
		}

		public void Add(RecyclerItem item) {
			var position = Items.Count;
			Items.Insert(position, item);
			NotifyItemInserted(position);
		}

		public void Remove(int position) {
			if (position < 0)
				return;

			Items.RemoveAt(position);
			NotifyItemRemoved(position);
		}

		public Action<View, RecyclerItem> OnItemClick { get; set; }
		public void OnClick (View v)
		{
			if (OnItemClick == null)
				return;

			OnItemClick (v, (RecyclerItem)v.Tag);
		}
	
	}


}

