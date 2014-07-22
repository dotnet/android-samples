using System;

namespace BatchStepSensor.CardStream
{
	public interface OnCardClickListener
	{
		void OnCardClick(int cardActionId, String cardTag);
	}
}

