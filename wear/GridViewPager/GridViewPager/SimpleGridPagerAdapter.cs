using System;
using Android.Content;
using Android.App;
using Android.Support.Wearable.Views;
using Android.Views;

namespace GridViewPagerSample
{
	public class SimpleGridPagerAdapter : FragmentGridPagerAdapter
	{
		private readonly Context context;
		public SimpleGridPagerAdapter (Context ctx, FragmentManager gm)
			:base(gm)
		{
			context = ctx;
		}

		static readonly int[] BgImages = new int[] {
			Resource.Drawable.debug_background_1,
			Resource.Drawable.debug_background_2,
			Resource.Drawable.debug_background_3,
			Resource.Drawable.debug_background_4,
			Resource.Drawable.debug_background_5
		};

		private class Page {
			public int TitleRes;
			public int TextRes;
			public int IconRes;
			public GravityFlags CardGravity = GravityFlags.Bottom;
			public bool ExpansionEnabled = true;
			public float ExpansionFactor = 1;
			public int ExpansionDirection = CardFragment.ExpandDown;


			public Page (int titleRes, int textRes, bool expansion)
			 :this(titleRes, textRes, 0) 
			{
				this.ExpansionEnabled = expansion;
			}

			public Page(int titleRes, int textRes, bool expansion, float expansionFactor) 
				:this(titleRes, textRes, 0)
			{
				this.ExpansionEnabled = expansion;
				this.ExpansionFactor = expansionFactor;
			}

			public Page (int titleRes, int textRes, int iconRes) {
				this.TitleRes = titleRes;
				this.TextRes = textRes;
				this.IconRes = iconRes;
			}

			public Page (int titleRes, int textRes, int iconRes, GravityFlags gravity) 
			{
				this.TitleRes = titleRes;
				this.TextRes = textRes;
				this.IconRes = iconRes;
				this.CardGravity = gravity;
			}
		}

		private readonly Page[][] Pages = new Page[][]
		{
			new Page[]{ new Page(Resource.String.welcome_title, Resource.String.welcome_text, Resource.Drawable.bugdroid, GravityFlags.CenterVertical) },
			new Page[]{ new Page(Resource.String.about_title, Resource.String.about_text, false) },
			new Page[]
			{ 
				new Page(Resource.String.cards_title, Resource.String.cards_text, true, 2),
				new Page(Resource.String.expansion_title, Resource.String.expansion_text, true, 10)
			},
			new Page[]
			{
				new Page(Resource.String.backgrounds_title, Resource.String.backgrounds_text, true, 2),
				new Page(Resource.String.columns_title, Resource.String.columns_text, true, 2)
			},
			new Page[]
			{
				new Page(Resource.String.dismiss_title, Resource.String.dismiss_text, Resource.Drawable.bugdroid, GravityFlags.CenterVertical)
			}
		};

		public override Fragment GetFragment (int row, int col)
		{
			Page page = Pages [row] [col];
			String title = page.TitleRes != 0 ? context.GetString (page.TitleRes) : null;
			String text = page.TextRes != 0 ? context.GetString (page.TextRes) : null;
			CardFragment fragment = CardFragment.Create (title, text, page.IconRes);
			// Advanced settings
			fragment.SetCardGravity ((int)page.CardGravity);
			fragment.SetExpansionEnabled (page.ExpansionEnabled);
			fragment.SetExpansionDirection (page.ExpansionDirection);
			fragment.SetExpansionFactor (page.ExpansionFactor);
			return fragment;
		}

		public override Android.Graphics.Drawables.Drawable GetBackgroundForPage (int row, int col)
		{
			return context.Resources.GetDrawable (BgImages [row % BgImages.Length]);
		}

		public override int RowCount {
			get {
				return Pages.Length;
			}
		}

		public override int GetColumnCount (int rowNum)
		{
			return Pages [rowNum].Length;
		}
	}
}

