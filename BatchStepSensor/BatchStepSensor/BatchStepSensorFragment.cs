
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BatchStepSensor.CardStream;
using Android.Content.PM;
using Android.Hardware;

namespace BatchStepSensor
{
	public class BatchStepSensorFragment : Android.Support.V4.App.Fragment, OnCardClickListener
	{

		public const string TAG = "StepSensorSample";
		// Cards
		private CardStreamFragment mCards = null;

		// Card tags
		public const string CARD_INTRO = "intro";
		public const string CARD_REGISTER_DETECTOR = "register_detector";
		public const string CARD_REGISTER_COUNTER = "register_counter";
		public const string CARD_BATCHING_DESCRIPTION = "register_batching_description";
		public const string CARD_COUNTING = "counting";
		public const string CARD_EXPLANATION = "explanation";
		public const string CARD_NOBATCHSUPPORT = "error";

		// Actions from REGISTER cards
		public const int ACTION_REGISTER_DETECT_NOBATCHING = 10;
		public const int ACTION_REGISTER_DETECT_BATCHING_5s = 11;
		public const int ACTION_REGISTER_DETECT_BATCHING_10s = 12;
		public const int ACTION_REGISTER_COUNT_NOBATCHING = 21;
		public const int ACTION_REGISTER_COUNT_BATCHING_5s = 22;
		public const int ACTION_REGISTER_COUNT_BATCHING_10s = 23;
		// Action from COUNTING card
		public const int ACTION_UNREGISTER = 1;
		// Actions from description cards
		private const int ACTION_BATCHING_DESCRIPTION_DISMISS = 2;
		private const int ACTION_EXPLANATION_DISMISS = 3;

		// State of application, used to register for sensors when app is restored
		public const int STATE_OTHER = 0;
		public const int STATE_COUNTER = 1;
		public const int STATE_DETECTOR = 2;

		// Bundle tags used to store data when restoring application state
		private const string BUNDLE_STATE = "state";
		private const string BUNDLE_LATENCY = "latency";
		private const string BUNDLE_STEPS = "steps";

		// max batch latency is specified in microseconds
		private const int BATCH_LATENCY_0 = 0; // no batching
		private const int BATCH_LATENCY_10s = 10000000;
		private const int BATCH_LATENCY_5s = 5000000;

		/*
    For illustration we keep track of the last few events and show their delay from when the
    event occurred until it was received by the event listener.
    These variables keep track of the list of timestamps and the number of events.
     */
		// Number of events to keep in queue and display on card
		private const int EVENT_QUEUE_LENGTH = 10;
		// List of timestamps when sensor events occurred
		private float[] mEventDelays = new float[EVENT_QUEUE_LENGTH];

		// number of events in event list
		private int mEventLength = 0;
		// pointer to next entry in sensor event list
		private int mEventData = 0;

		// Steps counted in current session
		private int mSteps = 0;
		// Value of the step counter sensor when the listener was registered.
		// (Total steps are calculated from this value.)
		private int mCounterSteps = 0;
		// Steps counted by the step counter previously. Used to keep counter consistent across rotation
		// changes
		private int mPreviousCounterSteps = 0;
		// State of the app (STATE_OTHER, STATE_COUNTER or STATE_DETECTOR)
		private int mState = STATE_OTHER;
		// When a listener is registered, the batch sensor delay in microseconds
		private int mMaxDelay = 0;

		ISensorEventListener mListener;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);


			mListener = new SensorEventListener () {
				OnSensorChangedAction = (SensorEvent e) =>
				{
					// Store the delay of this event
					RecordDelay(e);
					string delayString = DelayString;

					if (e.Sensor.Type == SensorType.StepDetector) {
						// A step detector event is received for each step.
						// This means we need to count steps ourselves

						mSteps += e.Values.Count;

						// Update the card with the latest step count
						CardStream.GetCard(CARD_COUNTING).SetTitle(GetString(Resource.String.counting_title, new Java.Lang.Integer(mSteps)))
							.SetDescription(GetString(Resource.String.counting_description,
								GetString(Resource.String.sensor_detector), new Java.Lang.Integer(mMaxDelay), new Java.Lang.String(delayString)));
						Log.Info(TAG, "New step detected by STEP_DETECTOR sensor. Total step count: " + mSteps);
					} else if (e.Sensor.Type == SensorType.StepCounter) {

						// A step counter event contains the total number of steps since the listener was first registered.
						// We need to keep track of this initial value to calculate the number of steps taken,
						// as the first value a listener receives is undefined

						if (mCounterSteps < 1) {
							// Initial value
							mCounterSteps = (int) e.Values[0];
						}

						// Calculate stes taken based on first counter value received
						mSteps = (int)e.Values[0] - mCounterSteps;

						// Add the number of steps previously taken, otherwise the counter would start as 0.
						// This is needed to keep the counter consistent across rotation changes.
						mSteps += mPreviousCounterSteps;

						// Update the card with the latest step count
						CardStream.GetCard(CARD_COUNTING).SetTitle(GetString(Resource.String.counting_title, new Java.Lang.Integer(mSteps)))
							.SetDescription(GetString(Resource.String.counting_description,
								GetString(Resource.String.sensor_counter), new Java.Lang.Integer(mMaxDelay), new Java.Lang.String(delayString)));
						Log.Info(TAG, "New step detected by STEP_COUNTER sensor. Total step count: " + mSteps);

					}
				}
			};
		}
		public override void OnResume ()
		{
			base.OnResume ();
			CardStreamFragment stream = CardStream;
			if (stream.VisibleCardCount < 1) {
				// No cards are visible, started for the first time
				// Prepare all cards and show the intro card.
				InitializeCards ();
				ShowIntroCard ();

				if (IsKitKatWithStepSensor)
					ShowRegisterCard ();
				else
					ShowErrorCard ();
			}
		}
		public override void OnPause ()
		{
			base.OnPause ();


			// Unregister the listener when the application is paused
			UnregisterListeners ();
		}

		/// <summary>
		/// Returns if this device is supported. It needs to be running Android KitKay (4.4) or higher, 
		/// with a step counter and step detector sensor.
		/// </summary>
		/// <value>True if the device can run this sample</value>
		private bool IsKitKatWithStepSensor
		{
			get {
				// Require at least Android KitKit
				BuildVersionCodes currentApiVersion = Android.OS.Build.VERSION.SdkInt;
				// Check that the device supports the step counter and detector sensors
				PackageManager packageManager = Activity.PackageManager;
				return currentApiVersion >= BuildVersionCodes.Kitkat
				&& packageManager.HasSystemFeature (PackageManager.FeatureSensorStepCounter)
				&& packageManager.HasSystemFeature (PackageManager.FeatureSensorStepDetector);
			}
		}

		/// <summary>
		/// Handles a click on a card action.
		/// Registers a SensorEventListener with the selected delay, dismisses cards or unregisters the listener.
		/// Actions are defined when a card is created
		/// </summary>
		/// <param name="cardActionId"></param>
		/// <param name="cardTag"></param>
		public void OnCardClick (int cardActionId, string cardTag)
		{
			switch (cardActionId) {
			// Register Step Counter card
			case ACTION_REGISTER_COUNT_NOBATCHING:
				RegisterEventListener(BATCH_LATENCY_0, SensorType.StepCounter);
				break;
			case ACTION_REGISTER_COUNT_BATCHING_5s:
				RegisterEventListener(BATCH_LATENCY_5s, SensorType.StepCounter);
				break;
			case ACTION_REGISTER_COUNT_BATCHING_10s:
				RegisterEventListener(BATCH_LATENCY_10s, SensorType.StepCounter);
				break;

				// Register Step Detector card
			case ACTION_REGISTER_DETECT_NOBATCHING:
				RegisterEventListener(BATCH_LATENCY_0, SensorType.StepDetector);
				break;
			case ACTION_REGISTER_DETECT_BATCHING_5s:
				RegisterEventListener(BATCH_LATENCY_5s, SensorType.StepDetector);
				break;
			case ACTION_REGISTER_DETECT_BATCHING_10s:
				RegisterEventListener(BATCH_LATENCY_10s, SensorType.StepDetector);
				break;

				// Unregister card
			case ACTION_UNREGISTER:
				ShowRegisterCard();
				UnregisterListeners();
				// reset the application state when explicitly unregistered
				mState = STATE_OTHER;
				break;

				// Explanation cards
			case ACTION_BATCHING_DESCRIPTION_DISMISS:
				// permanently remove the batch description card, it will not be shown again
				CardStream.RemoveCard(CARD_BATCHING_DESCRIPTION);
				break;
			case ACTION_EXPLANATION_DISMISS:
				// permanently remove the explanation card, it will not be shown again
				CardStream.RemoveCard (CARD_EXPLANATION);
				break;
			}
			if (cardTag == CARD_REGISTER_COUNTER || cardTag == CARD_REGISTER_DETECTOR)
				ShowCountingCards();
		}

		/// <summary>
		/// Register a SensorEventListener for the sensor and max batch delay.
		/// The maximum batch delay specifies the maximum duration in microseconds for which subsequent sensor events can be temporarily stored
		/// before they are delivered to the registered SensorEventListener.
		/// </summary>
		/// <param name="maxDelay">Max delay.</param>
		/// <param name="sensorType">Sensor type.</param>
		void RegisterEventListener(int maxDelay, SensorType sensorType)
		{
			// Keep track of the state so that the correct sensor type and batch delay can be set up when the app is restored (for example, on screen rotation)

			mMaxDelay = maxDelay;

			if (sensorType == SensorType.StepCounter) {
				mState = STATE_COUNTER;
				// Reset the initial step counter calue, the first event received by the event listener is
				// stored in mCounterSteps and used to calculate the total number of steps taken.
				mCounterSteps = 0;
				Log.Info (TAG, "Event listener for step counter sensor registered with a max delay of " + mMaxDelay);
			} else {
				mState = STATE_DETECTOR;
				Log.Info (TAG, "Event listener for step detector sensor registered with a max delay of " + mMaxDelay);
			}

			// Get the default sensor for the sensor type from the SensorManager
			SensorManager sensorManager = (SensorManager)Activity.GetSystemService (Service.SensorService);
			// sensorType is either SensorType.StepCounter or SensorType.StepDetector
			Sensor sensor = sensorManager.GetDefaultSensor (sensorType);

			// Register the listener for this sensor in batch mode.
			// If the max delay is 0, events will be delivered in continuous mode without batching.
			bool batchMode = sensorManager.RegisterListener (mListener, sensor, SensorDelay.Normal, maxDelay);

			if (!batchMode) {
				// Batch mode could not be enabled, show a warning message and switch to continuous mode
				CardStream.GetCard (CARD_NOBATCHSUPPORT).SetDescription(GetString (Resource.String.warning_nobatching));
				CardStream.ShowCard (CARD_NOBATCHSUPPORT);
				Log.Warn (TAG, "Could not register sensor listener in batch mode, falling back to continuous mode.");
			}

			if (maxDelay > 0 && batchMode) {
				// Batch mode was enabled successfully, show a description card
				CardStream.ShowCard (CARD_BATCHING_DESCRIPTION);
			}

			// Show the explanation card
			CardStream.ShowCard (CARD_EXPLANATION);
		}

		/// <summary>
		/// Unregisters the sensor listener if it is registered
		/// </summary>
		void UnregisterListeners()
		{
			SensorManager sensorManager =
				(SensorManager)Activity.GetSystemService (Service.SensorService);
			sensorManager.UnregisterListener (mListener);
			Log.Info (TAG, "Sensor listener unregister.");
		}

		/// <summary>
		/// Resets the step counter by clearing all counting variables and lists
		/// </summary>
		void ResetCounter() 
		{
			mSteps = 0;
			mCounterSteps = 0;
			mEventLength = 0;
			mEventDelays = new float[EVENT_QUEUE_LENGTH];
			mPreviousCounterSteps = 0;
		}

		/// <summary>
		/// Records the delay for the event.
		/// </summary>
		/// <param name="e"></param>
		private void RecordDelay(SensorEvent e)
		{
			// Calculate the delay from when event was recorded until it was received here in ms
			// Event timestamp is recorded in us accuracy, but ms accuracy is sufficient here
			mEventDelays [mEventData] = Java.Lang.JavaSystem.CurrentTimeMillis() - e.Timestamp;

			// Increment length counter
			mEventLength = Math.Min (EVENT_QUEUE_LENGTH, mEventLength + 1);
			// Move pointer to the next (oldest) location
			mEventData = (mEventData + 1) % EVENT_QUEUE_LENGTH;
		}

		StringBuilder mDelayStringBuilder = new StringBuilder();

		/// <summary>
		/// Returns a string describing the sensor delays recording in RecordDelay
		/// </summary>
		/// <value>The delay string.</value>
		private String DelayString
		{
			get {
				// Empty the StringBuilder
				mDelayStringBuilder.Length = 0;

				// Loop over all recorded delays and appent them to the buffer as a decimal
				for (int i = 0; i < mEventLength; i++) {
					if (i > 0) {
						mDelayStringBuilder.Append (", ");
					}
					int index = (mEventData + i) % EVENT_QUEUE_LENGTH;
					float delay = mEventDelays [index] / 1000f; // Convert delay from milliseconds into seconds
					mDelayStringBuilder.Append (String.Format ("{0}", delay));
				}
				return mDelayStringBuilder.ToString ();
			}
		}

		/// <Docs>Bundle in which to place your saved state.</Docs>
		/// <summary>
		/// Records the state of the application into the Android.OS.Bundle
		/// </summary>
		/// <param name="outState">Out state.</param>
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			// Store all variables required to restore the state of the application
			outState.PutInt (BUNDLE_LATENCY, mMaxDelay);
			outState.PutInt (BUNDLE_STATE, mState);
			outState.PutInt (BUNDLE_STEPS, mSteps);
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			// Fragment is being restored, reinitialise its state with data from the bundle
			if (savedInstanceState != null) {
				ResetCounter ();
				mSteps = savedInstanceState.GetInt (BUNDLE_STEPS);
				mState = savedInstanceState.GetInt (BUNDLE_STATE);
				mMaxDelay = savedInstanceState.GetInt (BUNDLE_LATENCY);

				// Register listeners again if in detector or counter states with restored delay
				if (mState == STATE_DETECTOR) {
					RegisterEventListener (mMaxDelay, SensorType.StepDetector);
				} else if (mState == STATE_COUNTER) {
					// Stpre the previous number of steps to keep counter count consistent
					mPreviousCounterSteps = mSteps;
					RegisterEventListener (mMaxDelay, SensorType.StepCounter);
				}
			}
		}

		/// <summary>
		/// Hides the registration cards, reset the counter and show the step counting card
		/// </summary>
		private void ShowCountingCards()
		{
			// Hide the registration cards
			CardStream.HideCard (CARD_REGISTER_DETECTOR);
			CardStream.HideCard (CARD_REGISTER_COUNTER);

			// show the explanation card if it has not been dismissed
			CardStream.ShowCard (CARD_EXPLANATION);

			// Reset the step counter, then show the step counting card
			ResetCounter ();

			// Set the initial text for the step counting card before a step is recorded
			String sensor = "-";
			if (mState == STATE_COUNTER) {
				sensor = GetString (Resource.String.sensor_counter);
			} else if (mState == STATE_DETECTOR) {
				sensor = GetString (Resource.String.sensor_detector);
			}

			// Set initial text
			CardStream.GetCard(CARD_COUNTING)
				.SetTitle(GetString(Resource.String.counting_title, 0))
				.SetDescription(GetString(Resource.String.counting_description, sensor, mMaxDelay, "-"));

			// Show the counting card and make it undismissible
			CardStream.ShowCard (CARD_COUNTING, false);
		}

		/// <summary>
		/// Show the introduction card
		/// </summary>
		private void ShowIntroCard() 
		{
			Card c = new Card.Builder (this, CARD_INTRO)
				.SetTitle (GetString (Resource.String.intro_title))
				.SetDescription (GetString (Resource.String.intro_message))
				.Build (Activity);
			CardStream.AddCard (c, true);
		}

		/// <summary>
		/// Show two registration cards, one for the step detector and counter sensors
		/// </summary>
		private void ShowRegisterCard()
		{
			// Hide the counting and explanation cards
			CardStream.HideCard (CARD_BATCHING_DESCRIPTION);
			CardStream.HideCard (CARD_EXPLANATION);
			CardStream.HideCard (CARD_COUNTING);

			// Show two undismissable registration cards, one for each step sensor
			CardStream.ShowCard (CARD_REGISTER_DETECTOR, false);
			CardStream.ShowCard (CARD_REGISTER_COUNTER, false);
		}

		/// <summary>
		/// Shows the error card.
		/// </summary>
		private void ShowErrorCard()
		{
			CardStream.ShowCard (CARD_NOBATCHSUPPORT, false);
		}

		/// <summary>
		/// Initializes the cards.
		/// </summary>
		private void InitializeCards()
		{
			// Step counting
			Card c = new Card.Builder (this, CARD_COUNTING)
				.SetTitle ("Steps")
				.SetDescription ("")
				.AddAction ("Unregister Listener", ACTION_UNREGISTER, Card.ACTION_NEGATIVE)
				.Build(Activity);
			CardStream.AddCard (c);

			// Register step detector listener
			c = new Card.Builder (this, CARD_REGISTER_DETECTOR)
				.SetTitle (GetString (Resource.String.register_detector_title))
				.SetDescription (GetString (Resource.String.register_detector_description))
				.AddAction (GetString (Resource.String.register_0), ACTION_REGISTER_DETECT_NOBATCHING, Card.ACTION_NEUTRAL)
				.AddAction (GetString (Resource.String.register_5), ACTION_REGISTER_DETECT_BATCHING_5s, Card.ACTION_NEUTRAL)
				.AddAction (GetString (Resource.String.register_10), ACTION_REGISTER_DETECT_BATCHING_10s, Card.ACTION_NEUTRAL)
				.Build (Activity);
			CardStream.AddCard (c);

			// Register step count listener
			c = new Card.Builder (this, CARD_REGISTER_COUNTER)
				.SetTitle (GetString (Resource.String.register_counter_title))
				.SetDescription (GetString (Resource.String.register_counter_description))
				.AddAction (GetString (Resource.String.register_0), ACTION_REGISTER_COUNT_NOBATCHING, Card.ACTION_NEUTRAL)
				.AddAction (GetString (Resource.String.register_5), ACTION_REGISTER_COUNT_BATCHING_5s, Card.ACTION_NEUTRAL)
				.AddAction (GetString (Resource.String.register_10), ACTION_REGISTER_COUNT_BATCHING_10s, Card.ACTION_NEUTRAL)
				.Build (Activity);
			CardStream.AddCard (c);

			// Batching description
			c = new Card.Builder (this, CARD_BATCHING_DESCRIPTION)
				.SetTitle (GetString (Resource.String.batching_queue_title))
				.SetDescription (GetString (Resource.String.batching_queue_description))
				.AddAction (GetString (Resource.String.action_notagain), ACTION_BATCHING_DESCRIPTION_DISMISS, Card.ACTION_POSITIVE)
				.Build (Activity);
			CardStream.AddCard (c);

			// Explanation
			c = new Card.Builder (this, CARD_EXPLANATION)
				.SetDescription (GetString (Resource.String.explanation_description))
				.AddAction (GetString (Resource.String.action_notagain), ACTION_EXPLANATION_DISMISS, Card.ACTION_POSITIVE)
				.Build (Activity);
			CardStream.AddCard (c);

			// Error
			c = new Card.Builder (this, CARD_NOBATCHSUPPORT)
				.SetTitle (GetString (Resource.String.error_title))
				.SetDescription (GetString (Resource.String.error_nosensor))
				.Build (Activity);
			CardStream.AddCard (c);
		}

		/// <summary>
		/// Gets the cached CardStreamFragment used to show cards
		/// </summary>
		/// <value>The card stream.</value>
		CardStreamFragment CardStream 
		{
			get {
				if (mCards == null)
					mCards = ((BatchStepSensor.CardStream.CardStream)Activity).GetCardStream();
				return mCards;
			}
		}
	}
}

