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
using Android.Graphics.Drawables;
using Android.Util;

namespace Support4
{
	public class CheckableFrameLayout : FrameLayout, ICheckable
	{
	    private bool _checked;
	
	    public CheckableFrameLayout(Context context) : base(context)
		{
	    }
		
		public CheckableFrameLayout(Context context, IAttributeSet attrs) : base (context, attrs)
		{	
		}
		
	
	    public void Toggle() 
		{
	        Checked = !_checked;
	    }
	
		#region ICheckable implementation
		public bool Checked {
			get {
				return _checked;
			}
			set {
				_checked = value;
				SetBackgroundDrawable(value
						? new ColorDrawable(Context.Resources.GetColor (Resource.Drawable.background))
						: null);
			}
		}
		#endregion


	}
}

