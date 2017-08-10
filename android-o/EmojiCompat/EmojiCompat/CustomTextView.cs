using System;
using Android.Content;
using Android.Support.V7.Widget;

namespace EmojiCompat
{
	/**
 	 * A sample implementation of custom TextView.
 	 *
 	 * <p>You can use {@link EmojiTextViewHelper} to make your custom TextView compatible with
 	 * EmojiCompat.</p>
 	 */
	public class CustomTextView : AppCompatTextView
	{
		EmojiTextViewHelper mEmojiTextViewHelper;

		public CustomTextView(Context context) : base(context, null)
		{
		}
	}
}
