using System;
using OpenTK;
using OpenTK.Graphics.ES31;

namespace Mono.Samples.Tetrahedron
{
	public class Tetrahedron
	{
		public float XAngle = 0, YAngle = 0;
		public float XAcc, YAcc;
		public float XSign = 1, YSign = 1;
		const float xInc = 0;
		const float yInc = .0066f;
		int Width, Height;
		int textureId;
		int programCompute, programRender;
		int level, maxLevel = 7, pieces, length;
		int counter = 0;

		const int UNIFORM_PROJECTION = 0;
		const int UNIFORM_LIGHT = 1;
		const int UNIFORM_VIEW = 2;
		const int UNIFORM_NORMAL_MATRIX = 3;
		const int UNIFORM_LEVEL = 4;
		const int UNIFORM_COUNT = 5;
		int[] uniforms = new int [UNIFORM_COUNT];
		const int ATTRIB_VERTEX = 0;
		const int ATTRIB_NORMAL = 1;
		const int ATTRIB_BAYCENTRIC = 2;
		const int ATTRIB_TEXCOORD = 3;
		const int ATTRIB_COUNT = 4;
		int vbo, vbi;

		public Tetrahedron ()
		{
		}

		void ComputeValues ()
		{
			pieces = 1;
			length = 1;
			for (int i = 0; i < level; i++) {
				pieces *= 4;
				length *= 2;
			}
		}

		internal void Initialize ()
		{
			GL.ClearColor (0, 0, 0, 1);

			GL.ClearDepth (1.0f);
			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.CullFace);
			GL.CullFace (CullFaceMode.Back);
			GL.Hint (HintTarget.GenerateMipmapHint, HintMode.Nicest);

			textureId = GL.GenTexture ();
			LoadShaders (LoadResource ("Mono.Samples.SierpinskiTetrahedron.Resources.Shader.csh"), null, null, out programCompute);
			LoadShaders (null, LoadResource ("Mono.Samples.SierpinskiTetrahedron.Resources.Shader.vsh"), LoadResource ("Mono.Samples.SierpinskiTetrahedron.Resources.Shader.fsh"), out programRender);
			GetUniforms ();
			level = maxLevel;
			ComputeValues ();
			InitModel ();
			level = 0;
			ComputeValues ();
		}

		internal void DeleteTexture ()
		{
			GL.DeleteTexture (textureId);
		}

		public void Render ()
		{
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Use shader program.
			GL.UseProgram (programRender);

			// Update uniform value.
			GL.UniformMatrix4 (uniforms [UNIFORM_PROJECTION], false, ref projection);
			GL.UniformMatrix4 (uniforms [UNIFORM_VIEW], false, ref view);
			GL.UniformMatrix4 (uniforms [UNIFORM_NORMAL_MATRIX], false, ref normalMatrix);
			GL.Uniform3 (uniforms [UNIFORM_LIGHT], 2.5f, 2.5f, 2.1f);
			GL.Uniform1 (uniforms [UNIFORM_LEVEL], (float) level);

			DrawModel ();

			const int frames = 90;
			counter++;
			if (counter == (2*maxLevel)*frames)
				counter = 0;
			if (counter % frames == 0) {
				level = counter / frames;
				if (level > maxLevel)
					level = 2 * maxLevel - level;
				ComputeValues ();
				SetupProjection (Width, Height);
			}
		}

		string LoadResource (string name)
		{
			return new System.IO.StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (name)).ReadToEnd ();
		}

		static readonly int vboBinding = 0;
		static readonly int vbiBinding = 1;

		public void InitModel ()
		{
			GL.GenBuffers (1, out vbo);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr (sizeof(float) * 12 * (2 * 4 + 4) * pieces), IntPtr.Zero, BufferUsage.StaticDraw);
			GL.BindBuffer (BufferTarget.ArrayBuffer, 0);

			GL.GenBuffers (1, out vbi);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vbi);
			GL.BufferData (BufferTarget.ElementArrayBuffer, new IntPtr (sizeof(UInt32) * 3 * 4 * pieces), IntPtr.Zero, BufferUsage.StaticDraw);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);

			ComputeModel ();
		}

		void ComputeModel ()
		{
			GL.UseProgram (programCompute);

			GL.BindBufferBase (BufferRangeTarget.ShaderStorageBuffer, vboBinding, vbo);
			GL.BindBufferBase (BufferRangeTarget.ShaderStorageBuffer, vbiBinding, vbi);

			GL.DispatchCompute (pieces, 1, 1);
			GL.MemoryBarrier (MemoryBarrierMask.VertexAttribArrayBarrierBit | MemoryBarrierMask.ElementArrayBarrierBit);

			GL.BindBufferBase (BufferRangeTarget.ShaderStorageBuffer, vboBinding, 0);
			GL.BindBufferBase (BufferRangeTarget.ShaderStorageBuffer, vbiBinding, 0);
		}

		internal void DrawModel ()
		{
			// Update attribute values.
			GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
			GL.VertexAttribPointer (ATTRIB_VERTEX, 4, VertexAttribPointerType.Float, false, sizeof(float)*12, IntPtr.Zero);
			GL.EnableVertexAttribArray (ATTRIB_VERTEX);

			GL.VertexAttribPointer (ATTRIB_NORMAL, 4, VertexAttribPointerType.Float, false, sizeof(float)*12, new IntPtr (sizeof (float)*4));
			GL.EnableVertexAttribArray (ATTRIB_NORMAL);

			GL.VertexAttribPointer (ATTRIB_BAYCENTRIC, 3, VertexAttribPointerType.Float, false, sizeof(float)*12, new IntPtr (sizeof (float)*8));
			GL.EnableVertexAttribArray (ATTRIB_BAYCENTRIC);

			GL.BindBuffer (BufferTarget.ElementArrayBuffer, vbi);
			GL.DrawElements (BeginMode.Triangles, 3*4*pieces, DrawElementsType.UnsignedInt, IntPtr.Zero);

			GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
		}

		Matrix4 view = new Matrix4 ();
		Matrix4 normalMatrix = new Matrix4 ();
		Matrix4 projection = new Matrix4 ();

		internal void SetupProjection (int width, int height)
		{
			Matrix4 model = Matrix4.CreateTranslation (-(float)length/2f + .5f, (float)(-(length*0.5)/System.Math.Sqrt (3) + .5/System.Math.Sqrt (3)), (float)(-length/System.Math.Sqrt (6) + .5/System.Math.Sqrt (6)));
			model = Matrix4.Mult (model, Matrix4.Mult (Matrix4.CreateRotationX (-XAngle), Matrix4.CreateRotationZ (-YAngle)));
			model = Matrix4.Mult (model, Matrix4.Scale(5f/length));
			float aspect = (float)width / height;
			if (aspect > 1) {
				Matrix4 scale = Matrix4.Scale (aspect);
				model = Matrix4.Mult (model, scale);
			}
			view = Matrix4.Mult (model, Matrix4.LookAt (0, -10, .8f, 0, 10, -1.6f, 0, 1, 0));
			projection = Matrix4.CreatePerspectiveFieldOfView (OpenTK.MathHelper.DegreesToRadians (42.0f), aspect, 1.0f, 200.0f);
			projection = Matrix4.Mult (view, projection);
			normalMatrix = Matrix4.Invert (view);
			normalMatrix.Transpose ();

			Width = width;
			Height = height;
		}

		void GetUniforms ()
		{
			GL.UseProgram (programRender);

			// Get uniform locations.
			uniforms [UNIFORM_PROJECTION] = GL.GetUniformLocation (programRender, "projection");
			uniforms [UNIFORM_VIEW] = GL.GetUniformLocation (programRender, "view");
			uniforms [UNIFORM_NORMAL_MATRIX] = GL.GetUniformLocation (programRender, "normalMatrix");
			uniforms [UNIFORM_LIGHT] = GL.GetUniformLocation (programRender, "light");
			uniforms [UNIFORM_LEVEL] = GL.GetUniformLocation (programRender, "level");
		}

		bool LoadShaders (string compShaderSource, string vertShaderSource, string fragShaderSource, out int program)
		{
			int compShader, vertShader, fragShader;

			// Create shader program.
			program = GL.CreateProgram ();

			// Create and compile compute shader.
			if (compShaderSource != null) {
				if (!CompileShader (ShaderType.ComputeShader, compShaderSource, out compShader)) {
					Console.WriteLine ("Failed to compile vertex shader");
					return false;
				}
			} else
				compShader = 0;
			// Create and compile vertex shader.
			if (vertShaderSource != null) {
				if (!CompileShader (ShaderType.VertexShader, vertShaderSource, out vertShader)) {
					Console.WriteLine ("Failed to compile vertex shader");
					return false;
				}
			} else
				vertShader = 0;
			// Create and compile fragment shader.
			if (fragShaderSource != null) {
				if (!CompileShader (ShaderType.FragmentShader, fragShaderSource, out fragShader)) {
					Console.WriteLine ("Failed to compile fragment shader");
					return false;
				}
			} else
				fragShader = 0;

			// Attach compute shader to program.
			if (compShader > 0)
				GL.AttachShader (program, compShader);

			// Attach vertex shader to program.
			if (vertShader > 0) {
				GL.AttachShader (program, vertShader);
				// Bind attribute locations.
				// This needs to be done prior to linking.
				GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
				GL.BindAttribLocation (program, ATTRIB_NORMAL, "normal");
				GL.BindAttribLocation (program, ATTRIB_BAYCENTRIC, "baycentric");
			}

			// Attach fragment shader to program.
			if (fragShader > 0)
				GL.AttachShader (program, fragShader);

			// Link program.
			if (!LinkProgram (program)) {
				Console.WriteLine ("Failed to link program: {0:x}", program);

				if (vertShader != 0)
					GL.DeleteShader (vertShader);

				if (fragShader != 0)
					GL.DeleteShader (fragShader);

				if (program != 0) {
					GL.DeleteProgram (program);
					program = 0;
				}
				return false;
			}

			// Release vertex and fragment shaders.
			if (vertShader != 0) {
				GL.DetachShader (program, vertShader);
				GL.DeleteShader (vertShader);
			}

			if (fragShader != 0) {
				GL.DetachShader (program, fragShader);
				GL.DeleteShader (fragShader);
			}

			return true;
		}

		internal void DestroyShaders ()
		{
			if (programRender != 0) {
				GL.DeleteProgram (programRender);
				programRender = 0;
			}
		}

		static bool CompileShader (ShaderType type, string src, out int shader)
		{
			shader = GL.CreateShader (type);
			GL.ShaderSource (shader, src);
			GL.CompileShader (shader);

			#if DEBUG || true
			int logLength = 0;
			GL.GetShader (shader, ShaderParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				Console.WriteLine ("Shader compile log:\n{0}", GL.GetShaderInfoLog (shader));
			}
			#endif

			int status = 0;
			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);
			if (status == 0) {
				GL.DeleteShader (shader);
				return false;
			}

			return true;
		}

		static bool LinkProgram (int prog)
		{
			GL.LinkProgram (prog);

			#if DEBUG
			int logLength = 0;
			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				var infoLog = new System.Text.StringBuilder (logLength);
				GL.GetProgramInfoLog (prog, logLength, out logLength, infoLog);
				Console.WriteLine ("Program link log:\n{0}", infoLog);
			}
			#endif
			int status = 0;
			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
			if (status == 0)
				return false;

			return true;
		}

		static void CheckGLError ()
		{
			ErrorCode code = GL.GetErrorCode ();
			if (code != ErrorCode.NoError) {
				Console.WriteLine ("GL Error {0}", code);
			}
		}

		static bool ValidateProgram (int prog)
		{
			GL.ValidateProgram (prog);

			int logLength = 0;
			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				var infoLog = new System.Text.StringBuilder (logLength);
				GL.GetProgramInfoLog (prog, logLength, out logLength, infoLog);
				Console.WriteLine ("Program validate log:\n{0}", infoLog);
			}

			int status = 0;
			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
			if (status == 0)
				return false;

			return true;
		}

		public void UpdateWorld ()
		{
			XAngle += XSign * (xInc + XAcc * XAcc);
			YAngle += YSign * (yInc + YAcc * YAcc);
			SetupProjection (Width, Height);
			XAcc = System.Math.Max (0, XAcc - 0.001f);
			YAcc = System.Math.Max (0, YAcc - 0.001f);
		}

		public void Move (float xDiff, float yDiff)
		{
			XSign = yDiff > 0 ? 1 : -1;
			YSign = xDiff > 0 ? 1 : -1;
			xDiff = YSign*System.Math.Min (30, System.Math.Abs (xDiff));
			yDiff = XSign*System.Math.Min (30, System.Math.Abs (yDiff));
			XAngle += yDiff / 200;
			YAngle += xDiff / 200;
			XAcc = System.Math.Abs (yDiff / 100);
			YAcc = System.Math.Abs (xDiff / 100);
		}
	}
}