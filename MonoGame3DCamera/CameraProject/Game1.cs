using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CameraProject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

		VertexPositionNormalTexture[] floorVerts;

		BasicEffect effect;

		Texture2D checkerboardTexture;

		// New camera code
		Camera camera;

		Robot robot;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;
						
            Content.RootDirectory = "Content";
        }

		protected override void Initialize ()
		{
			floorVerts = new VertexPositionNormalTexture[6];

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

			robot = new Robot ();
			robot.Initialize (Content);

			// New camera code
			camera = new Camera (graphics.GraphicsDevice);

			base.Initialize ();
		}

		protected override void LoadContent()
		{
			using (var stream = TitleContainer.OpenStream ("Content/checkerboard.png"))
			{
				checkerboardTexture = Texture2D.FromStream (this.GraphicsDevice, stream);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			robot.Update (gameTime);
			// New camera code
			camera.Update (gameTime);
		    base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
		    GraphicsDevice.Clear(Color.CornflowerBlue);

			DrawGround ();

			// New camera code
			robot.Draw (camera);

		    base.Draw(gameTime);
		}

		void DrawGround()
		{
			// New camera code
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;

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
    }
}
