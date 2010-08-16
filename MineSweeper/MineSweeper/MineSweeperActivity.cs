using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.OS;

namespace Novell.DroidSamples.MineSweeper
{
	public class MineSweeperActivity : Activity
	{
		public MineSweeperActivity (IntPtr handle)
			: base (handle)
		{ }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (R.layout.mine_sweeper);
		}
	}
}

