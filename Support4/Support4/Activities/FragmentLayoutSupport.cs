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
using Android.Support.V4.App;
using Android.Util;
using Android.Locations;
using Android.Preferences;

namespace Support4
{
	[Activity (Label = "@string/fragment_layout_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]
	public class FragmentLayoutSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.fragment_layout_support);		
		}
		
		
	    /**
	     * This is a secondary activity, to show what the user has selected
	     * when the screen is not large enough to show it all in one activity.
	     */
		[Activity]
	    public class DetailsActivity : FragmentActivity 
		{
			
			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				
				if(Resources.Configuration.Orientation == Android.Content.Res.Orientation.Landscape)
				{
					Finish();
					return;
				}
				
				if(savedInstanceState == null)
				{
					var details = new DetailsFragment();
					details.Arguments = Intent.Extras;
					SupportFragmentManager.BeginTransaction().Add(Android.Resource.Id.Content, details).Commit();
				}
			}
	    }
		
		public class TitlesFragment : ListFragment
		{
			bool dualPane;
        	int curCheckPosition = 0;
			
			public override void OnActivityCreated (Bundle savedInstanceState)
			{
				base.OnActivityCreated (savedInstanceState);
				
				ListAdapter = new ArrayAdapter<string>(this.Activity, Resource.Layout.simple_list_item_checkable_1, Android.Resource.Id.Text1, Shakespeare.TITLES); 
				
				View detailsFrame = this.Activity.FindViewById(Resource.Id.details);
				dualPane = detailsFrame != null && detailsFrame.Visibility == ViewStates.Visible;
				
				if(savedInstanceState != null)
				{
					curCheckPosition = savedInstanceState.GetInt("curChoice", 0);	
				}
				
				if(dualPane)
				{
					// In dual-pane mode, the list view highlights the selected item.
					ListView.ChoiceMode = ChoiceMode.Single;
                
	                // Make sure our UI is in the correct state.
	                ShowDetails(curCheckPosition);
				}
			}
		
			public override void OnSaveInstanceState (Bundle outState)
			{
				base.OnSaveInstanceState (outState);
				
				outState.PutInt("curChoice", curCheckPosition);
			}
			
			
			public override void OnListItemClick (ListView l, View v, int position, long id)
			{
				Console.WriteLine ("Show position: " + position);
				ShowDetails(position);
			}
			
			/**
	         * Helper function to show the details of a selected item, either by
	         * displaying a fragment in-place in the current UI, or starting a
	         * whole new activity in which it is displayed.
	         */
	        void ShowDetails(int index) 
			{
	            curCheckPosition = index;
	
				if(dualPane)
				{
					ListView.SetItemChecked(index, true);
					
					var details = (DetailsFragment) FragmentManager.FindFragmentById(Resource.Id.details);
					if(details == null || details.GetShownIndex() != index)
					{
						Console.WriteLine ("Index = " + index);
						details = new DetailsFragment(index);
						
						var ft = FragmentManager.BeginTransaction();
						ft.Replace(Resource.Id.details, details);
						ft.SetTransition(FragmentTransaction.TransitFragmentFade);
						ft.Commit();
					}
				}
				else
				{
					var intent = new Intent();
					intent.SetClass(Activity, typeof(DetailsActivity));
					intent.PutExtra("index", index);
					StartActivity(intent);
				}
			}
		}
		
		public class DetailsFragment : Fragment
		{
			public DetailsFragment()
			{
				
			}
			
			public DetailsFragment(int index)
			{
				var args = new Bundle();
				args.PutInt("index", index);
				Arguments = args;
			}
		
			public int GetShownIndex()
			{
				return Arguments.GetInt("index", 0);
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				if(container == null) 
				{
					// We have different layouts, and in one of them this
					// fragment's containing frame doesn't exist.  The fragment
					// may still be created from its saved state, but there is
					// no reason to try to create its view hierarchy because it
					// won't be displayed.  Note this is not needed -- we could
					// just run the code below, where we would create and return
					// the view hierarchy; it would just never be used.
					return null;	
				}
				
				var scroller = new ScrollView(this.Activity);
				var text = new TextView(this.Activity);
				var padding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, this.Activity.Resources.DisplayMetrics);
				text.SetPadding(padding, padding, padding, padding);
				scroller.AddView(text);
				text.Text = Shakespeare.DIALOGUE[GetShownIndex()];
				return scroller;
				
			}
		}
	}
}

