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
using Android.Text.Format;
using Android.Graphics.Drawables;
using Android.Graphics;
using TimeZone = Java.Util.TimeZone;
using MonoIO.Utilities;

namespace MonoIO
{
	public class BlockView : Button
	{
		private static FormatStyleFlags TIME_STRING_FLAGS = FormatStyleFlags.ShowDate | FormatStyleFlags.ShowWeekday 
			| FormatStyleFlags.AbbrevWeekday | FormatStyleFlags.ShowTime | FormatStyleFlags.AbbrevTime;
		private string mBlockId;
		private string mTitle;
		private long mStartTime;
		private long mEndTime;
		private bool mContainsStarred;
		private int mColumn;
		
		public BlockView (Context context, string blockId, string title, long startTime, long endTime, bool containsStarred, int column) : base (context)
		{
			
			mBlockId = blockId;
			mTitle = title;
			mStartTime = startTime;
			mEndTime = endTime;
			mContainsStarred = containsStarred;
			mColumn = column;
	
			Text = mTitle;
	
			// TODO: turn into color state list with layers?
			Color textColor = Android.Graphics.Color.White;
			Color accentColor = new Color (-1);
			switch (mColumn) {
			case 0:
				accentColor = Resources.GetColor (Resource.Color.block_column_1);
				break;
			case 1:
				accentColor = Resources.GetColor (Resource.Color.block_column_2);
				break;
			case 2:
				accentColor = Resources.GetColor (Resource.Color.block_column_3);
				break;
			}
	
			LayerDrawable buttonDrawable = (LayerDrawable)
	                context.Resources.GetDrawable (Resource.Drawable.btn_block);
			buttonDrawable.GetDrawable (0).SetColorFilter (accentColor, PorterDuff.Mode.SrcAtop);
			buttonDrawable.GetDrawable (1).SetAlpha (mContainsStarred ? 255 : 0);
	
			SetTextColor (textColor);
			SetBackgroundDrawable (buttonDrawable);
		}
		
		public string GetBlockId ()
		{
			return mBlockId;
		}
	
		public string GetBlockTimeString ()
		{
			TimeZone.Default = UIUtils.ConferenceTimeZone;
			return DateUtils.FormatDateTime (Context, mStartTime, TIME_STRING_FLAGS);
		}
	
		public long GetStartTime ()
		{
			return mStartTime;
		}
	
		public long GetEndTime ()
		{
			return mEndTime;
		}
	
		public int GetColumn ()
		{
			return mColumn;
		}
	}
}

