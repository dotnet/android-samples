using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Com.SlidingMenu.Lib.App;
using Com.SlidingMenu.Lib;

namespace SlidingMenuExample
{
	[Activity (Label = "SlidingMenuExample", MainLauncher = true)]
	public class Activity1 : SlidingActivity, SlidingMenu.IOnClosedListener, SlidingMenu.IOnCloseListener, SlidingMenu.IOnOpenListener, SlidingMenu.IOnOpenedListener
	{
		public override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			SetBehindContentView(Resource.Layout.menu);

			this.SlidingMenu.SetFadeEnabled (true);
			this.SlidingMenu.SetFadeDegree(0.35f);

			this.SlidingMenu.SetOnCloseListener (this);
			this.SlidingMenu.SetOnOpenedListener (this);
			this.SlidingMenu.SetOnOpenListener (this);
			this.SlidingMenu.SetOnOpenedListener (this);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			Button back = FindViewById<Button> (Resource.Id.Button5);
			CheckBox marginCheck = FindViewById<CheckBox> (Resource.Id.marginCheck);
			RadioGroup slideFromRadios = FindViewById<RadioGroup>(Resource.Id.slideFromOptions);

			slideFromRadios.CheckedChange += (object sender, RadioGroup.CheckedChangeEventArgs e) => {
				if (e.CheckedId == Resource.Id.modeLeftRadio)
					this.SlidingMenu.Mode = 0; //LEFT
				else
					this.SlidingMenu.Mode = 1; //RIGHT
			};

			marginCheck.Click += (object sender, EventArgs e) => {

				this.SlidingMenu.BehindOffset= (int)((marginCheck.Checked) ? Resources.GetDimension(Resource.Dimension.slidingMargin) : 0) ;
			};

			back.Click += MenuToggle;
			button.Click += MenuToggle;



		}

		void MenuToggle(object sender, EventArgs e)
		{
			this.Toggle ();
		}
	
	
		#region Close and Open listener
		public void OnOpen ()
		{
			Console.Write("Open");
		}

		public void OnOpened ()
		{
			Console.Write("Opened");
		}

		public void OnClose ()
		{
			Console.Write("Close");
		}

		public void OnClosed ()
		{
			Console.Write("Closed");
		}
		#endregion
	}
}


