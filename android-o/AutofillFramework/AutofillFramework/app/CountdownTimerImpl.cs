using System;
using Android.OS;
using Android.Widget;

namespace AutofillFramework.app
{
	public class CountdownTimerImpl : CountDownTimer
	{
		public TextView CountdownText { get; set; }
		public WelcomeActivity Activity { get; set; }

		public CountdownTimerImpl(WelcomeActivity activity, TextView countdownText,
			long millisInFuture, long countDownInterval) : base(millisInFuture, countDownInterval) 
			{
			Activity = activity;
			CountdownText = countdownText;
		}

		public override void OnFinish()
		{
			if (!Activity.IsFinishing)
			{
				Activity.Finish();
			}
		}

		public override void OnTick(long millisUntilFinished)
		{
			int secondsRemaining = Convert.ToInt32(millisUntilFinished / 1000);
			CountdownText.Text = CountdownText.Context.Resources
				.GetQuantityString(Resource.Plurals.welcome_page_countdown, secondsRemaining, secondsRemaining);
		}
	}
}
