/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;

namespace Mono.Samples.Snake
{
	/**
	 * TileView: a View-variant designed for handling arrays of "icons" or other
	 * drawables.
	 * 
	 */

	public class TileView : View
	{

		/**
		 * Parameters controlling the size of the tiles and their range within view.
		 * Width/Height are in pixels, and Drawables will be scaled to fit to these
		 * dimensions. X/Y Tile Counts are the number of tiles that will be drawn.
		 */

		protected static int mTileSize;

		protected static int mXTileCount;
		protected static int mYTileCount;

		private static int mXOffset;
		private static int mYOffset;

		/**
		 * A hash that maps integer handles specified by the subclasser to the
		 * drawable that will be used for that reference
		 */
		private Bitmap[] mTileArray;

		/**
		 * A two-dimensional array of integers in which the number represents the
		 * index of the tile that should be drawn at that locations
		 */
		private int[,] mTileGrid;

		private Paint mPaint = new Paint ();

		public TileView (Context context, AttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			//TypedArray a = context.ObtainStyledAttributes (attrs, R.styleable.TileView);

			mTileSize = 12; //a.GetInt (R.styleable.TileView_tileSize, 12);

			//a.Recycle ();
		}

		public TileView (Context context, AttributeSet attrs) :
			base (context, attrs)
		{
			//TypedArray a = context.ObtainStyledAttributes (attrs, R.styleable.TileView);

			mTileSize = 12;//a.GetInt (R.styleable.TileView_tileSize, 12);

			//a.Recycle ();
		}

		/**
		 * Rests the internal array of Bitmaps used for drawing tiles, and
		 * sets the maximum index of tiles to be inserted
		 * 
		 * @param tilecount
		 */

		public void resetTiles (int tilecount)
		{
			mTileArray = new Bitmap[tilecount];
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			mXTileCount = (int)System.Math.Floor ((double)w / mTileSize);
			mYTileCount = (int)System.Math.Floor ((double)h / mTileSize);

			mXOffset = ((w - (mTileSize * mXTileCount)) / 2);
			mYOffset = ((h - (mTileSize * mYTileCount)) / 2);

			mTileGrid = new int[mXTileCount, mYTileCount];
			clearTiles ();
		}

		/**
		 * Function to set the specified Drawable as the tile for a particular
		 * integer key.
		 * 
		 * @param key
		 * @param tile
		 */
		public void loadTile (int key, Drawable tile)
		{
			Bitmap bitmap = Bitmap.CreateBitmap (mTileSize, mTileSize, Bitmap.Config.ARGB_8888);
			Canvas canvas = new Canvas (bitmap);
			tile.SetBounds (0, 0, mTileSize, mTileSize);
			tile.Draw (canvas);

			mTileArray[key] = bitmap;
		}

		/**
		 * Resets all tiles to 0 (empty)
		 * 
		 */
		public void clearTiles ()
		{
			for (int x = 0; x < mXTileCount; x++) {
				for (int y = 0; y < mYTileCount; y++) {
					setTile (0, x, y);
				}
			}
		}

		/**
		 * Used to indicate that a particular tile (set with loadTile and referenced
		 * by an integer) should be drawn at the given x/y coordinates during the
		 * next invalidate/draw cycle.
		 * 
		 * @param tileindex
		 * @param x
		 * @param y
		 */
		public void setTile (int tileindex, int x, int y)
		{
			mTileGrid[x, y] = tileindex;
		}


		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
			for (int x = 0; x < mXTileCount; x += 1) {
				for (int y = 0; y < mYTileCount; y += 1) {
					if (mTileGrid[x, y] > 0) {
						canvas.DrawBitmap (mTileArray[mTileGrid[x, y]],
							    mXOffset + x * mTileSize,
							    mYOffset + y * mTileSize,
							    mPaint);
					}
				}
			}
		}
	}
}

