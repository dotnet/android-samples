Working With Audio – Test Application
=====================================

Sample code to accompany the article Working With Audio.This simple application provides the following functionality:*  Recording audio using the high-level API (MediaRecorder.)*  Playing audio using the high-level API (MediaPlayer.)*  Recording audio using the low-level API (AudioRecord.)*  Playing audio using the low-level API (AudioTrack.)*  Optionally use Audio notifications.
One can only play back what has been recorded in the same session. (For this reason, the playback buttons are disabled on start-up.)
Once on operation has been started, it must be stopped. No other actions are allowed in the meantime.
The best way to demonstrate recording and playback is to record some sound and then play it back again.
The best way to see audio notifications in action is to call the test phone, keep the call open for a while and then end the call while there is an ongoing operation. Audio-focus will be taken away and then granted again when the call is complete.