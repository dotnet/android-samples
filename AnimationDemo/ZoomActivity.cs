namespace com.xamarin.evolve2013.animationsdemo
{
    using System;
    using System.Collections.Generic;

    using Android.Animation;
    using Android.App;
    using Android.Graphics;
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using Android.Views.Animations;
    using Android.Widget;

    /// <summary>
    ///   This activity is an extra example of animation using the ObjectAnimator.
    /// </summary>
    [Activity(Label = "@string/title_zoom", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class ZoomActivity : Activity
    {
        private Animator _currentAnimator;
        private int _shortAnimationDuration;
        private Dictionary<int, AnimatorSet> _expandingAnimators;
        private Dictionary<int, AnimatorSet> _shrinkingAnimators; 

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_zoom);

            _shortAnimationDuration = Resources.GetInteger(Android.Resource.Integer.ConfigShortAnimTime);
            _expandingAnimators = new Dictionary<int, AnimatorSet>(2);
            _shrinkingAnimators = new Dictionary<int, AnimatorSet>(2);

            View thumb1View = FindViewById(Resource.Id.thumb_button_1);
            thumb1View.Tag = Resource.Drawable.image1;
            thumb1View.Click += ZoomImageFromThumb;

            View thumb2View = FindViewById(Resource.Id.thumb_button_2);
            thumb2View.Tag = Resource.Drawable.image2;
            thumb2View.Click += ZoomImageFromThumb;

        }

        /// <summary>
        ///   This method will determine the scaling ratio for the image view.
        /// </summary>
        /// <param name="startBounds">The visible rectangle of the thumbnail.</param>
        /// <param name="finalBounds">The visible rectangle of the expanded image view.</param>
        /// <returns></returns>
        private static float CalculateStartScale(Rect startBounds, Rect finalBounds)
        {
            float startScale;
            // First figure out width-to-height ratio of each rectangle.
            float finalBoundsRatio = finalBounds.Width() / (float)finalBounds.Height();
            float startBoundsRatio = startBounds.Width() / (float)startBounds.Height();

            if (finalBoundsRatio > startBoundsRatio)
            {
                // Extend start bounds horizontally
                startScale = (float)startBounds.Height() / finalBounds.Height();
                float startWidth = startScale * finalBounds.Width();
                float deltaWidth = (startWidth - startBounds.Width()) / 2;
                startBounds.Left -= (int)deltaWidth;
                startBounds.Right += (int)deltaWidth;
            }
            else
            {
                // Extend start bounds vertically
                startScale = (float)startBounds.Width() / finalBounds.Width();
                float startHeight = startScale * finalBounds.Height();
                float deltaHeight = (startHeight - startBounds.Height()) / 2;
                startBounds.Top -= (int)deltaHeight;
                startBounds.Bottom += (int)deltaHeight;
            }
            return startScale;
        }

        /// <summary>
        ///   Builds the AnimatorSet that will create the expanding animation - the user will perceive the thumbnail getting bigger.
        /// </summary>
        /// <param name="expandedView">This is the ImageView that the thumbnail will scale up to.</param>
        /// <param name="startBounds">The visible rectangle of the thumbnail (global coordinates).</param>
        /// <param name="finalBounds">The visible rectangle of the full sized image (global coordinates).</param>
        /// <param name="startScale"></param>
        /// <returns></returns>
        private AnimatorSet BuildExpandingAnimatorSet(ImageView expandedView, Rect startBounds, Rect finalBounds, float startScale)
        {
			// Each expanding animator is unique to the start location - we'll cache the AnimatorSet
			// instance based on the starting location.
			int key = startBounds.GetHashCode ();
            if (_expandingAnimators.ContainsKey(key))
            {
                return _expandingAnimators[key];
            }

            AnimatorSet expandSet = new AnimatorSet();
            expandSet.Play(ObjectAnimator.OfFloat(expandedView, View.X, startBounds.Left, finalBounds.Left))
                     .With(ObjectAnimator.OfFloat(expandedView, View.Y, startBounds.Top, finalBounds.Top))
                     .With(ObjectAnimator.OfFloat(expandedView, "ScaleX", startScale, 1f))
                     .With(ObjectAnimator.OfFloat(expandedView, "ScaleY", startScale, 1f));
            expandSet.SetDuration(_shortAnimationDuration);
            expandSet.SetInterpolator(new DecelerateInterpolator());
            expandSet.AnimationEnd += NullOutCurrentAnimator;
            expandSet.AnimationCancel += NullOutCurrentAnimator;

            _expandingAnimators.Add(key, expandSet);
            return expandSet;
        }

        private void NullOutCurrentAnimator(object sender, EventArgs eventArgs)
        {
            if (_currentAnimator == null)
            {
                return;
            }
            _currentAnimator = null;
        }

        /// <summary>
        ///   Builds the AnimatorSet to shrink the full sized image back to the thumbnail.
        /// </summary>
        /// <param name="bigView">The full sized view.</param>
        /// <param name="thumbView">The thumbnail view.</param>
        /// <param name="startBounds">The visible rectangle of the thumbnail when it is visible.</param>
        /// <param name="scale">Scale ratio.</param>
        /// <returns></returns>
        private AnimatorSet BuildShrinkingAnimatorSet(View bigView, View thumbView, Rect startBounds, float scale)
        {
            if (_shrinkingAnimators.ContainsKey(thumbView.Id))
            {
                return _shrinkingAnimators[thumbView.Id];
            }

            AnimatorSet shrinkSet = new AnimatorSet();
            shrinkSet.Play(ObjectAnimator.OfFloat(bigView, View.X, startBounds.Left))
                     .With(ObjectAnimator.OfFloat(bigView, View.Y, startBounds.Top))
                     .With(ObjectAnimator.OfFloat(bigView, "ScaleX", scale))
                     .With(ObjectAnimator.OfFloat(bigView, "ScaleY", scale));
            shrinkSet.SetDuration(_shortAnimationDuration);
            shrinkSet.SetInterpolator(new DecelerateInterpolator());
            shrinkSet.AnimationEnd += (sender1, args1) =>{
                thumbView.Alpha = 1.0f;
                bigView.Visibility = ViewStates.Gone;
                NullOutCurrentAnimator(sender1, args1);
            };

            shrinkSet.AnimationCancel += (sender1, args1) =>{
                thumbView.Alpha = 1.0f;
                bigView.Visibility = ViewStates.Gone;
                NullOutCurrentAnimator(sender1, args1);
            };

            _shrinkingAnimators.Add(thumbView.Id, shrinkSet);
            return shrinkSet;
        }


        /// <summary>
        ///   Retrieves a reference to the ImageView that will hold the expanded picture.
        /// </summary>
        /// <remarks>
        ///   The resource id of the expanded image is kept in the tag property of the thumbnail, this
        ///   is why we need a reference to the thumnale we're expanding.
        /// </remarks>
        /// <param name="thumbView"></param>
        /// <returns></returns>
        private ImageView GetExpandedImageView(View thumbView)
        {
            ImageView expandedImageView = FindViewById<ImageView>(Resource.Id.expanded_image);
            int finalImageResourceId = (int)thumbView.Tag; // In this example we store the resource id of the big image in the tag of the thumbnail.
            expandedImageView.SetImageResource(finalImageResourceId);

            // Hide the thumbnail and show the zoomed-in view. When the animation begins,
            // it will position the zoomed-in view in the place of the thumbnail.
            thumbView.Alpha = 0f;
            expandedImageView.Visibility = ViewStates.Visible;

            // Set the pivot point for SCALE_X and SCALE_Y transformations to the top-left corner of
            // the zoomed-in view (the default is the center of the view).
            expandedImageView.PivotX = 0f;
            expandedImageView.PivotY = 0f;
            return expandedImageView;
        }

        /// <summary>
        ///   The event handler for the thumbnails. Called when they are clicked, it display
        ///   the full sized image, animating the transition from thumbnail to full sized image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ZoomImageFromThumb(object sender, EventArgs eventArgs)
        {
            View thumbView = (View)sender;
            ImageView expandedImageView = GetExpandedImageView(thumbView);

            if (_currentAnimator != null)
            {
                _currentAnimator.Cancel();
            }
            Rect startBounds = new Rect();
            Rect finalBounds = new Rect();
            Point globalOffset = new Point();

            // The start bounds are the global visible rectangle of the thumbnail
            thumbView.GetGlobalVisibleRect(startBounds);

            // The final bounds are the global visible rectangle of the container view. Also
            // set the container view's offset as the origin for the bounds, since that's
            // the origin for the positioning animation properties (X, Y).
            FindViewById(Resource.Id.container).GetGlobalVisibleRect(finalBounds, globalOffset);
            startBounds.Offset(-globalOffset.X, -globalOffset.Y);
            finalBounds.Offset(-globalOffset.X, -globalOffset.Y);

            float startScale = CalculateStartScale(startBounds, finalBounds);

            // Construct and run the parallel animation of the four translation and scale properties
            // (X, Y, SCALE_X, and SCALE_Y).
            AnimatorSet expandSet = BuildExpandingAnimatorSet(expandedImageView, startBounds, finalBounds, startScale);
            expandSet.Start();
            _currentAnimator = expandSet;

            // Upon clicking the zoomed image, it should zoom back down to the original bounds
            // and show the thumbnail instead of the expanded image.
            expandedImageView.Click += (o, args) =>{
                if (_currentAnimator != null)
                {
                    _currentAnimator.Cancel();
                }

                AnimatorSet shrinkSet = BuildShrinkingAnimatorSet(expandedImageView, thumbView, startBounds, startScale);
                shrinkSet.Start();
                _currentAnimator = shrinkSet;
            };
        }
    }
}
