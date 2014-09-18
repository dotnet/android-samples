AgendaData Sample
=================
This sample demonstrates sending calendar events from an Android handheld device to an Android Wear device, as well as deleting those events.

Running This Sample
===================
In order to run this sample, you will need a physical Android handheld device (phone or tablet), and either an Android Wear device or the Android Wear emulator.

Download the Android Wear app from the Play Store to your handheld device, and follow the instructions to connect to your Wear device. If you are using the Wear emulator, make sure your phone/tablet is connected to the computer and run "adb -d forward tcp:5601 tcp:5601" in the console or terminal (adb should be in the platform-tools folder in your Android SDK installation folder).

Once your handheld and your wearable are connected, open the solution, set AgendaData as the start up project and deploy it to your handheld. Then set Wearable as the start up project and deploy it to your wearable. If you are using a physical wearable device, it must be connected to the computer through USB, you cannot deploy to a wearable through the bluetooth connection to the phone.