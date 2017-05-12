GL Buffers ES30
==============

This is a more "strict" version of GLES30 rendering

* Uses Vertex Array object (vao) instead of having to use Attrib pointer (and glfinish)
* Uses Uniform buffer object (and their bindings)
* Shaders are set in the asset folder (since authoring shader inside a c# string is really no fun...)
* Shaders use es30 flags (in/out attributes instead of deprecated varying) and explicit layouts.


Requirements
------------

There is one requirement to run this sample:

1. A device with OpenGL ES 3.0 support.

Note that emulators targeting API levels 1 through 13 provide only OpenGL ES 1.0 support, not 2.0.

