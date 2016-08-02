using System;

using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Content;
using Android.Runtime;

using Squareup.Picasso;

using Android.Support.V17.Leanback.App;
using Android.Support.V17.Leanback.Widget;

namespace TvLeanback
{
	public class CardPresenter : Presenter
	{
		private const string TAG = "CardPresenter";

		protected static Context mContext {
			get;
			private set;
		}

		public static readonly int CARD_WIDTH = 200;
		public static readonly int CARD_HEIGHT = 100;


		public override ViewHolder OnCreateViewHolder (ViewGroup parent)
		{
			mContext = parent.Context;

			var cardView = new ImageCardView (mContext);
			cardView.Focusable = true;
			cardView.FocusableInTouchMode = true;
			cardView.SetBackgroundColor (mContext.Resources.GetColor (Resource.Color.fastlane_background));
			return new CustomViewHolder (cardView);
		}

		public override void OnBindViewHolder (Presenter.ViewHolder viewHolder, Java.Lang.Object item)
		{
			Movie movie = (Movie)item;
			((CustomViewHolder)viewHolder).movie = movie;

			if (movie.GetCardImageURI () != null) {
				var holder = (CustomViewHolder)viewHolder;
				var card = holder.cardView;
				card.TitleText = movie.Title;
				card.ContentText = movie.Studio;
				card.SetMainImageDimensions (CARD_WIDTH, CARD_HEIGHT);
				holder.UpdateCardViewImage (movie.GetCardImageURI ());
			}
		}

		public override void OnUnbindViewHolder (Presenter.ViewHolder viewHolder)
		{
		}

		public override void OnViewAttachedToWindow (Presenter.ViewHolder viewHolder)
		{
		}
	}

	public class PicassoImageCardViewTarget : Java.Lang.Object, ITarget
	{
		private ImageCardView mImageCardView;

		public PicassoImageCardViewTarget (ImageCardView imageCardView)
		{
			mImageCardView = imageCardView;
		}

		public void OnBitmapLoaded (Bitmap bitmap, Picasso.LoadedFrom loadedFrom)
		{
			Drawable bitmapDrawable = new BitmapDrawable (mImageCardView.Context.Resources, bitmap); //TODO get proper context
			mImageCardView.MainImage = bitmapDrawable;
		}

		public void OnBitmapFailed (Drawable drawable)
		{
			mImageCardView.MainImage = drawable;
		}

		public void OnPrepareLoad (Drawable drawable)
		{
			// Do nothing, default_background manager has its own transitions
		}

	}

	public class CustomViewHolder : Android.Support.V17.Leanback.Widget.Presenter.ViewHolder
	{
		private readonly Context mContext;

		public Movie movie {
			get;
			set;
		}

		public ImageCardView cardView {
			get;
			private set;
		}

		public string TitleText {
			set {
				cardView.TitleText = value;
			}
			get {
				return cardView.TitleText;
			}
		}

		public string ContentText {
			set {
				cardView.ContentText = value;
			}
			get {
				return cardView.ContentText;
			}
		}

		private Drawable mDefaultCardImage;
		private PicassoImageCardViewTarget mImageCardViewTarget;

		public CustomViewHolder (View view) : base (view)
		{
			cardView = (ImageCardView)view;
			mContext = view.Context;
			mImageCardViewTarget = new PicassoImageCardViewTarget (cardView);
			mDefaultCardImage = mContext.Resources.GetDrawable (Resource.Drawable.movie);
		}

		public void UpdateCardViewImage (Java.Net.URI uri) //FIXME prob this version, maybe don't work with either
		{
			Picasso.With (mContext)
				.Load (uri.ToString ())
				.Resize (Utils.dpToPx (CardPresenter.CARD_WIDTH, mContext), 
				Utils.dpToPx (CardPresenter.CARD_HEIGHT, mContext))
				.Error (mDefaultCardImage)
				.Into (mImageCardViewTarget);
		}
	}
}

