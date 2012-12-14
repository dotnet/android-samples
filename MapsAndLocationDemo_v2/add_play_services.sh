#/bin/sh
cp -r $ANDROID_HOME/extras/google/google_play_services/libproject/ .
cd google-play-services_lib
android update project -p .
ant debug
cd ..
