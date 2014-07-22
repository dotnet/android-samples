
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

namespace BatchStepSensor.CardStream
{
	/// <summary>
	/// A Fragment that handles a stream of cards.
	/// Cards can be shown or hidden. When a card is shown it can also be marked as not-dismissible.
	/// </summary>
	public class CardStreamFragment : Android.Support.V4.App.Fragment
	{
		private const int INITIAL_SIZE = 15;
		private CardStreamLinearLayout mLayout = null;
		private Dictionary<String, Card> mVisibleCards = new Dictionary<String, Card>(INITIAL_SIZE);
		private Dictionary<String, Card> mHiddenCards = new Dictionary<String, Card>(INITIAL_SIZE);
		private HashSet<String> mDismissibleCards = new HashSet<String> ();

		private OnDismissListener mCardDismissListener;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate (Resource.Layout.cardstream, container, false);
			mLayout = (CardStreamLinearLayout)view.FindViewById (Resource.Id.card_stream);
			mLayout.SetOnDismissListener (mCardDismissListener);

			return view;
		}

		/// <summary>
		/// Add a visible, dismissible card to the card stream.
		/// </summary>
		/// <param name="card">The card to add</param>
		public void AddCard(Card card)
		{
			String tag = card.Tag;

			if (!mVisibleCards.ContainsKey (tag) && !mHiddenCards.ContainsKey (tag)) {
				View view = card.View;
				view.Tag = tag;
				mHiddenCards.Add (tag, card);
			}
		}

		/// <summary>
		/// Add and show a card
		/// </summary>
		/// <param name="card">The card to add</param>
		/// <param name="show">If set to <c>true</c> show.</param>
		public void AddCard(Card card, bool show)
		{
			AddCard (card);
			if (show) {
				ShowCard (card.Tag);
			}
		}

		/// <summary>
		/// Remove the card with the specified tag
		/// </summary>
		/// <returns><c>true</c>, if the card was removed, <c>false</c> otherwise.</returns>
		/// <param name="tag">Tag of the card to be removed</param>
		public bool RemoveCard(string tag)
		{
			if (mVisibleCards.ContainsKey (tag)) {
				// Attempt to remove a visible card first
				Card card = mVisibleCards [tag];
				mVisibleCards.Remove (tag);
				mLayout.RemoveView (card.View);
				return true;
			} else {
				// Card is hidden, no need to remove it from the layout
				if (mHiddenCards.ContainsKey (tag)) {
					mHiddenCards.Remove (tag);
					return true;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// Show a dismissible card
		/// </summary>
		/// <returns><c>false</c> if the card could not be shown</returns>
		/// <param name="tag"></param>
		public bool ShowCard(string tag)
		{
			return ShowCard (tag, true);
		}

		/// <summary>
		/// Show a card
		/// </summary>
		/// <returns><c>false</c> if the card could not be shown</returns>
		/// <param name="tag"></param>
		/// <param name="dismissible"></param>
		public bool ShowCard(string tag, bool dismissible)
		{
			if (mLayout != null && mHiddenCards.ContainsKey (tag) && !mVisibleCards.ContainsKey(tag)) {
				Card card = mHiddenCards [tag];
				mHiddenCards.Remove (tag);
				mVisibleCards.Add (tag, card);
				mLayout.AddCard (card.View, dismissible);
				if (dismissible) {
					mDismissibleCards.Add (tag);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Hides the card.
		/// </summary>
		/// <returns><c>false</c> if the card could not be hidden</returns>
		/// <param name="tag">Tag.</param>
		public bool HideCard(string tag)
		{
			if (mVisibleCards.ContainsKey (tag)) {
				Card card = mVisibleCards [tag];
				mVisibleCards.Remove (tag);
				mHiddenCards.Add (tag, card);

				mLayout.RemoveView (card.View);
				return true;
			}
			return mHiddenCards.ContainsKey (tag);
		}

		private void DismissCard(string tag)
		{
			if (mVisibleCards.ContainsKey (tag)) {
				Card card = mVisibleCards [tag];
				mVisibleCards.Remove (tag);
				mHiddenCards.Add (tag, card);
			}
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mCardDismissListener = new OnDismissListener()
			{
				OnDismissAction = (String tag) =>
				{
					DismissCard(tag);
				}
			};
		}

		public bool IsCardVisible (string tag)
		{
			return mVisibleCards.ContainsKey (tag);
		}

		/// <summary>
		/// Check if card the card is shown and dismissible
		/// </summary>
		/// <returns><c>true</c> the card is both shown and dismissible</returns>
		/// <param name="tag"></param>

		public bool IsCardDismissible (string tag)
		{
			return mDismissibleCards.Contains (tag);
		}

		/// <summary>
		/// Returns the card with this tag
		/// </summary>
		/// <returns>The card with the specified tag</returns>
		/// <param name="tag"></param>
		public Card GetCard(string tag)
		{
			if (mVisibleCards.ContainsKey (tag)) {
				return mVisibleCards [tag];
			} else if (mHiddenCards.ContainsKey (tag)) {
				return mHiddenCards [tag];
			} else
				return null;
		}

		/// <summary>
		/// Moves the view port to show the card with this tag
		/// </summary>
		/// <param name="tag"></param>
		public void SetFirstVisibleCard(string tag)
		{
			if (mVisibleCards.ContainsKey (tag)) {
				mLayout.FirstVisibleCardTag = tag;
			}
		}

		public int VisibleCardCount
		{
			get {
				return mVisibleCards.Count;
			}
		}

		public ICollection<Card> VisibleCards
		{
			get {
				return mVisibleCards.Values;
			}
		}

		public void RestoreState(CardStreamState state, OnCardClickListener callback)
		{
			// Restore hidden cards
			foreach (Card c in state.HiddenCards) {
				Card card = new Card.Builder (callback, c).Build (Activity);
				if (!mHiddenCards.ContainsKey (card.Tag))
					mHiddenCards.Add (card.Tag, card);
				else
					mHiddenCards [card.Tag] = card;
			}

			// temprarily set up list of dismissible cards
			HashSet<String> dismissibleCards = state.DismissibleCards;

			// Restore shown cards

			foreach (Card c in state.VisibleCards) {
				Card card = new Card.Builder (callback, c).Build (Activity);
				AddCard (card);
				string tag = card.Tag;
				ShowCard (tag, dismissibleCards.Contains (tag));
			}

			mLayout.TriggerShowInitialAnimation ();
		}

		public CardStreamState DumpState()
		{
			Card[] visible = CloneCards (mVisibleCards.Values);
			Card[] hidden = CloneCards (mHiddenCards.Values);
			HashSet<string> dismissible = new HashSet<string> (mDismissibleCards);
			string fistVisible = mLayout.FirstVisibleCardTag;

			return new CardStreamState (visible, hidden, dismissible, fistVisible);
		}
		private Card[] CloneCards(ICollection<Card> cards) {
			Card[] cardArray = new Card[cards.Count];
			int i = 0;
			foreach (Card c in cards) {
				cardArray [i++] = c.CreateShallowClone ();
			}
			return cardArray;
		}
	}
}

