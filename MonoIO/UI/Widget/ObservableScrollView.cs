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
using Android.Util;

namespace MonoIO
{
	public class ObservableScrollView : ScrollView
	{
		private OnScrollListener mScrollListener;

		public ObservableScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
		{	
		}
		
		protected override void OnScrollChanged (int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged (l, t, oldl, oldt);
			
			if (mScrollListener != null) {
	            mScrollListener.OnScrollChanged(this);
	        }
		}
		
		public bool IsScrollPossible() {
	        return ComputeVerticalScrollRange() > Height;
	    }
	
	    public void SetOnScrollListener(OnScrollListener listener) {
	        mScrollListener = listener;
	    }
	
	    public interface OnScrollListener 
	    {
	        void OnScrollChanged(ObservableScrollView view);
	    }
	}
}

