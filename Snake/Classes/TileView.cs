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
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;

namespace Mono.Samples.Snake
{
	// TileView: a View-variant designed for handling arrays of "icons" or other
	// drawables.
	public class TileView : View
	{
		protected static int tile_size = 12;

		protected static int x_tile_count;
		protected static int y_tile_count;

		private static int x_offset; 
		private static int y_offset;

		private Bitmap[] tile_bitmaps;
		private TileType[,] tiles;

		private Paint paint = new Paint ();

		public TileView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			//TypedArray a = context.ObtainStyledAttributes (attrs, R.styleable.TileView);
			//a.GetInt (R.styleable.TileView_tileSize, 12);
			//a.Recycle ();
		}

		public TileView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			//TypedArray a = context.ObtainStyledAttributes (attrs, R.styleable.TileView);
			//a.GetInt (R.styleable.TileView_tileSize, 12);
			//a.Recycle ();
		}

		#region Public Methods
		// Resets the internal array of Bitmaps used for drawing tiles, and
		// sets the maximum index of tiles to be inserted
		public void ResetTiles (int tileCount)
		{
			tile_bitmaps = new Bitmap[tileCount];
		}

		// Function to set the specified Drawable as the tile for a particular
		// integer key.
		public void LoadTile (TileType type, Drawable tile)
		{
			Bitmap bitmap = Bitmap.CreateBitmap (tile_size, tile_size, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas (bitmap);

			tile.SetBounds (0, 0, tile_size, tile_size);
			tile.Draw (canvas);

			tile_bitmaps[(int)type] = bitmap;
		}

		// Resets all tiles to 0 (empty)
		public void ClearTiles ()
		{
			for (int x = 0; x < x_tile_count; x++)
				for (int y = 0; y < y_tile_count; y++)
					SetTile (0, x, y);
		}

		// Used to indicate that a particular tile (set with loadTile and referenced
		// by an integer) should be drawn at the given x/y coordinates during the
		// next invalidate/draw cycle.
		public void SetTile (TileType tile, int x, int y)
		{
			tiles[x, y] = tile;
		}
		#endregion

		#region Protected Methods
		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			x_tile_count = (int)System.Math.Floor ((double)w / tile_size);
			y_tile_count = (int)System.Math.Floor ((double)h / tile_size);

			x_offset = ((w - (tile_size * x_tile_count)) / 2);
			y_offset = ((h - (tile_size * y_tile_count)) / 2);

			tiles = new TileType[x_tile_count, y_tile_count];

			ClearTiles ();
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			for (int x = 0; x < x_tile_count; x += 1)
				for (int y = 0; y < y_tile_count; y += 1)
					if (tiles[x, y] > 0)
						canvas.DrawBitmap (tile_bitmaps[(int)tiles[x, y]],
							    x_offset + x * tile_size,
							    y_offset + y * tile_size,
							    paint);
		}
		#endregion
	}
}