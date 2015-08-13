using System;
using System.IO;
using Android.Content;
using Android.Media;
using Android.Media.Session;
using Android.Net;
using Android.Net.Wifi;

namespace MediaBrowserService
{
	public class Playback : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener, 
	MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, 
	MediaPlayer.IOnPreparedListener, MediaPlayer.IOnSeekCompleteListener
	{
		static readonly string Tag = LogHelper.MakeLogTag(typeof(Playback));

		public const float VolumeDuck = 0.2f;
		public const float VolumeNormal = 1.0f;

		const int AudioNoFocusNoDuck = 0;
		const int AudioNoFocusCanDuck = 1;
		const int AudioFocused  = 2;

		readonly MusicService service;
		readonly WifiManager.WifiLock wifiLock;
		public PlaybackStateCode State { get; set; }
		bool playOnFocusGain;
		public ICallback Callback { get; set; }
		readonly MusicProvider musicProvider;
		volatile bool audioNoisyReceiverRegistered;
		volatile int currentPosition;
		volatile string currentMediaId;

		int audioFocus = AudioNoFocusNoDuck;
		AudioManager audioManager;
		MediaPlayer mediaPlayer;

		IntentFilter mAudioNoisyIntentFilter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);

		readonly BroadcastReceiver mAudioNoisyReceiver = new BroadcastReceiver ();

		class BroadcastReceiver : Android.Content.BroadcastReceiver
		{
			public Action<Context, Intent> OnReceiveImpl { get; set; }
			public override void OnReceive (Context context, Intent intent)
			{
				OnReceiveImpl (context, intent);
			}
		}

		public Playback (MusicService service, MusicProvider musicProvider)
		{
			this.service = service;
			this.musicProvider = musicProvider;
			audioManager = (AudioManager) service.GetSystemService (Context.AudioService);
			wifiLock = ((WifiManager) service.GetSystemService (Context.WifiService))
				.CreateWifiLock(WifiMode.Full, "sample_lock");
			mAudioNoisyReceiver.OnReceiveImpl = (context, intent) => {
				if (AudioManager.ActionAudioBecomingNoisy == intent.Action) {
					LogHelper.Debug (Tag, "Headphones disconnected.");
					if (IsPlaying) {
						var i = new Intent (context, typeof(MusicService));
						i.SetAction (MusicService.ActionCmd);
						i.PutExtra (MusicService.CmdName, MusicService.CmdPause);
						service.StartService (i);
					}
				}
			};
		}

		public void Start ()
		{
			
		}

		public void Stop (bool notifyListeners)
		{
			State = PlaybackStateCode.Stopped;

			if (notifyListeners && Callback != null) {
				Callback.OnPlaybackStatusChanged (State);
			}

			currentPosition = CurrentStreamPosition;
			GiveUpAudioFocus ();
			UnregisterAudioNoisyReceiver ();
			RelaxResources (true);
			if (wifiLock.IsHeld) {
				wifiLock.Release ();
			}
		}
			
		public bool IsConnected = true;

		public bool IsPlaying {
			get {
				return playOnFocusGain || (mediaPlayer != null && mediaPlayer.IsPlaying);
			}
		}

		public int CurrentStreamPosition {
			get {
				return mediaPlayer != null ? mediaPlayer.CurrentPosition : currentPosition;
			}
		}

		public void Play (MediaSession.QueueItem item) 
		{
			playOnFocusGain = true;
			TryToGetAudioFocus ();
			RegisterAudioNoisyReceiver ();
			string mediaId = item.Description.MediaId;
			bool mediaHasChanged = mediaId != currentMediaId;
			if (mediaHasChanged) {
				currentPosition = 0;
				currentMediaId = mediaId;
			}

			if (State == PlaybackStateCode.Paused && !mediaHasChanged && mediaPlayer != null) {
				ConfigMediaPlayerState ();
			} else {
				State = PlaybackStateCode.Stopped;
				RelaxResources (false);
				MediaMetadata track = musicProvider.GetMusic (
					MediaIDHelper.ExtractMusicIDFromMediaID (item.Description.MediaId));

				string source = track.GetString (MusicProvider.CustomMetadataTrackSource);

				try {
					CreateMediaPlayerIfNeeded ();

					State = PlaybackStateCode.Buffering;

					mediaPlayer.SetAudioStreamType (Android.Media.Stream.Music);
					mediaPlayer.SetDataSource (source);

					mediaPlayer.PrepareAsync ();

					wifiLock.Acquire ();

					if (Callback != null) {
						Callback.OnPlaybackStatusChanged (State);
					}

				} catch (IOException ex) {
					LogHelper.Error (Tag, ex, "Exception playing song");
					if (Callback != null) {
						Callback.OnError (ex.Message);
					}
				}
			}
		}

		public void Pause ()
		{
			if (State == PlaybackStateCode.Playing) {
				if (mediaPlayer != null && mediaPlayer.IsPlaying) {
					mediaPlayer.Pause ();
					currentPosition = mediaPlayer.CurrentPosition;
				}
				RelaxResources (false);
				GiveUpAudioFocus ();
			}
			State = PlaybackStateCode.Paused;
			if (Callback != null) {
				Callback.OnPlaybackStatusChanged (State);
			}
			UnregisterAudioNoisyReceiver ();
		}

		public void SeekTo (int position)
		{
			LogHelper.Debug (Tag, "seekTo called with ", position);

			if (mediaPlayer == null) {
				currentPosition = position;
			} else {
				if (mediaPlayer.IsPlaying) {
					State = PlaybackStateCode.Buffering;
				}
				mediaPlayer.SeekTo (position);
				if (Callback != null) {
					Callback.OnPlaybackStatusChanged (State);
				}
			}
		}
			
		void TryToGetAudioFocus ()
		{
			LogHelper.Debug (Tag, "tryToGetAudioFocus");
			if (audioFocus != AudioFocused) {
				var result = audioManager.RequestAudioFocus (this, Android.Media.Stream.Music,
					AudioFocus.Gain);
				if (result == AudioFocusRequest.Granted) {
					audioFocus = AudioFocused;
				}
			}
		}

		void GiveUpAudioFocus ()
		{
			LogHelper.Debug (Tag, "giveUpAudioFocus");
			if (audioFocus == AudioFocused) {
				if (audioManager.AbandonAudioFocus (this) == AudioFocusRequest.Granted) {
					audioFocus = AudioNoFocusNoDuck;
				}
			}
		}

		void ConfigMediaPlayerState ()
		{
			LogHelper.Debug (Tag, "configMediaPlayerState. mAudioFocus=", audioFocus);
			if (audioFocus == AudioNoFocusNoDuck) {
				if (State == PlaybackStateCode.Playing) {
					Pause ();
				}
			} else {  // we have audio focus:
				if (audioFocus == AudioNoFocusCanDuck) {
					mediaPlayer.SetVolume (VolumeDuck, VolumeDuck);
				} else {
					if (mediaPlayer != null) {
						mediaPlayer.SetVolume (VolumeNormal, VolumeNormal);
					}
				}
				if (playOnFocusGain) {
					if (mediaPlayer != null && !mediaPlayer.IsPlaying) {
						LogHelper.Debug (Tag,"configMediaPlayerState startMediaPlayer. seeking to ",
							currentPosition);
						if (currentPosition == mediaPlayer.CurrentPosition) {
							mediaPlayer.Start ();
							State = PlaybackStateCode.Playing;
						} else {
							mediaPlayer.SeekTo(currentPosition);
							State = PlaybackStateCode.Buffering;
						}
					}
					playOnFocusGain = false;
				}
			}
			if (Callback != null) {
				Callback.OnPlaybackStatusChanged (State);
			}
		}

		public void OnAudioFocusChange (AudioFocus focusChange)
		{
			LogHelper.Debug(Tag, "onAudioFocusChange. focusChange=", focusChange);
			if (focusChange == AudioFocus.Gain) {
				audioFocus = AudioFocused;

			} else if (focusChange == AudioFocus.Loss ||
				focusChange == AudioFocus.LossTransient ||
				focusChange == AudioFocus.LossTransientCanDuck) {
				bool canDuck = focusChange == AudioFocus.LossTransientCanDuck;
				audioFocus = canDuck ? AudioNoFocusCanDuck : AudioNoFocusNoDuck;

				playOnFocusGain |= State == PlaybackStateCode.Playing && !canDuck;
			} else {
				LogHelper.Error (Tag, "onAudioFocusChange: Ignoring unsupported focusChange: ", focusChange);
			}
			ConfigMediaPlayerState ();
		}

		public void OnCompletion (MediaPlayer mp)
		{
			LogHelper.Debug(Tag, "onCompletion from MediaPlayer");
			if (Callback != null) {
				Callback.OnCompletion ();
			}
		}

		public bool OnError (MediaPlayer mp, MediaError what, int extra)
		{
			LogHelper.Error (Tag, "Media player error: what=" + what + ", extra=" + extra);
			if (Callback != null) {
				Callback.OnError ("MediaPlayer error " + what + " (" + extra + ")");
			}
			return true;
		}

		public void OnPrepared (MediaPlayer mp)
		{
			LogHelper.Debug (Tag, "onPrepared from MediaPlayer");
			ConfigMediaPlayerState ();
		}

		public void OnSeekComplete (MediaPlayer mp)
		{
			LogHelper.Debug (Tag, "onSeekComplete from MediaPlayer:", mp.CurrentPosition);
			currentPosition = mp.CurrentPosition;
			if (State == PlaybackStateCode.Buffering) {
				mediaPlayer.Start ();
				State = PlaybackStateCode.Playing;
			}
			if (Callback != null) {
				Callback.OnPlaybackStatusChanged(State);
			}
		}

		void CreateMediaPlayerIfNeeded ()
		{
			LogHelper.Debug (Tag, "createMediaPlayerIfNeeded. needed? ", (mediaPlayer==null));
			if (mediaPlayer == null) {
				mediaPlayer = new MediaPlayer ();

				mediaPlayer.SetWakeMode (service.ApplicationContext,
					Android.OS.WakeLockFlags.Partial);

				mediaPlayer.SetOnPreparedListener (this);
				mediaPlayer.SetOnCompletionListener (this);
				mediaPlayer.SetOnErrorListener (this);
				mediaPlayer.SetOnSeekCompleteListener (this);
			} else {
				mediaPlayer.Reset ();
			}
		}

		void RelaxResources (bool releaseMediaPlayer)
		{
			LogHelper.Debug (Tag, "relaxResources. releaseMediaPlayer=", releaseMediaPlayer);

			service.StopForeground (true);

			if (releaseMediaPlayer && mediaPlayer != null) {
				mediaPlayer.Reset ();
				mediaPlayer.Release ();
				mediaPlayer = null;
			}

			if (wifiLock.IsHeld) {
				wifiLock.Release ();
			}
		}

		void RegisterAudioNoisyReceiver ()
		{
			if (!audioNoisyReceiverRegistered) {
				service.RegisterReceiver (mAudioNoisyReceiver, mAudioNoisyIntentFilter);
				audioNoisyReceiverRegistered = true;
			}
		}

		void UnregisterAudioNoisyReceiver ()
		{
			if (audioNoisyReceiverRegistered) {
				service.UnregisterReceiver (mAudioNoisyReceiver);
				audioNoisyReceiverRegistered = false;
			}
		}

		public interface ICallback
		{
			void OnCompletion ();
			void OnPlaybackStatusChanged (PlaybackStateCode state);
			void OnError (string error);
		}
	}
}

