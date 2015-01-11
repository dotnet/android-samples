using System;
using System.Collections.Generic;

namespace BatchStepSensor.CardStream
{
	/// <summary>
	/// A class that holds the state of a CardStreamFragment
	/// </summary>
	public class CardStreamState
	{
		protected Card[] visibleCards, hiddenCards;
		public Card[] VisibleCards
		{
			get 
			{
				return visibleCards;
			}
		}


		public Card[] HiddenCards
		{
			get 
			{
				return hiddenCards;
			}
		}

		protected HashSet<string> dismissibleCards;
		public HashSet<string> DismissibleCards
		{
			get {
				return dismissibleCards;
			}
		}
		protected string shownTag;

		public CardStreamState(Card[] visible, Card[] hidden, HashSet<String> dismissible, String shownTag)
		{
			visibleCards = visible;
			hiddenCards = hidden;
			dismissibleCards = dismissible;
			this.shownTag = shownTag;
		}
	}
}

