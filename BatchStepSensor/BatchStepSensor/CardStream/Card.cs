using System;
using Android.Views;
using Android.Widget;
using Android.Animation;
using System.Collections.Generic;
using Android.Graphics;
using Android.App;

namespace BatchStepSensor.CardStream
{
	public class Card
	{
		public const int ACTION_POSITIVE = 1;
		public const int ACTION_NEGATIVE = 2;
		public const int ACTION_NEUTRAL = 3;

		public const int PROGRESS_TYPE_NO_PROGRESS = 0;
		public const int PROGRESS_TYPE_NORMAL = 1;
		public const int PROGRESS_TYPE_INDETERMINATE = 2;
		public const int PROGRESS_TYPE_LABEL = 3;

		private OnCardClickListener mClickListener;


		// The card model contains a reference to its desired layout (for extensibility), title,
		// description, zero to many action buttons, and zero or 1 progress indicators.
		private int mLayoutId = Resource.Layout.card;
        //Tag that uniquely identifies this card.
		private String mTag = null;

		private String mTitle = null;
		private String mDescription = null;

		private View mCardView = null;
		private View mOverlayView = null;
		private TextView mTitleView = null;
		private TextView mDescView = null;
		private View mActionAreaView = null;

		private Animator mOngoingAnimator = null;

        /// <summary>
		/// Visual state, either ARD_STATE_NORMAL, CARD_STATE_FOCUSED or CARD_STATE_INACTIVE
        /// </summary>
		private int mCardState = CARD_STATE_NORMAL;
		public const int CARD_STATE_NORMAL = 1;
		public const int CARD_STATE_FOCUSED = 2;
		public const int CARD_STATE_INACTIVE = 3;

		/// <summary>
		/// Represents actions that can be taken from the card. Statistically the developer can designate the action a positive, negative (ok/cancel, for instance), or neutral.
		/// </summary>
		private List<CardAction> mCardActions = new List<CardAction>();

		/// <summary>
		/// Some cards will have a sense of "progress which should be associated with, but separated from its "parent" card. 
		/// To push for simplicity in samples, Cards are designed to have a maximum of one progress indicator per Card.
		/// </summary>
		private CardProgress mCardProgress = null;
		public String Tag 
		{
			get { return mTag; }
		}
		public View View { get { return mCardView; } }

		public Card SetDescription(string desc)
		{
			if (mDescView != null) {
				mDescription = desc;
				mDescView.Text = desc;
			}
			return this;
		}

		public Card SetTitle(string desc)
		{
			if (mTitleView != null) {
				mTitle = desc;
				mTitleView.Text = desc;
			}
			return this;
		}

		/// <summary>
		/// Gets or sets the UI state
		/// </summary>
		/// <value>CARD_STATE_NORMAL, CARD_STATE_FOCUSED or CARD_STATE_INACTIVE</value>
		public int State 
		{ 
			get { return mCardState; }
			set { SetState (value); }
		}

		/// <summary>
		/// Set the UI state/
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="state">Either CARD_STATE_NORMAL, CARD_STATE_FOCUSED or CARD_STATE_INACTIVE</param>
		public Card SetState(int state)
		{
			mCardState = state;
			if (null != mOverlayView) {
				if (null != mOngoingAnimator) {
					mOngoingAnimator.End ();
					mOngoingAnimator = null;
				}
				switch (state) {
				case CARD_STATE_NORMAL:
					mOverlayView.Visibility = ViewStates.Gone;
					mOverlayView.Alpha = 1f;
					break;
				case CARD_STATE_FOCUSED:
					mOverlayView.Visibility = ViewStates.Visible;
					mOverlayView.SetBackgroundResource (Resource.Drawable.card_overlay_focused);
					ObjectAnimator animator = ObjectAnimator.OfFloat (mOverlayView, "alpha", 0f);
					animator.RepeatMode = ValueAnimatorRepeatMode.Reverse;
					animator.RepeatCount = int.MaxValue;
					animator.SetDuration (1000);
					animator.Start ();
					mOngoingAnimator = animator;
					break;
				case CARD_STATE_INACTIVE:
					mOverlayView.Visibility = ViewStates.Visible;
					mOverlayView.Alpha = 1f;
					mOverlayView.SetBackgroundColor (Color.Argb (0xAA, 0xCC, 0xCC, 0xCC));
					break;
				}
			}
			return this;
		}

		/// <summary>
		/// Sets the type of progress inticator.
		/// The progress type can only be changed if the Card was initially built with a progress indicator.
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="progressType">A value of eIther PROGRESS_TYPE_NORMAL, PROGRESS_TYPE_LABEL or PROGRESS_TYPE_NO_PROGRESS</param>
		public Card SetProgressType(int progressType)
		{
			if (mCardProgress == null) {
				mCardProgress = new CardProgress ();
			}
			mCardProgress.ProgressType = progressType;
			return this;
		}

		/// <summary>
		/// Gets or sets the progress indicator type
		/// </summary>
		/// <value>The type of progress indicator</value>
		public int ProgressType
		{
			get {
				if (mCardProgress == null) {
					return PROGRESS_TYPE_NO_PROGRESS;
				}
				return mCardProgress.ProgressType;
			}
			set { SetProgressType (value); }
		}

		/// <summary>
		/// Set progress to the specified value. Only applicable if the card has the progress type PROGRESS_TYPE_NORMAL.
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="progress"></param>
		public Card SetProgress(int progress)
		{
			if (mCardProgress != null) {
				mCardProgress.Progress = progress;
			}
			return this;
		}

		/// <summary>
		/// Set the range of the progress to 0 - max. Only applicable if the card has the progress type PROGRESS_TYPE_NORMAL.
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="max"></param>
		public Card SetMaxProgress(int max)
		{
			if (mCardProgress != null) {
				mCardProgress.Max = max;
			}
			return this;
		}

		/// <summary>
		/// Set the label text for the progress if the card has a progress type of PROGRESS_TYPE_NORMAL, PROGRESS_TYPE_INDETERMINATE or PROGRESS_TYPE_LABEL
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="text"></param>
		public Card SetProgressLabel(String text)
		{
			if (mCardProgress != null) {
				mCardProgress.Label = text;
			}
			return this;
		}

		/// <summary>
		/// Toggle the visibility of the progress section of the card. Only applicable if the card has a progress type of
		/// PROGIRESS_TYPE_NORMAL, PROGRESS_TYPE_INDETERMINTE or PROGRESS_TYPE_LABEL
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="isVisible">If set to <c>true</c> is visible.</param>
		public Card SetProgressVisibiliy(Boolean isVisible)
		{
			if (mCardProgress.ProgressView == null) {
				return this; // Card does not have progress
			}
			mCardProgress.ProgressView.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;

			return this;
		}

		/// <summary>
		/// Adds an action to this card during build time.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		private void AddAction(string label, int id, int type)
		{
			CardAction cardAction = new CardAction ();
			cardAction.Label = label;
			cardAction.Id = id;
			cardAction.Type = type;
			mCardActions.Add (cardAction);
		}

		/// <summary>
		/// Toggles the visibility of a card action.
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="actionId"></param>
		/// <param name="isVisible"></param>
		public Card SetActionVisibility(int actionId, Boolean isVisible)
		{
			ViewStates visibilityFlag = isVisible ? ViewStates.Visible : ViewStates.Gone;
			return this;
		}

		/// <summary>
		/// Toggles the visibility of the action area of this Card through an animation
		/// </summary>
		/// <returns>The Card itself, allows for chaining of calls</returns>
		/// <param name="isVisible"></param>
		public Card SetActionAreaVisibility (Boolean isVisible)
		{
			if (mActionAreaView == null) {
				return this; // Card does not have an action area
			}

			if (isVisible) {
				// Show the action area
				mActionAreaView.Visibility = ViewStates.Visible;
				mActionAreaView.PivotY = 0;
				mActionAreaView.PivotX = mCardView.Width / 2f;
				mActionAreaView.Alpha = 0.5f;
				mActionAreaView.RotationX = -90f;
				mActionAreaView.Animate ().RotationX (0).Alpha (1).SetDuration (400);
			} else {
				// Hid e the action area
				mActionAreaView.PivotY = 0f;
				mActionAreaView.PivotX = mCardView.Width / 2f;
				mActionAreaView.Animate ().RotationX (-90f).Alpha (0f).SetDuration (400).SetListener (new AnimatorListener () {
					OnAnimationEndAction = (animation) =>
					{
						mActionAreaView.Visibility = ViewStates.Gone;
					}
				});
			}
			return this;
		}

		/// <summary>
		/// Creates a shallow clone of the card. Shallow means that all values are present, but now views.
		/// This is useful for saving/restoring in the case of configuration changes, like screen rotation
		/// </summary>
		/// <returns>A shallow clone of the card instance</returns>
		public Card CreateShallowClone()
		{
			Card cloneCard = new Card ();

			// Outer card values
			cloneCard.mTitle = mTitle;
			cloneCard.mDescription = mDescription;
			cloneCard.mTag = mTag;
			cloneCard.mLayoutId = mLayoutId;
			cloneCard.mCardState = mCardState;

			// Progress
			if (mCardProgress != null) {
				cloneCard.mCardProgress = mCardProgress.CreateShallowClone ();
			}

			// Actions
			foreach (CardAction action in mCardActions) {
				cloneCard.mCardActions.Add (action.CreateShallowClone ());
			}

			return cloneCard;
		}

		/// <summary>
		/// Prepare the card to be stored for configuration change
		/// </summary>
		public void PrepareForConfugurationChange()
		{
			// Null out views
			mCardView = null;
			foreach (CardAction action in mCardActions) {
				action.ActionView = null;
			}
			mCardProgress.ProgressView = null;
		}

		public class Builder 
		{
			private Card mCard;

			/// <summary>
			/// Instantiate the builder with data from a shallow clone.
			/// </summary>
			/// <param name="listener"></param>
			/// <param name="card"></param>
			public Builder(OnCardClickListener listener, Card card)
			{
				mCard = card;
				mCard.mClickListener = listener;
			}

			public Builder(OnCardClickListener listener, String tag)
			{
				mCard = new Card();
				mCard.mTag = tag;
				mCard.mClickListener = listener;
			}

			public Builder SetTitle(String title)
			{
				mCard.mTitle = title;
				return this;
			}

			public Builder SetDescription(string desc)
			{
				mCard.mDescription = desc;
				return this;
			}

			/// <summary>
			/// Add an action.
			/// The type describes how this action will be displayed. Accepted values are ACTION_NEUTRAL, ACTION_POSITIVE or ACTION_NEGATIVE
			/// </summary>
			/// <returns>This builder</returns>
			/// <param name="label">The text to display for tis action</param></param></param>
			/// <param name="id">Identifier for this action, supploed in the click listener</param>
			/// <param name="type">UI style of action</param>
			public Builder AddAction(string label, int id, int type)
			{
				mCard.AddAction (label, id, type);
				return this;
			}

			/// <summary>
			/// Override the default layout.
			/// The referenced layout file has to contain the same identifiers as defined in the defaut layout
			/// </summary>
			/// <returns>This builder</returns>
			/// <param name="layout"></param>
			public Builder SetLayout(int layout)
			{
				mCard.mLayoutId = layout;
				return this;
			}

			/// <summary>
			/// Set the type of progress bar to display.
			/// Accepted values are PROGRESS_TYPE_NO_PROGRESS, PROGRESS_TYPE_NORMAL, PROGRESS_TYPE_INDETERMINATE, PROGRESS_TYPE_LABEL
			/// </summary>
			/// <returns>The progress type.</returns>
			/// <param name="progressType">Progress type.</param>
			public Builder SetProgressType(int progressType)
			{
				mCard.SetProgressType (progressType);
				return this;
			}

			public Builder SetProgressLabel(string label)
			{
				// Ensure the progress layout has been initialized, use 'no progress[ by default
				if (mCard.mCardProgress == null) {
					mCard.SetProgressType (PROGRESS_TYPE_NO_PROGRESS);
				}
				mCard.mCardProgress.Label = label;
				return this;
			}

			public Builder SetProgressMaxValue(int maxValue)
			{
				// Ensure the progress layout has been initialized, use 'no progress[ by default
				if (mCard.mCardProgress == null) {
					mCard.SetProgressType (PROGRESS_TYPE_NO_PROGRESS);
				}
				mCard.mCardProgress.Max = maxValue;
				return this;
			}

			public Builder SetStatus(int status)
			{
				mCard.SetState (status);
				return this;
			}

			public Card Build(Activity activity)
			{
				LayoutInflater inflater = activity.LayoutInflater;
				// Inflating the card.
				ViewGroup cardView = (ViewGroup)inflater.Inflate (mCard.mLayoutId,
					                     (ViewGroup)activity.FindViewById (Resource.Id.card_stream), false);

				// Check that the layout contains a TextView with the card_title id
				View viewTitle = cardView.FindViewById (Resource.Id.card_title);
				if (mCard.mTitle != null && viewTitle != null) {
					mCard.mTitleView = (TextView)viewTitle;
					mCard.mTitleView = (TextView)viewTitle;
					mCard.mTitleView.Text = mCard.mTitle;
				} else if (viewTitle != null) {
					viewTitle.Visibility = ViewStates.Gone;
				}

				// Check that the layout contains a TextView with the card_content id
				View viewDesc = cardView.FindViewById (Resource.Id.card_content);
				if (mCard.mDescription != null && viewDesc != null) {
					mCard.mDescView = (TextView)viewDesc;
					mCard.mDescView.Text = mCard.mDescription;
				} else if (viewDesc != null) {
					viewDesc.Visibility = ViewStates.Gone;
				}

				ViewGroup actionArea = (ViewGroup)cardView.FindViewById (Resource.Id.card_actionarea);

				// Inflate progress
				InitializeProgressView (inflater, actionArea);

				// Inflate all action views.
				InitializeActionViews (inflater, cardView, actionArea);

				mCard.mCardView = cardView;
				mCard.mOverlayView = cardView.FindViewById (Resource.Id.card_overlay);

				return mCard;
			}

			/// <summary>
			/// Initialize data from the given card
			/// </summary>
			/// <returns></returns>
			/// <param name="card"></param>
			public Builder CloneFromCard(Card card)
			{
				mCard = card.CreateShallowClone ();
				return this;
			}

			private void InitializeActionViews (LayoutInflater inflater, ViewGroup cardView, ViewGroup actionArea)
			{
				if (mCard.mCardActions.Count != 0) {
					// Set an action area to visible only when actions are visible
					actionArea.Visibility = ViewStates.Visible;
					mCard.mActionAreaView = actionArea;
				}

				// Inflate all card actions
				foreach (CardAction action in mCard.mCardActions) {
					int useActionLayout = 0;
					switch (action.Type) {
					case Card.ACTION_POSITIVE:
						useActionLayout = Resource.Layout.card_button_positive;
						break;
					case Card.ACTION_NEGATIVE:
						useActionLayout = Resource.Layout.card_button_negative;
						break;
					default:
						useActionLayout = Resource.Layout.card_button_neutral;
						break;
					}

					action.ActionView = inflater.Inflate (useActionLayout, actionArea, false);
					Button actionButton = (Button)action.ActionView.FindViewById (Resource.Id.card_button);

					actionButton.Text = action.Label;
					actionButton.SetOnClickListener (new OnClickListener () {
						OnClickAction = (v) =>
						{
							mCard.mClickListener.OnCardClick(action.Id, mCard.mTag);
						}
					});
					actionArea.AddView (action.ActionView);
				}
			}

			/// <summary>
			/// Builds the progress view into the given ViewGroup
			/// </summary>
			/// <param name="inflater"></param>
			/// <param name="actionArea"></param>
			private void InitializeProgressView(LayoutInflater inflater, ViewGroup actionArea)
			{
				// Only inflate progress layout if a progress tupe other than NO_PROGRESS was set.
				if (mCard.mCardProgress != null) {
					// Setup progress card
					View progressView = inflater.Inflate (Resource.Layout.card_progress, actionArea, false);
					ProgressBar progressBar = (ProgressBar)progressView.FindViewById (Resource.Id.card_progress);
					((TextView)progressView.FindViewById (Resource.Id.card_progress_text)).SetText (mCard.mCardProgress.Max);
					progressBar.Max = mCard.mCardProgress.Max;
					progressBar.Progress = 0;
					mCard.mCardProgress.ProgressView = progressView;
					mCard.mCardProgress.ProgressType = mCard.ProgressType;
					actionArea.AddView(progressView);
				}
			}
		}

		/// <summary>
		/// Represents a clicable action, accessible from the bottom of the card.
		/// Fields include the label, an ID to specify the action that was performed in the callback,
		/// an action type (positive, nefative, neutral), and the callback.
		/// </summary>
		public class CardAction 
		{
			public string Label;
			public int Id;
			public int Type;
			public View ActionView;
			public CardAction CreateShallowClone()
			{
				CardAction actionClone = new CardAction ();
				actionClone.Label = Label;
				actionClone.Id = Id;
				actionClone.Type = Type;
				return actionClone;
				// Not the view. never the view (don't want to hold view references for OnConfigurationChange)
			}
		}

		/// <summary>
		/// Describes the progress of a Card.
		/// Three types of progress are supported: PROGRESS_TYPE_NORMAL, PROGRESS_TYPE_INTERMEDIATE, PROGRESS_TYPE_LABEL.
		/// </summary>
		public class CardProgress 
		{
			private int progressType = Card.PROGRESS_TYPE_NO_PROGRESS;
			private string label = "";
			private int currProgress = 0;
			private int maxValue = 100;

			public View ProgressView = null;
			private ProgressBar progressBar = null;
			private TextView progressLabel = null;

			public CardProgress CreateShallowClone() {
				CardProgress progressClone = new CardProgress ();
				progressClone.label = label;
				progressClone.currProgress = currProgress;
				progressClone.maxValue = maxValue;
				progressClone.progressType = progressType;
				return progressClone;
			}

			/// <summary>
			/// Sets the progress. Only useful for the type PROGRESS_TYPE_NORMAL.
			/// </summary>
			/// <value></value>
			public int Progress 
			{
				set 
				{
					currProgress = value;
					ProgressBar bar = ProgressBar;
					if (bar != null)
					{
						bar.Progress = value;
						bar.Invalidate();
					}
				}
			}

			/// <summary>
			/// Sets the range of the progress 0 - value.
			/// Only useful for the type PROGRESS_TYPE_NORMAL.
			/// </summary>
			/// <value></value>
			public int Max
			{
				set {
					maxValue = value;
					ProgressBar bar = ProgressBar;
					if (bar != null) {
						bar.Max = value;
					}
				}
				get {
					return maxValue;
				}
			}

			/// <summary>
			/// Sets or gets the label text that appears near the progress indicator.
			/// </summary>
			/// <value></value>
			public string Label
			{
				set {
					label = value;
					TextView labelView = ProgressLabel;
					if (labelView != null) {
						labelView.Text = value;
					}
				}

			}

			public TextView ProgressLabel
			{
				get {
					if (progressLabel != null) {
						return progressLabel;
					} else if (ProgressView != null) {
						progressLabel = (TextView)ProgressView.FindViewById (Resource.Id.card_progress_text);
						return progressLabel;
					} else {
						return null;
					}
				}
			}

			/// <summary>
			/// Gets or sets how progress is displayed.
			/// </summary>
			/// <value>PROGRESS_TYPE_NORMAL, PROGRESS_TYPE_INDETERMINATE, PROGRESS_TYPE_LABEL</value>
			public int ProgressType
			{
				set {
					progressType = value;
					if (ProgressView != null) {
						switch (value) {
						case PROGRESS_TYPE_NO_PROGRESS:
							ProgressView.Visibility = ViewStates.Gone;
							break;
						case PROGRESS_TYPE_NORMAL:
							ProgressView.Visibility = ViewStates.Visible;
							ProgressBar.Indeterminate = false;
							break;
						case PROGRESS_TYPE_INDETERMINATE:
							ProgressView.Visibility = ViewStates.Visible;
							ProgressBar.Indeterminate = true;
							break;
						}
					}
				}
				get {
					return progressType;
				}
			}

			public ProgressBar ProgressBar {
				get {
					if (progressBar != null) {
						return progressBar;
					} else if (ProgressView != null) {
						progressBar = (ProgressBar)ProgressView.FindViewById (Resource.Id.card_progress);
						return progressBar;
					} else {
						return null;
					}
				}
			}
		}
	}
}

