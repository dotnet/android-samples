using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

namespace Example_WorkingWithAudio
{
    [Activity(Label = "Example_WorkingWithAudio", MainLauncher = true, Icon = "@drawable/icon")]
    public class WorkingWithAudioActivity : Activity
    {
        PlayAudio playAudio = new PlayAudio ();
        RecordAudio recordAudio = new RecordAudio ();
        LowLevelPlayAudio llPlayAudio = new LowLevelPlayAudio ();
        LowLevelRecordAudio llRecordAudio = new LowLevelRecordAudio ();
        NotificationManager nMan = new NotificationManager ();

        static public bool useNotifications = false;
        static Activity activity = null;

        static public Activity Activity {
            get { return (activity); }
        }

        // buttons.
        Button startRecording = null;
        Button stopRecording = null;
        Button startPlayback = null;
        Button stopPlayback = null;
        Button startLlRecording = null;
        Button stopLlRecording = null;
        Button startLlPlayback = null;
        Button stopLlPlayback = null;
        bool haveRecording = false;
        bool isRecording = false;
        bool isPlaying = false;
        bool haveLlRecording = false;
        bool isLlRecording = false;
        bool isLlPlaying = false;
        TextView status = null;

        void disableAllButtons ()
        {
            startRecording.Enabled = false;
            stopRecording.Enabled = false;
            startPlayback.Enabled = false;
            stopPlayback.Enabled = false;

            startLlRecording.Enabled = false;
            stopLlRecording.Enabled = false;
            startLlPlayback.Enabled = false;
            stopLlPlayback.Enabled = false;
        }

        // Provides the policy for which buttons should be enabled for any particular state.
        // The rule is that any action that has been started must be ended before the user 
        // is allowed to do anything else.
        void handleButtonState ()
        {
            disableAllButtons ();
            if (isRecording) {
                stopRecording.Enabled = true;
                return;
            }
            if (isLlRecording) {
                stopLlRecording.Enabled = true;
                return;
            }
           
            if (isPlaying) {
                stopPlayback.Enabled = true;
                return;
            } else {
                if (haveRecording)
                    startPlayback.Enabled = true;
                
            }
                
            if (isLlPlaying) {
                stopLlPlayback.Enabled = true;
                return;
            } else {
                if (haveLlRecording)
                    startLlPlayback.Enabled = true;
                
            }
            startRecording.Enabled = true;
            startLlRecording.Enabled = true;
        }

        public void setStatus (String message)
        {
            status.Text = message;
        }

        public void resetPlayback ()
        {
            isPlaying = false;
            isLlPlaying = false;
            handleButtonState ();
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            NotificationManager.audioManager = (AudioManager)GetSystemService (Context.AudioService);

            // Hook up all of our handlers.

            // High-level operations.
            startRecording = FindViewById<Button> (Resource.Id.startRecordingButton);
            startRecording.Click += delegate {
                startOperation (recordAudio);
                disableAllButtons ();
                isRecording = true;
                haveRecording = true;
                handleButtonState ();
            };
            stopRecording = FindViewById<Button> (Resource.Id.endRecordingButton);
            stopRecording.Click += delegate {
                stopOperation (recordAudio);
                isRecording = false;
                handleButtonState ();
            };
            startPlayback = FindViewById<Button> (Resource.Id.startPlaybackButton);
            //button.Click += delegate { _playAudio.start(); };
            startPlayback.Click += delegate {
                startOperation (playAudio);
                disableAllButtons ();
                isPlaying = true;
                handleButtonState ();
            };
            stopPlayback = FindViewById<Button> (Resource.Id.endPlaybackButton);
            stopPlayback.Click += delegate {
                stopOperation (playAudio);
                isPlaying = false;
                handleButtonState ();
            };

            // Low-level operations.
            startLlRecording = FindViewById<Button> (Resource.Id.llStartRecordingButton);
            startLlRecording.Click += delegate {
               
                startOperation (llRecordAudio); 
                disableAllButtons (); 
                isLlRecording = true; 
                haveLlRecording = true; 
                handleButtonState ();
            };
  
            stopLlRecording = FindViewById<Button> (Resource.Id.llEndRecordingButton);
            stopLlRecording.Click += delegate {
                stopOperation (llRecordAudio);
                isLlRecording = false;
                handleButtonState ();
            };
 
            startLlPlayback = FindViewById<Button> (Resource.Id.llStartPlaybackButton);

            startLlPlayback.Click += delegate {
                // Wait for the recording thread to exit so as to avoid file contention.
                while (llRecordAudio.IsRecording) {
                    Thread.Sleep (100);
                }
                startOperation (llPlayAudio); 
                disableAllButtons (); 
                isLlPlaying = true;  
                handleButtonState (); 
            };

            stopLlPlayback = FindViewById<Button> (Resource.Id.llEndPlaybackButton);

            stopLlPlayback.Click += delegate {
                stopOperation (llPlayAudio);
                isLlPlaying = false;
                handleButtonState ();
            };

            handleButtonState ();

            // Notifications.
            CheckBox notifCheckBox = FindViewById<CheckBox> (Resource.Id.useNotificationsCheckBox);
            notifCheckBox.Click += delegate {
                useNotifications = notifCheckBox.Checked;
            };

            status = FindViewById<TextView> (Resource.Id.status);

            NotificationManager.MainActivity = this;
            activity = this;
        }

        void startOperation (INotificationReceiver nRec)
        {
            if (useNotifications) {
                bool haveFocus = nMan.RequestAudioResources (nRec);
                if (haveFocus) {
                    status.Text = "Granted";
                    nRec.Start ();
                } else {
                    status.Text = "Denied";
                }
            } else {
                nRec.Start ();
            }
        }

        void stopOperation (INotificationReceiver nRec)
        {
            nRec.Stop ();
            if (useNotifications) {
                nMan.ReleaseAudioResources ();
                status.Text = "Released";
            }

        }

    }
}

