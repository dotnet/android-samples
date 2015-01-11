using System;

using Android.Views;
using Android.Support.V17.Leanback.Widget;
using Android.Support.V17.Leanback.App;
using Android.Util;

using SupportViewholder = Android.Support.V17.Leanback.Widget.AbstractDetailsDescriptionPresenter.ViewHolder;
namespace TvLeanback
{
	public class DetailsDescriptionPresenter : AbstractDetailsDescriptionPresenter
	{
		protected override void OnBindDescription (ViewHolder viewHolder, Java.Lang.Object item)
		{
			var movie = (Movie)item;
			if (movie != null) {
				Log.Debug ("DetailsDescriptionPresenter", movie.ToString ());
				viewHolder.Title.Text = movie.Title;
				viewHolder.Subtitle.Text = movie.Studio;
				viewHolder.Body.Text = movie.Description;
			} else
				Log.Debug ("DetailsDescriptionPresenter", "movie == null");
		}
	}
}

