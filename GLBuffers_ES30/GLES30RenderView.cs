using System;
using System.Runtime.InteropServices;
using System.Text;

using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Util;
using Android.Views;
using Android.Content;
using Android.Content.Res;
using System.IO;
using OpenTK;

using System.Diagnostics;

// Render a triangle using OpenGLES 3.0

namespace Mono.Samples.GLBufferES30
{
    public unsafe class GLES30RenderView : AndroidGameView
    {
        /// <summary>
        /// Camera struct, mathes vertex shader uniform object
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct CameraStruct
        {
            public Matrix4 view;
            public Matrix4 projection;
            public Matrix4 viewProjection;
        }

        private Stopwatch stopWatch = Stopwatch.StartNew();

        //Quad vertex buffer handle
        private int vertexBufferHandle;

        //Vertex array object handle
        private int vaoHandle;

        private int colorBlockIndex;
        private int objectBlockIndex;
        private int cameraBlockIndex;

        /*Those are handles for uniform buffer objects. Color for fragment,
         object/camera are also split since they have a different update frequency */
        private int colorBufferHandle;
        private int objectBufferHandle;
        private int cameraBufferHandle;

        //Our single shader handle
        private int program;

        public GLES30RenderView(Context context) : base(context)
        {
        }

        /// <summary>
        /// Loads text from an asset file
        /// </summary>
        /// <param name="path">Path in asset folder</param>
        /// <returns>File content</returns>
        private string LoadAssetText(string path)
        {
            using (StreamReader sr = new StreamReader(this.Context.Assets.Open(path)))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Small static function to create a single buffer
        /// </summary>
        /// <returns>Buffer handle</returns>
        private static int GenerateBuffer()
        {
            int[] bd = new int[1];
            GL.GenBuffers(1, bd);
            return bd[0];
        }

        /// <summary>
        /// Small static function to create a single vertex array object
        /// </summary>
        /// <returns>Vertex array object handle</returns>
        private static int GenerateVertexArray()
        {
            int[] array = new int[1];
            GL.GenVertexArrays(1, array);
            return array[0];
        }

        /// <summary>
        /// Binds handle to a specific index
        /// </summary>
        /// <param name="bufferHandle">Buffer handle</param>
        /// <param name="slot">Slot</param>
        private void BindUniformBuffer(int bufferHandle, int slot)
        {
            GL.BindBufferBase(All.UniformBuffer, slot, bufferHandle);
        }

        /// <summary>
        /// Creates vertex buffer from quad
        /// </summary>
        private void CreateVertexBuffer()
        {
            float[] vertices = new float[]
            {
            -0.25f, 0.25f, 0.0f,
            -0.25f, -0.25f, 0.0f,
            0.25f, 0.25f, 0.0f,
            0.25f, -0.25f, 0.0f,
            };

            //Creates and copies vertex buffer data
            vertexBufferHandle = GenerateBuffer();
            GL.BindBuffer(All.ArrayBuffer, this.vertexBufferHandle);

            GL.BufferData<float>(All.ArrayBuffer, new IntPtr(4 *vertices.Length), vertices, All.StaticDraw);

            GL.BindBuffer(All.ArrayBuffer, 0);
        }

        /// <summary>
        /// Creates vertex array object (note : in that case we only have position buffer)
        /// </summary>
        private void CreateVAO()
        {
            this.vaoHandle = GenerateVertexArray();

            GL.BindVertexArray(this.vaoHandle);

            int componentCount = 3; //3d vector
            int positionStride = sizeof(float) * componentCount;

            //This should be done for all attribute, attach vertex buffer and point to gpu data
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(All.ArrayBuffer, this.vertexBufferHandle);
          
            GL.VertexAttribPointer(0, componentCount, All.Float, false, positionStride, (IntPtr)0);

            GL.BindBuffer(All.ArrayBuffer, 0);

            //Detach our vao
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Writes data to an opengl buffer
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="bufferHandle">handle</param>
        /// <param name="value">value</param>
        private static unsafe void WriteToUniformBuffer<T>(int bufferHandle, T value) where T : struct
        {
            GL.BindBuffer(All.UniformBuffer, bufferHandle);
            GL.BufferData<T>(All.UniformBuffer, (IntPtr)Marshal.SizeOf<T>(), ref value, All.DynamicDraw);
            GL.BindBuffer(All.UniformBuffer, 0);
        }

        /// <summary>
        /// Writes data to an opengl buffer (byref version)
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="bufferHandle">handle</param>
        /// <param name="value">value</param>
        private static unsafe void WriteToUniformBuffer<T>(int bufferHandle, ref T value) where T : struct
        {
            GL.BindBuffer(All.UniformBuffer, bufferHandle);
            GL.BufferData<T>(All.UniformBuffer, (IntPtr)Marshal.SizeOf<T>(), ref value, All.DynamicDraw);
            GL.BindBuffer(All.UniformBuffer, 0);
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            GLContextVersion = GLContextVersion.Gles3_0;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try {
                Log.Verbose("GLTriangle", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            } catch (Exception ex) {
                Log.Verbose("GLTriangle", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try {
                Log.Verbose("GLTriangle", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode();//0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            } catch (Exception ex) {
                Log.Verbose("GLTriangle", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        protected override void OnLoad(EventArgs e)
        {
            // This is completely optional and only needed
            // if you've registered delegates for OnLoad
            base.OnLoad(e);

            //Loads shader sources
            string vertexShaderSrc = LoadAssetText("transformVertex.vert");
            string fragmentShaderSrc = LoadAssetText("solidColor.frag");


            int vertexShader = LoadShader(All.VertexShader, vertexShaderSrc);
            int fragmentShader = LoadShader(All.FragmentShader, fragmentShaderSrc);
            program = GL.CreateProgram();
            if (program == 0)
                throw new InvalidOperationException("Unable to create program");

            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);

            GL.BindAttribLocation(program, 0, "vPosition");
            GL.LinkProgram(program);

            int linked;
            GL.GetProgram(program, All.LinkStatus, out linked);
            if (linked == 0)
            {
                // link failed
                int length = 0;
                GL.GetProgram(program, All.InfoLogLength, out length);
                if (length > 0)
                {
                    var log = new StringBuilder(length);
                    GL.GetProgramInfoLog(program, length, out length, log);
                    Log.Debug("GL2", "Couldn't link program: " + log.ToString());
                }

                GL.DeleteProgram(program);
                throw new InvalidOperationException("Unable to link program");
            }

            //Vertex buffer and vertex array object
            CreateVertexBuffer();
            CreateVAO();

            //Those are our 3 uniform buffers
            this.colorBufferHandle = GenerateBuffer();
            this.objectBufferHandle = GenerateBuffer();
            this.cameraBufferHandle = GenerateBuffer();

            /*Here we get the block location for each uniform object in the linked program.
            Please note that in ES31 we can directly assign layout index in the shader itself, 
            sparing reflection here */
            colorBlockIndex = GL.GetUniformBlockIndex(program, new StringBuilder("cbColor"));
            objectBlockIndex = GL.GetUniformBlockIndex(program, new StringBuilder("cbObject"));
            cameraBlockIndex = GL.GetUniformBlockIndex(program, new StringBuilder("cbCamera"));

            Run();
        }

        int LoadShader(All type, string source)
        {
            int shader = GL.CreateShader(type);
            if (shader == 0)
                throw new InvalidOperationException("Unable to create shader");

            int length = 0;
            GL.ShaderSource(shader, 1, new string[] { source }, (int[])null);
            GL.CompileShader(shader);

            int compiled = 0;
            GL.GetShader(shader, All.CompileStatus, out compiled);
            if (compiled == 0)
            {
                length = 0;
                GL.GetShader(shader, All.InfoLogLength, out length);
                if (length > 0)
                {
                    var log = new StringBuilder(length);
                    GL.GetShaderInfoLog(shader, length, out length, log);
                    Log.Debug("GL2", "Couldn't compile shader: " + log.ToString());
                }

                GL.DeleteShader(shader);
                throw new InvalidOperationException("Unable to compile shader of type : " + type.ToString());
            }

            return shader;
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            float currentTime = (float)stopWatch.Elapsed.TotalSeconds;

            base.OnRenderFrame(e);

            GL.Viewport(0, 0, this.Width, this.Height);

            GL.ClearColor (0.7f, 0.7f, 0.7f, 1);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			//Attach our shader
			GL.UseProgram (program);

            //Some defined bind points for our uniform buffers
            int colorBindPoint = 0;
            int cameraBindPoint = 1;
            int objectBindPoint = 2;

            //Binds uniforms to some user defined slots
            BindUniformBuffer(objectBufferHandle, objectBindPoint);
            BindUniformBuffer(cameraBufferHandle, cameraBindPoint);
            BindUniformBuffer(colorBufferHandle, colorBindPoint);

            //Set shader uniform binding
            GL.UniformBlockBinding(program, objectBlockIndex, objectBindPoint);
            GL.UniformBlockBinding(program, cameraBlockIndex, cameraBindPoint);
            GL.UniformBlockBinding(program, colorBlockIndex, colorBindPoint);

            Matrix4 world = Matrix4.Identity;
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(OpenTK.MathHelper.DegreesToRadians(45.0f), (float)this.Width / (float)this.Height, 0.01f, 100.0f);
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, -4), Vector3.Zero, Vector3.UnitY);

            CameraStruct camera = new CameraStruct()
            {
                view = view,
                projection = proj,
                viewProjection = view * proj
            };

            //Copy camera and color data
            WriteToUniformBuffer(cameraBufferHandle, ref camera);
            WriteToUniformBuffer(colorBufferHandle, new OpenTK.Vector4(1.0f, 0.0f, 0.0f, 0.0f));

            //Attach our rectangle
            GL.BindVertexArray(this.vaoHandle);

            //Sets first quad transform and draw it
            world = Matrix4.CreateRotationZ(currentTime) * Matrix4.CreateTranslation(-0.5f, 0.0f, 0.0f);
            WriteToUniformBuffer(objectBufferHandle, ref world);
            GL.DrawArrays(All.TriangleStrip, 0, 4);

            //Sets second quad transform and draw it
            world = Matrix4.CreateRotationZ(currentTime) * Matrix4.CreateTranslation(0.5f, 0.0f, 0.0f);
            WriteToUniformBuffer(objectBufferHandle, ref world);
            GL.DrawArrays(All.TriangleStrip, 0, 4);

            //unbind vao
            GL.BindVertexArray(0);

            SwapBuffers ();
		}

		protected override void OnResize (EventArgs e)
		{
			MakeCurrent ();
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);


            GL.DeleteBuffers(4, new int[]
            {
                this.vertexBufferHandle,
                this.colorBufferHandle,
                this.objectBufferHandle,
                this.cameraBufferHandle
            });

            GL.DeleteProgram(this.program);

            GL.DeleteVertexArrays(1, new int[]
            {
                this.vaoHandle
            });
        }
    }
}
