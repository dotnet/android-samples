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
using Android.Text;

namespace Support4
{
	[Activity (Label = "@string/fragment_receive_result_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]		
	public class FragmentReceiveResultSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			var lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,ViewGroup.LayoutParams.MatchParent);
			
	        var frame = new FrameLayout(this);
	        frame.Id = Resource.Id.simple_fragment;
	        SetContentView(frame, lp);
	
	        if (savedInstanceState == null) {
	            // Do first time initialization -- add fragment.
	            var newFragment = new ReceiveResultFragment();
	            var ft = SupportFragmentManager.BeginTransaction();
	            ft.Add(Resource.Id.simple_fragment, newFragment).Commit();
	        }
				
			// Create your application here
		}
		
		public class ReceiveResultFragment : Fragment
		{
			private const int GET_CODE = 0;
			private TextView results;
			
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
			}
			
			public override void OnSaveInstanceState (Bundle p0)
			{
				base.OnSaveInstanceState (p0);
			}
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle p2)
			{
				var v = inflater.Inflate(Resource.Layout.receive_result, container, false);

	            // Retrieve the TextView widget that will display results.
	            results = v.FindViewById<TextView>(Resource.Id.results);
	
	            // This allows us to later extend the text buffer.
	            results.SetText(results.Text, TextView.BufferType.Editable);
	
	            // Watch for button clicks.
	            var getButton = v.FindViewById<Button>(Resource.Id.get);
	            getButton.Click += (sender, e) => {
					// Start the activity whose result we want to retrieve.  The
	                // result will come back with request code GET_CODE.
	                var intent = new Intent(Activity, typeof(SendResult));
	                StartActivityForResult(intent, GET_CODE);
				};
	
	            return v;
			}
			
			public override void OnActivityResult (int requestCode, int resultCode, Intent data)
			{
				// You can use the requestCode to select between multiple child
	            // activities you may have started.  Here there is only one thing
	            // we launch.
				if (requestCode == GET_CODE) {

	                // We will be adding to our text.
	                var text = results.EditableText;
	
	                // This is a standard resultCode that is sent back if the
	                // activity doesn't supply an explicit result.  It will also
	                // be returned if the activity failed to launch.
	                if (resultCode == (int) Result.Canceled) {
	                    text.Append("(cancelled)");
	
	                // Our protocol with the sending activity is that it will send
	                // text in 'data' as its result.
	                } else {
	                    text.Append("(okay ");
	                    text.Append(resultCode.ToString());
	                    text.Append(") ");
	                    if (data != null) {
	                        text.Append(data.Action);
	                    }
	                }
	
	                text.Append("\n");
	            }
			}
			
		}
	}
}

