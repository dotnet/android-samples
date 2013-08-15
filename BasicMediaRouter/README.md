Basic Media Router
==================
This sample demonstrates the use of the MediaRouter API to show content on a
secondary display using a Presentation.

The activity uses the MediaRouter API to automatically detect when a
presentation display is available and to allow the user to control the media
routes using a menu item provided by the MediaRouteActionProvider. When a 
presentation display is available a Presentation (implemented as a SamplePresentation)
is shown on the preferred display. A button toggles the background color of the 
secondary screen to show the interaction wbetween the primary and secondary screens.

This sample requires an HDMI or Wifi display. Alternatively, the
"Simulate secondary displays" feature in Development Settings can be enabled
to simulate secondary displays.

Based on the Google Android-18 sample "BasicMediaRouter".
Ported by: Peter Collins