
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

using Java.Util;
using Java.Lang;

namespace JumpingJack
{
	public class MyRunnable : Java.Lang.Object, IRunnable
	{
		CounterFragment cf;
		public MyRunnable(CounterFragment counterFragment)
		{
			cf = counterFragment;
		}
		public void Run()
		{
			cf.counter_text.SetCompoundDrawablesRelativeWithIntrinsicBounds (
				cf.up ? cf.up_drawable : cf.down_drawable, null, null, null);
			cf.up = !cf.up;
		}
	}

	public class MyTimertask : TimerTask
	{
		CounterFragment cf;
		public MyTimertask(CounterFragment counterFragment)
		{
			cf = counterFragment;
		}
		public override void Run ()
		{
			cf.handler.Post(new MyRunnable(cf));
		}
	}

	public class CounterFragment : Android.Support.V4.App.Fragment
	{

		const int ANIMATION_INTERVAL_MS = 500;
		public TextView counter_text;
		Timer animation_timer;
		public Handler handler;
		MyTimertask animation_task;
		public bool up = false;
		public Drawable down_drawable;
		public Drawable up_drawable;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate (Resource.Layout.counter_layout, container, false);
			down_drawable = Resources.GetDrawable (Resource.Drawable.jump_down_50);
			up_drawable = Resources.GetDrawable (Resource.Drawable.jump_up_50);
			counter_text = (TextView)view.FindViewById (Resource.Id.counter);
			counter_text.SetCompoundDrawablesRelativeWithIntrinsicBounds (up_drawable, null, null, null);
			SetCounter (Utils.GetCounterFromPreference (Activity));
			handler = new Handler ();
			StartAnimation ();
			return view;

		}

		public void StartAnimation()
		{
			animation_task = new MyTimertask (this);
			animation_timer = new Timer ();
			animation_timer.ScheduleAtFixedRate (animation_task, ANIMATION_INTERVAL_MS, ANIMATION_INTERVAL_MS);
		}

		public void SetCounter(string text)
		{
			counter_text.Text = text;
		}

		public void SetCounter(int i)
		{
			SetCounter (i < 0 ? "0" : i.ToString ());
		}

		public override void OnDetach ()
		{
			animation_timer.Cancel ();
			base.OnDetach ();
		}
	}
}

