using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;

namespace AutofillFramework
{
    public class NavigationItem : FrameLayout
    {
        public CardView mCardView;

        protected NavigationItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public NavigationItem(Context context) : this(context, null)
        {
        }

        public NavigationItem(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public NavigationItem(Context context, IAttributeSet attrs, int defStyleAttr) : this(context, attrs,
            defStyleAttr, 0)
        {
        }

        public NavigationItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.NavigationItem,
                defStyleAttr, defStyleRes);
            var labelText = typedArray.GetString(Resource.Styleable.NavigationItem_labelText);
            var infoText = typedArray.GetString(Resource.Styleable.NavigationItem_infoText);
            var logoDrawable = typedArray.GetDrawable(Resource.Styleable.NavigationItem_itemLogo);
            var colorRes = typedArray.GetResourceId(Resource.Styleable.NavigationItem_imageColor, 0);
            int imageColor = ContextCompat.GetColor(Context, colorRes);
            typedArray.Recycle();
            var rootView = LayoutInflater.From(context).Inflate(Resource.Layout.navigation_item, this);
            if (logoDrawable != null)
            {
                logoDrawable.SetColorFilter(new Color(imageColor), PorterDuff.Mode.SrcIn);
            }
            var buttonLabel = rootView.FindViewById<TextView>(Resource.Id.buttonLabel);
            buttonLabel.Text = labelText;
            buttonLabel.SetCompoundDrawablesRelativeWithIntrinsicBounds(logoDrawable, null, null, null);
            var infoButton = rootView.FindViewById<InfoButton>(Resource.Id.infoButton);
            infoButton.SetInfoText(infoText);
            infoButton.SetColorFilter(new Color(imageColor));
            mCardView = rootView.FindViewById<CardView>(Resource.Id.cardView);
        }

        public void SetNavigationButtonClickListener([Nullable] IOnClickListener l)
        {
            mCardView.SetOnClickListener(l);
        }
    }
}