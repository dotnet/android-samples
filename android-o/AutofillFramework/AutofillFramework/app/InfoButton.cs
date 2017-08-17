using System;
using Android.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;

namespace AutofillFramework.app
{
	public class InfoButton : AppCompatImageButton
	{
		public InfoButton(Context context) : this (context, null) {}

		public InfoButton(Context context, IAttributeSet attrs) : this (context, attrs, 0){}

		public InfoButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.InfoButton,
					defStyleAttr, 0);
			var infoText = typedArray.GetString(Resource.Styleable.InfoButton_dialogText);
			typedArray.Recycle();
			SetInfoText(infoText);
		}

		public void SetInfoText(string infoText)
		{
			Click += (sender, e) => {
				new AlertDialog.Builder(Context)
						.SetMessage(infoText).Create().Show();
			};
    	}
	}
}
