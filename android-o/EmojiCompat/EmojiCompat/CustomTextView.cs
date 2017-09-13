using System;
using Android.Content;
using Android.Runtime;
using Android.Support.Text.Emoji.Widget;
using Android.Support.V7.Widget;
using Android.Util;

namespace EmojiCompatSample
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

		public CustomTextView(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) 
		{}

		public CustomTextView(Context context): this(context, null) 
		{}

		public CustomTextView(Context context, IAttributeSet attrs) : this(context, attrs, 0) 
		{}

		public CustomTextView(Context context, IAttributeSet attrs, int defStyleAttr) 
			: base(context, attrs, defStyleAttr) 
		{
			GetEmojiTextViewHelper().UpdateTransformationMethod();
		}

		public override void SetFilters(Android.Text.IInputFilter[] filters)
		{
			base.SetFilters(GetEmojiTextViewHelper().GetFilters(filters));
		}

		public override void SetAllCaps(bool allCaps)
		{
			base.SetAllCaps(allCaps);
			GetEmojiTextViewHelper().SetAllCaps(allCaps);
		}

		/**
		 * Returns the {@link EmojiTextViewHelper} for this TextView.
		 *
		 * <p>This method can be called from super constructors through {@link
		 * #setFilters(InputFilter[])} or {@link #setAllCaps(boolean)}.</p>
		 */
		EmojiTextViewHelper GetEmojiTextViewHelper()
		{
			if (mEmojiTextViewHelper == null)
			{
				mEmojiTextViewHelper = new EmojiTextViewHelper(this);
			}
			return mEmojiTextViewHelper;
		}
	}
}
