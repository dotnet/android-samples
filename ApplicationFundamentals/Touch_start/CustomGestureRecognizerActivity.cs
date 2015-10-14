namespace TouchWalkthrough
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Android.App;
    using Android.Gestures;
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using Android.Widget;

    /// <summary>
    ///   This activity will use a gesture to change the image that is displayed on the screen.
    /// </summary>
    [Activity(Label = "@string/activity_custom_gesture_recognizer")]
    public class CustomGestureRecognizerActivity : Activity
    {
/*
        private GestureLibrary _gestureLibrary;
        private ImageView _imageView;
*/

/*        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // This activity will use a GestureOverlayView as it's content view. The layout file
            // will be added as a subview of this.
            GestureOverlayView gestureOverlayView = new GestureOverlayView(this);
            SetContentView(gestureOverlayView);
            gestureOverlayView.GesturePerformed += GestureOverlayViewOnGesturePerformed;

            // Load up the layout file for this activity and add it as child view of the 
            // GestureOverlayView
            View view = LayoutInflater.Inflate(Resource.Layout.custom_gesture_layout, null);
            _imageView = view.FindViewById<ImageView>(Resource.Id.imageView1);
            gestureOverlayView.AddView(view);

            // Load the binary gesture file that we created.
            _gestureLibrary = GestureLibraries.FromRawResource(this, Resource.Raw.gestures);
            if (!_gestureLibrary.Load())
            {
                Log.Wtf(GetType().FullName, "There was a problem loading the gesture library.");
                Finish();
            }
        }*/

/*
        private void GestureOverlayViewOnGesturePerformed(object sender, GestureOverlayView.GesturePerformedEventArgs gesturePerformedEventArgs)
        {
            IEnumerable<Prediction> predictions = from p in _gestureLibrary.Recognize(gesturePerformedEventArgs.Gesture)
                                                  orderby p.Score descending
                                                  where p.Score > 1.0
                                                  select p;
            Prediction prediction = predictions.FirstOrDefault();

            if (prediction == null)
            {
                Log.Debug(GetType().FullName, "Nothing seemed to match the user's gesture, so don't do anything.");
                return;
            }

            Log.Debug(GetType().FullName, "Using the prediction named {0} with a score of {1}.", prediction.Name, prediction.Score);

            if (prediction.Name.StartsWith("checkmark"))
            {
                _imageView.SetImageResource(Resource.Drawable.checked_me);
            }
            else if (prediction.Name.StartsWith("erase", StringComparison.OrdinalIgnoreCase))
            {
                // Match one of our "erase" gestures
                _imageView.SetImageResource(Resource.Drawable.check_me);
            }
        }
*/
    }
}
