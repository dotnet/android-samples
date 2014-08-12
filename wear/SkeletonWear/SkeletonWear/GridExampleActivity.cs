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
using Android.Graphics;
using Android.Support.Wearable.Views;

namespace SkeletonWear
{
	[Activity (Label = "GridExampleActivity", MainLauncher = false, Exported = true)]			
	public class GridExampleActivity : Activity
	{
		internal int NUM_ROWS = 10;
		internal int NUM_COLS = 3;
		MainAdapter mAdapter;
		GridViewPager mPager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.grid_activity);
			mPager = FindViewById<GridViewPager> (Resource.Id.fragment_container);
			mAdapter = new MainAdapter (FragmentManager, this);
			mPager.Adapter = mAdapter;
		}

		internal class MainAdapter : FragmentGridPagerAdapter
		{
			/*Java generics do not exist in bytecode, so the Java project's Hashmap<Point, ImageReference> 
			 * would be Hashmap<Java.Lang.Object, Java.Lang.Object>. Using C#'s Dictionary provides the same functionality with trivial changes to use.
			*/
			Dictionary<Point, ImageReference> mBackgrounds = new Dictionary<Point, ImageReference> ();
			GridExampleActivity owner;

			public MainAdapter (FragmentManager fm, GridExampleActivity owner) : base (fm)
			{
				this.owner = owner;
			}

			public override int RowCount {
				get {
					return owner.NUM_ROWS;
				}
			}

			public override int GetColumnCount (int p0)
			{
				return owner.NUM_COLS;
			}

			public override Fragment GetFragment (int row, int col)
			{
				return MainFragment.NewInstance (row, col);
			}

			public override ImageReference GetBackground (int row, int col)
			{
				var pt = new Point (row, col);
				ImageReference imgRef = null;
				if(mBackgrounds.ContainsKey(pt))
					imgRef = mBackgrounds [pt];
				if (imgRef == null) {
					var bm = Bitmap.CreateBitmap (200, 200, Bitmap.Config.Argb8888);
					var c = new Canvas (bm);
					var p = new Paint ();
					c.DrawRect (0, 0, 200, 200, p);
					p.AntiAlias = true;
					p.SetTypeface (Typeface.Default);
					p.TextSize = 64;
					p.Color = Color.LightGray;
					c.DrawText (col + "-" + row, 20, 100, p);
					imgRef = ImageReference.ForBitmap (bm);
					mBackgrounds.Add (pt, imgRef);
				}
				return imgRef;
			}
		}

		internal class MainFragment : CardFragment
		{
			public static MainFragment NewInstance (int rowNum, int colNum)
			{
				var args = new Bundle ();
				args.PutString (CardFragment.KeyTitle, "Row :" + rowNum);
				args.PutString (CardFragment.KeyText, "Col :" + colNum);
				var f = new MainFragment ();
				f.Arguments = args;
				return f;
			}

		}
	}

}

