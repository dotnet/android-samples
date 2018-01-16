using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using Android.Arch.Lifecycle;
using Android.Support.V4.App;
using Android.Widget;

namespace ViewModel
{
	[Activity(Label = "ViewModel", MainLauncher = true, Icon = "@mipmap/ic_resource")]
	public class MainActivity : FragmentActivity
	{
		ItemViewModel ViewModel;
		RecyclerView RecyclerView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			// setup RecyclerView
			RecyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);
			RecyclerView.SetLayoutManager(new LinearLayoutManager(this));

			// setup ViewModel
			ViewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(ItemViewModel))) as ItemViewModel;
		}

		protected override void OnStart()
		{
			base.OnStart();

			// Update Items
			RecyclerView.SetAdapter(new ItemAdapter { List = ViewModel.GetItems() });
		}

		class ItemAdapter : RecyclerView.Adapter
		{
			public List<Item> List;

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ViewItem, parent, false);
				return new ItemViewHolder(view);
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				var itemViewHolder = holder as ItemViewHolder;
				itemViewHolder.Name.Text = List[position].Name;
			}

			public override int ItemCount => List.Count;

			class ItemViewHolder : RecyclerView.ViewHolder
			{
				public TextView Name { get; set; }
				public ItemViewHolder(View itemView) : base(itemView)
				{
					Name = itemView.FindViewById<TextView>(Resource.Id.Name);
				}
			}
		}
	}
}

