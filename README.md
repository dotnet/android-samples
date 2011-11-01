MonoDroid Samples
=================

MonoDroid samples, showing use of Android API wrappers from C#.

Samples
-------

The following samples are provided:

  - ApiDemo: Shows how to use various Android APIs.

  - Button: A full-screen button with click counter, demonstrates use of C#
    events to handle UI events.

  - ContactManager: Shows how to access contacts stored on an Android device.

  - GLCube: A simple colored cube that keeps rotating about its center, rendered
    via OpenGLES and OpenTK, via AndroidGameView.

  - GLTriangle20: Renders a simple triangle using OpenGLES 2.0 shaders.

  - HelloWorld: A port of the simple Android Hello World application.
    Also demonstrates use of resources.

  - HoneycombGallery: Shows how to use some of the tablet APIs introduced in
    Android 3.0 (Honeycomb).

  - JeyBoy: Shows how to create an advanced music game.

  - JniDemo: Shows how to create manual bindings to Java libraries so they
    can be consumed by Mono for Android applications.

  - LabelledSections: Shows how to create a list with section labels.

  - LiveWallpaperDemo: Shows how to create a live wallpaper.

  - MapsDemo: Shows how to use the GoogleMaps bindings.

  - MultiResolution: Demonstrates embedded resources and UI integration.

  - SimpleWidget: Shows how to create an Android desktop widget.

  - SkeletonApp: Demonstrates text editing and keyboard support.

  - Snake: Shows how to create a simple Snake game.

  - TexturedCube: OpenGLES and OpenTK demonstration.  Drag the cube around to
    see it rotate.


Contributing
------------

Before adding a sample to the repository, please run either install-hook.bat
or install-hook.sh depending on whether you're on Windows or a Posix system.
This will install a Git hook that runs the Xamarin code sample validator before
a commit, to ensure that all samples are good to go.
