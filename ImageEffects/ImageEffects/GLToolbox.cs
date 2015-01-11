using System;
using Android.Opengl;

namespace ImageEffects
{
	public class GLToolbox
	{
		public static int LoadShader (int shaderType, string source)
		{
			int shader = GLES20.GlCreateShader (shaderType);
			if (shader != 0) {
				GLES20.GlShaderSource (shader, source);
				GLES20.GlCompileShader (shader);
				var compiled = new int[1];
				GLES20.GlGetShaderiv (shader, GLES20.GlCompileStatus, compiled, 0);
				if (compiled [0] == 0) {
					string info = GLES20.GlGetShaderInfoLog (shader);
					GLES20.GlDeleteShader (shader);
					throw new InvalidOperationException ("Could not compile shader " +
					shaderType + ":" + info);
				}
			}
			return shader;
		}

		public static int CreateProgram (string vertexSource,
		                                      string fragmentSource)
		{
			int vertexShader = LoadShader (GLES20.GlVertexShader, vertexSource);
			if (vertexShader == 0) {
				return 0;
			}

			int pixelShader = LoadShader (GLES20.GlFragmentShader, fragmentSource);
			if (pixelShader == 0) {
				return 0;
			}

			int program = GLES20.GlCreateProgram ();
			if (program != 0) {
				GLES20.GlAttachShader (program, vertexShader);
				CheckGLError ("glAttachShader");
				GLES20.GlAttachShader (program, pixelShader);
				CheckGLError ("glAttachShader");
				GLES20.GlLinkProgram (program);
				var linkStatus = new int[1];
				GLES20.GlGetProgramiv (program, GLES20.GlLinkStatus, linkStatus, 0);
				if (linkStatus [0] != GLES20.GlTrue) {
					String info = GLES20.GlGetProgramInfoLog (program);
					GLES20.GlDeleteProgram (program);
					throw new InvalidOperationException ("Could not link program: " + info);
				}
			}
			return program;
		}

		public static void InitTexParams ()
		{
			GLES20.GlTexParameteri (GLES20.GlTexture2d,
			                                GLES20.GlTextureMagFilter, GLES20.GlLinear);
			GLES20.GlTexParameteri (GLES20.GlTexture2d,
			                                GLES20.GlTextureMinFilter, GLES20.GlLinear);
			GLES20.GlTexParameteri (GLES20.GlTexture2d, GLES20.GlTextureWrapS,
			                                GLES20.GlClampToEdge);
			GLES20.GlTexParameteri (GLES20.GlTexture2d, GLES20.GlTextureWrapT,
			                                GLES20.GlClampToEdge);
		}

		public static void CheckGLError (string op)
		{
			int error;
			while ((error = GLES20.GlGetError ()) != GLES20.GlNoError) {
				throw new InvalidOperationException (op + ": glError " + error);
			}
		}
	}
}