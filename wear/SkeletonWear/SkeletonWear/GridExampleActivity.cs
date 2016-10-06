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
using Android.Graphics.Drawables;

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
			Dictionary<Point, Drawable> mBackgrounds = new Dictionary<Point, Drawable> ();
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

			public override Android.Graphics.Drawables.Drawable GetBackgroundForPage (int row, int column)
			{
				Point pt = new Point (column, row);
				Drawable drawable;
				if (!mBackgrounds.ContainsKey(pt))
				{
					// the key wasn't found in Dictionary
					var bm = Bitmap.CreateBitmap(200, 200, Bitmap.Config.Argb8888);
					var c = new Canvas(bm);
					var p = new Paint();
					// Clear previous image.
					c.DrawRect(0, 0, 200, 200, p);
					p.AntiAlias = true;
					p.SetTypeface(Typeface.Default);
					p.TextSize = 64;
					p.Color = Color.LightGray;
					p.TextAlign = Paint.Align.Center;
					c.DrawText(column + "-" + row, 100, 100, p);
					drawable = new BitmapDrawable(owner.Resources, bm);
					mBackgrounds.Add(pt, drawable);
				}
				else
				{
					// the key was found
					drawable = mBackgrounds[pt];
				}
				return drawable;
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

