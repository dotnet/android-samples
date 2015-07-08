using System;
using Android.OS;

namespace MediaBrowserService
{
	public static class CarHelper
	{
		const string AutoAppPackageName = "com.google.android.projection.gearhead";

		// Use these extras to reserve space for the corresponding actions, even when they are disabled
		// in the playbackstate, so the custom actions don't reflow.
		const string SlotReservationSkipToNext =
			"com.google.android.gms.car.media.ALWAYS_RESERVE_SPACE_FOR.ACTION_SKIP_TO_NEXT";
		const string SlotReservationSkipToPrev =
			"com.google.android.gms.car.media.ALWAYS_RESERVE_SPACE_FOR.ACTION_SKIP_TO_PREVIOUS";
		const string SlotReservationQueue =
			"com.google.android.gms.car.media.ALWAYS_RESERVE_SPACE_FOR.ACTION_QUEUE";


		public static bool IsValidCarPackage(string packageName) {
			return AutoAppPackageName == packageName;
		}

		public static void SetSlotReservationFlags(Bundle extras, bool reservePlayingQueueSlot,
			bool reserveSkipToNextSlot, bool reserveSkipToPrevSlot) {
			if (reservePlayingQueueSlot) {
				extras.PutBoolean(SlotReservationQueue, true);
			} else {
				extras.Remove(SlotReservationQueue);
			}
			if (reserveSkipToPrevSlot) {
				extras.PutBoolean(SlotReservationSkipToPrev, true);
			} else {
				extras.Remove(SlotReservationSkipToPrev);
			}
			if (reserveSkipToNextSlot) {
				extras.PutBoolean(SlotReservationSkipToNext, true);
			} else {
				extras.Remove(SlotReservationSkipToNext);
			}
		}
	}
}

