namespace TouchWalkthrough
{
	using System.Collections.Generic;
	using System.Linq;

	using Android.App;
	using Android.Content;
	using Android.Views;
	using Android.Widget;

	using Java.Lang;

	/// <summary>
	/// This adapter holds the names of activities in the application that can be 
	/// launched from the main screen, the intent that can be used to launch them.
	/// </summary>
	public class MainMenuAdapter : BaseAdapter
	{
		private  readonly Dictionary<string, Intent> _gestureActivities;
		private readonly Activity _activity;

		public MainMenuAdapter (Activity activity)
		{
			_activity = activity;
			_gestureActivities = new Dictionary<string, Intent>
                                     {
                                         { "Touch Sample", new Intent(activity, typeof(TouchActivity)) }, 
                                         { "Gesture Recognizer", new Intent(activity, typeof(GestureRecognizerActivity)) },
                                         { "Custom Gesture", new Intent(activity, typeof(CustomGestureRecognizerActivity))}
                                     };
		}

		public override int Count { get { return _gestureActivities.Count; } }
        
		/// <summary>
		/// Returns an intent that can be used to start the activity to display the appropriate example.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public override Object GetItem (int position)
		{
			return _gestureActivities.Values.ElementAt (position);
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TextView result = convertView as TextView;
			if (result == null) {
				result = new TextView (_activity)
                             {
                                 TextSize = 28.0f,
                                 Text = _gestureActivities.Keys.ElementAt(position)
                             };
				result.SetPadding (0, 15, 0, 15);
			}

			return result;
		}
	}
}