using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

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
    // This class shows how to use the low level class AudioTrack in order to play Audio.
    //
    class LowLevelPlayAudio : INotificationReceiver
    {
        static string filePath = "/data/data/Example_WorkingWithAudio.Example_WorkingWithAudio/files/testAudio.mp4";
        byte[] buffer = null;
        AudioTrack audioTrack = null;

        public void Playback ()
        {
            FileStream fileStream = new FileStream (filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader (fileStream);
            long totalBytes = new System.IO.FileInfo (filePath).Length;
            buffer = binaryReader.ReadBytes ((Int32)totalBytes);
            fileStream.Close ();
            fileStream.Dispose ();
            binaryReader.Close ();
            PlayAudioTrack ();
        }

        protected void PlayAudioTrack ()
        {
            audioTrack = new AudioTrack (
                // Stream type
                Android.Media.Stream.Music,
                // Frequency
                11025,
                // Mono or stereo
                ChannelConfiguration.Mono,
                // Audio encoding
                Android.Media.Encoding.Pcm16bit,
                // Length of the audio clip.
                buffer.Length,
                // Mode. Stream or static.
                AudioTrackMode.Stream);

            audioTrack.Play ();

            audioTrack.Write (buffer, 0, buffer.Length);
        }

        public void Start ()
        {
            Playback ();
        }

        public void Stop ()
        {
            if (audioTrack != null) {
                audioTrack.Stop ();
                audioTrack.Release ();
                audioTrack = null;
            }
        }

    }
}