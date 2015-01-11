using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Util;
using Android.Gms.Wearable;

namespace FindMyPhoneSample
{
	[Service]
	[IntentFilter (new string[]{ "com.google.android.gms.wearable.BIND_LISTENER" })]
	public class SoundAlarmListenerService : WearableListenerService
	{
		const string TAG = "ExampleFindPhoneApp";
		const string FIELD_ALARM_ON = "alarm_on";

		private AudioManager audio_manager;
		static int orig_volume;
		int max_volume;
		Android.Net.Uri alarm_sound;
		MediaPlayer media_player;

		public override void OnCreate ()
		{
			base.OnCreate ();

			audio_manager = (AudioManager)GetSystemService (AudioService);

			//set the original volume and max volume variables to the phone's current and max volume levels
			orig_volume = audio_manager.GetStreamVolume (Android.Media.Stream.Ring);
			max_volume = audio_manager.GetStreamMaxVolume (Android.Media.Stream.Ring);

			//set the sound for the alarm to the phone's alarm tone
			alarm_sound = RingtoneManager.GetDefaultUri (RingtoneType.Alarm);

			media_player = new MediaPlayer ();
		}

		public override void OnDestroy ()
		{
			//turn off the alarm
			audio_manager.SetStreamVolume ((Android.Media.Stream.Ring), orig_volume, 0);
			media_player.Release ();
			base.OnDestroy ();
		}

		public override void OnDataChanged (DataEventBuffer buffer)
		{
			if (Log.IsLoggable (TAG, LogPriority.Debug))
				Log.Debug (TAG, "OnDataChanged: " + buffer + " for " + PackageName);
			for (int i = 0; i < buffer.Count; i++) {
				IDataEvent e = Android.Runtime.Extensions.JavaCast<IDataEvent> (buffer.Get (i));
				if (e.Type  == DataEvent.TypeDeleted) {
					Log.Info (TAG, e + " deleted");
				} else if (e.Type  == DataEvent.TypeChanged) {
					bool alarmOn = (bool)DataMap.FromByteArray (e.DataItem.GetData ()).Get (FIELD_ALARM_ON);
					if (!alarmOn) {
						orig_volume = audio_manager.GetStreamVolume (Android.Media.Stream.Alarm);
						media_player.Reset ();
						audio_manager.SetStreamVolume (Android.Media.Stream.Alarm, max_volume, 0);
						media_player.SetAudioStreamType (Android.Media.Stream.Alarm);
						try {
							media_player.SetDataSource (ApplicationContext, alarm_sound);
							media_player.Prepare ();
						} catch (IOException ex) {
							Log.Error (TAG, "Failed to prepare media player to play alarm.", ex);
						}
						media_player.Start ();
					} else {
						audio_manager.SetStreamVolume (Android.Media.Stream.Alarm, orig_volume, 0);
						if (media_player.IsPlaying)
							media_player.Stop ();
					}
				}
			}
			buffer.Close ();
		}
	}
}
