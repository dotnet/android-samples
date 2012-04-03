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
using Android.Support.V4.View;
using Java.Lang;
using Android.Util;

namespace ViewPagerIndicator
{
	class TabPageIndicator : HorizontalScrollView, PageIndicator
	{
		private LinearLayout mTabLayout;
		private ViewPager mViewPager;
		private ViewPager.IOnPageChangeListener mListener;
		private LayoutInflater mInflater;
		int mMaxTabWidth;
		private int mSelectedTabIndex;
		
		public TabPageIndicator (Context context) : base(context, null)
		{
		}
	
		public TabPageIndicator (Context context, IAttributeSet attrs) : base(context, attrs)
		{
			HorizontalScrollBarEnabled = false;
			
			mInflater = LayoutInflater.From (context);
	
			mTabLayout = new LinearLayout (Context);
			AddView (mTabLayout, new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.FillParent));
		}
		
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			var widthMode = MeasureSpec.GetMode (widthMeasureSpec);
			var lockedExpanded = widthMode == MeasureSpecMode.Exactly;
			FillViewport = lockedExpanded;
	
			int childCount = mTabLayout.ChildCount;
			if (childCount > 1 && (widthMode == MeasureSpecMode.Exactly || widthMode == MeasureSpecMode.AtMost)) {
				if (childCount > 2) {
					mMaxTabWidth = (int)(MeasureSpec.GetSize (widthMeasureSpec) * 0.4f);
				} else {
					mMaxTabWidth = MeasureSpec.GetSize (widthMeasureSpec) / 2;
				}
			} else {
				mMaxTabWidth = -1;
			}
	
			int oldWidth = MeasuredWidth;
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			int newWidth = MeasuredWidth;
	
			if (lockedExpanded && oldWidth != newWidth) {
				// Recenter the tab display if we're at a new (scrollable) size.
				SetCurrentItem (mSelectedTabIndex);
			}
		}
		
		private void AnimateToTab (int position)
		{
			var tabView = mTabLayout.GetChildAt (position);
	        
			// Do we not have any call backs because we're handling this with Post?
			/*if (mTabSelector != null) {
	            RemoveCallbacks(mTabSelector);
	        }*/
			
			Post (() => {
				var scrollPos = tabView.Left - (Width - tabView.Width) / 2;
				SmoothScrollTo (scrollPos, 0);
			});
		}
		
		public void SetCurrentItem (int item)
		{
			if (mViewPager == null) {
				throw new IllegalStateException ("ViewPager has not been bound.");
			}
			mSelectedTabIndex = item;
			var tabCount = mTabLayout.ChildCount;
			for (int i = 0; i < tabCount; i++) {
				var child = mTabLayout.GetChildAt (i);
				var isSelected = (i == item);
				child.Selected = isSelected;
				if (isSelected) {
					AnimateToTab (item);
				}
			}
		}
		
		protected override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			
			Console.WriteLine ("OnAttachedToWindow");
			/*
			 * 
			 *  super.onAttachedToWindow();
        if (mTabSelector != null) {
            // Re-post the selector we saved
            post(mTabSelector);
        }
*/
		}
		
		protected override void OnDetachedFromWindow ()
		{
			base.OnDetachedFromWindow ();
			
			Console.WriteLine ("OnDetachedFromWindow...");
//			super.onDetachedFromWindow();
//        if (mTabSelector != null) {
//            removeCallbacks(mTabSelector);
//        }
		}
		
		private void AddTab (string text, int index)
		{
			//Workaround for not being able to pass a defStyle on pre-3.0
			var tabView = (TabView)mInflater.Inflate (Resource.Layout.vpi__tab, null);
			tabView.Init (this, text, index);
			tabView.Focusable = true;
			tabView.Click += delegate(object sender, EventArgs e) {
				var tView = (TabView)sender;
				
				mViewPager.CurrentItem = tView.GetIndex ();
			};
	
			mTabLayout.AddView (tabView, new LinearLayout.LayoutParams (0, LayoutParams.FillParent, 1));
		}
		
		public void OnPageScrollStateChanged (int p0)
		{
			if (mListener != null) {
				mListener.OnPageScrollStateChanged (p0);
			}
		}
		
		public void OnPageScrolled (int p0, float p1, int p2)
		{
			if (mListener != null) {
				mListener.OnPageScrolled (p0, p1, p2);
			}
		}
		
		public void OnPageSelected (int p0)
		{
			SetCurrentItem (p0);
			if (mListener != null) {
				mListener.OnPageSelected (p0);
			}
		}
		
		public void SetViewPager (ViewPager view)
		{
			var adapter = view.Adapter;
			if (adapter == null) {
				throw new IllegalStateException ("ViewPager does not have adapter instance.");
			}
			if (!(adapter is TitleProvider)) {
				throw new IllegalStateException ("ViewPager adapter must implement TitleProvider to be used with TitlePageIndicator.");
			}
			mViewPager = view;
			view.SetOnPageChangeListener (this);
			NotifyDataSetChanged ();
		}
		
		public void NotifyDataSetChanged ()
		{
			mTabLayout.RemoveAllViews ();
			TitleProvider adapter = (TitleProvider)mViewPager.Adapter;
			int count = ((PagerAdapter)adapter).Count;
			for (int i = 0; i < count; i++) {
				AddTab (adapter.GetTitle (i), i);
			}
			if (mSelectedTabIndex > count) {
				mSelectedTabIndex = count - 1;
			}
			SetCurrentItem (mSelectedTabIndex);
			RequestLayout ();
		}
		
		public void SetViewPager (ViewPager view, int initialPosition)
		{
			SetViewPager (view);
			SetCurrentItem (initialPosition);
		}
		
		public void SetOnPageChangeListener (ViewPager.IOnPageChangeListener listener)
		{
			mListener = listener;
		}
		
		public class TabView : LinearLayout
		{
			private TabPageIndicator mParent;
			private int mIndex;
	
			public TabView (Context context, IAttributeSet attrs) : base(context, attrs)
			{
			}
	
			public void Init (TabPageIndicator parent, string text, int index)
			{
				mParent = parent;
				mIndex = index;
	
				TextView textView = FindViewById<TextView> (Android.Resource.Id.Text1);
				textView.Text = text;
			}
			
			protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
			{
				base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
				
				// Re-measure if we went beyond our maximum size.
				if (mParent.mMaxTabWidth > 0 && MeasuredWidth > mParent.mMaxTabWidth) {
					base.OnMeasure (MeasureSpec.MakeMeasureSpec (mParent.mMaxTabWidth, MeasureSpecMode.Exactly), heightMeasureSpec);
				}
				
			}
	
			public int GetIndex ()
			{
				return mIndex;
			}
		}
	}
}

