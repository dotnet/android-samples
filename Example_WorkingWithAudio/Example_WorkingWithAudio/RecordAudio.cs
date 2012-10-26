using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

namespace Example_WorkingWithAudio
{
    //
    // Shows how to use the MediaRecorder to record audio.
    //
    class RecordAudio : INotificationReceiver
    {
        //static string filePath = "/sdcard/Music/testAudio.mp3";
        static string filePath = "/data/data/Example_WorkingWithAudio.Example_WorkingWithAudio/files/testAudio.mp4";
        MediaRecorder recorder = null;

        public void StartRecorder ()
        {
            try {
                //Java.IO.File sdDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic);
                //filePath = sdDir + "/" + "testAudio.mp3";
                if (File.Exists (filePath))
                    File.Delete (filePath);

                //Java.IO.File myFile = new Java.IO.File(filePath);
                //myFile.CreateNewFile();

                if (recorder == null)
                    recorder = new MediaRecorder (); // Initial state.
                else
                    recorder.Reset ();

                recorder.SetAudioSource (AudioSource.Mic);
                recorder.SetOutputFormat (OutputFormat.Mpeg4);
                recorder.SetAudioEncoder (AudioEncoder.AmrNb); // Initialized state.
                recorder.SetOutputFile (filePath); // DataSourceConfigured state.
                recorder.Prepare (); // Prepared state
                recorder.Start (); // Recording state.
 
            } catch (Exception ex) {
                Console.Out.WriteLine (ex.StackTrace);
            }
        }

        public void StopRecorder ()
        {
            if (recorder != null) {
                recorder.Stop ();
                recorder.Release ();
                recorder = null;
            }
        }

        public void Start ()
        {
            StartRecorder ();
        }

        public void Stop ()
        {
            StopRecorder ();
        }

    }
}