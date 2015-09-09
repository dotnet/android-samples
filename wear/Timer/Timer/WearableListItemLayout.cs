
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;

using Android.Support.Wearable.Views;

namespace Timer
{
	public class WearableListItemLayout : LinearLayout, WearableListView.IOnCenterProximityListener
	{
	 	float mFadedTextAlpha;
	 	int mFadedCircleColor;
		int mChosenCircleColor;
		ImageView mCircle;
		float mScale;
		TextView mName;

		public WearableListItemLayout (Context context) :
			base (context)
		{
			Initialize ();
		}

		public WearableListItemLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public WearableListItemLayout (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			mFadedTextAlpha =  this.Resources.GetInteger (Resource.Integer.action_text_faded_alpha) / 100f;
			mFadedCircleColor =  this.Resources.GetColor (Resource.Color.wl_gray);
			mChosenCircleColor =  this.Resources.GetColor (Resource.Color.wl_blue);
		}

		protected override void OnFinishInflate ()
		{
			base.OnFinishInflate ();
			mCircle = FindViewById<ImageView> (Resource.Id.circle);
			mName = FindViewById<TextView> (Resource.Id.time_text);
		}

		public void OnCenterPosition (bool animate)
		{
			mName.Alpha = 1f;
			((GradientDrawable) mCircle.Drawable).SetColor (mChosenCircleColor);
		}

		public void OnNonCenterPosition (bool animate)
		{
			((GradientDrawable) mCircle.Drawable).SetColor (mFadedCircleColor);
			mName.Alpha = mFadedTextAlpha;
		}

	}
}

