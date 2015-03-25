using System;
using Android.OS;
namespace Timer
{
	public class TimerObj
	{
		// Start time in milliseconds.
		public long startTime;

		// Length of the timer in milliseconds.
		public long originalLength;

		/**
	     * Construct a timer with a specific start time and length.
	     *
	     * @param startTime the start time of the timer.
	     * @param timerLength the length of the timer.
	     */
		public TimerObj(long startTime, long timerLength) {
			this.startTime = startTime;
			this.originalLength = timerLength;
		}

		/**
	     * Calculate the time left of this timer.
	     * @return the time left for this timer.
	     */
		public long TimeLeft() {
			long millis = SystemClock.ElapsedRealtime();
			return originalLength - (millis - startTime);
		}
	}
}

