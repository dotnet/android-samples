using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.AppCompat.Widget;

namespace AutofillFramework
{
    public class InfoButton : AppCompatImageButton
    {
        public InfoButton(Context context) : this(context, null)
        {
        }

        public InfoButton(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public InfoButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.InfoButton, defStyleAttr, 0);
            var infoText = typedArray.GetString(Resource.Styleable.InfoButton_dialogText);
            typedArray.Recycle();
            SetInfoText(infoText);
        }

        public void SetInfoText(string infoText)
        {
            Click += delegate
            {
                new AlertDialog.Builder(Context)
                    .SetMessage(infoText).Create().Show();
            };
        }
    }
}