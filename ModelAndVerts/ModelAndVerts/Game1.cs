using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModelAndVerts
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

		// This is the model instance that we'll load
		// our XNB into:
		Model model;

		VertexPositionTexture[] floorVerts;

		BasicEffect effect;

		Texture2D checkerboardTexture;

		Vector3 cameraPosition = new Vector3(15, 10, 10);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;
						
            Content.RootDirectory = "Content";
        }

		protected override void Initialize ()
		{
			floorVerts = new VertexPositionTexture[6];

			floorVerts [0].Position = new Vector3 (-20, -20, 0);
			floorVerts [1].Position = new Vector3 (-20,  20, 0);
			floorVerts [2].Position = new Vector3 ( 20, -20, 0);

			floorVerts [3].Position = floorVerts[1].Position;
			floorVerts [4].Position = new Vector3 ( 20,  20, 0);
			floorVerts [5].Position = floorVerts[2].Position;

			int repetitions = 20;

			floorVerts [0].TextureCoordinate = new Vector2 (0, 0);
			floorVerts [1].TextureCoordinate = new Vector2 (0, repetitions);
			floorVerts [2].TextureCoordinate = new Vector2 (repetitions, 0);

			floorVerts [3].TextureCoordinate = floorVerts[1].TextureCoordinate;
			floorVerts [4].TextureCoordinate = new Vector2 (repetitions, repetitions);
			floorVerts [5].TextureCoordinate = floorVerts[2].TextureCoordinate;

			effect = new BasicEffect (graphics.GraphicsDevice);

			base.Initialize ();
		}

		protected override void LoadContent()
		{
			// Notice that loading a model is very similar
			// to loading any other XNB (like a Texture2D).
			// The only difference is the generic type.
			model = Content.Load<Model> ("robot");

			// We aren't using the content pipeline, so we need
			// to access the stream directly:
			using (var stream = TitleContainer.OpenStream ("Content/checkerboard.png"))
			{
				checkerboardTexture = Texture2D.FromStream (this.GraphicsDevice, stream);
			}
		}

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
		{
		    GraphicsDevice.Clear(Color.CornflowerBlue);

			DrawGround ();

			DrawModel (new Vector3 (-4, 0, 3));
			DrawModel (new Vector3 ( 0, 0, 3));
			DrawModel (new Vector3 ( 4, 0, 3));

			DrawModel (new Vector3 (-4, 4, 3));
			DrawModel (new Vector3 ( 0, 4, 3));
			DrawModel (new Vector3 ( 4, 4, 3));

		    base.Draw(gameTime);
		}

		void DrawGround()
		{
			// The assignment of effect.View and effect.Projection
			// are nearly identical to the code in the Model drawing code.
			var cameraLookAtVector = Vector3.Zero;
			var cameraUpVector = Vector3.UnitZ;

			effect.View = Matrix.CreateLookAt (
				cameraPosition, cameraLookAtVector, cameraUpVector);

			float aspectRatio = 
				graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
			float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
			float nearClipPlane = 1;
			float farClipPlane = 200;

			effect.Projection = Matrix.CreatePerspectiveFieldOfView(
				fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

			// new code:
			effect.TextureEnabled = true;
			effect.Texture = checkerboardTexture;

			foreach (var pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply ();

				graphics.GraphicsDevice.DrawUserPrimitives (
							PrimitiveType.TriangleList,
					floorVerts,
					0,
					2);
			}
		}

		void DrawModel(Vector3 modelPosition)
		{
			foreach (var mesh in model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting ();
					effect.PreferPerPixelLighting = true;

					effect.World = Matrix.CreateTranslation (modelPosition);

					var cameraLookAtVector = Vector3.Zero;
					var cameraUpVector = Vector3.UnitZ;

					effect.View = Matrix.CreateLookAt (
						cameraPosition, cameraLookAtVector, cameraUpVector);

					float aspectRatio = 
						graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
					float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
					float nearClipPlane = 1;
					float farClipPlane = 200;

					effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

					
				}

				// Now that we've assigned our properties on the effects we can
				// draw the entire mesh
				mesh.Draw ();
			}
		}
    }
	                                                                                                                
}
