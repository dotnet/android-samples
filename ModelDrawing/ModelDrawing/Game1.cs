using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModelDrawing
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

		// This is the model instance that we'll load
		// our XNB into:
		Model model;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;
						
            Content.RootDirectory = "Content";
        }
        protected override void LoadContent()
        {
			// Notice that loading a model is very similar
			// to loading any other XNB (like a Texture2D).
			// The only difference is the generic type.
			model = Content.Load<Model> ("robot");
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			DrawModel (new Vector3 (-4, 0, 0));
			DrawModel (new Vector3 ( 0, 0, 0));
			DrawModel (new Vector3 ( 4, 0, 0));


			DrawModel (new Vector3 (-4, 0, 3));
			DrawModel (new Vector3 ( 0, 0, 3));
			DrawModel (new Vector3 ( 4, 0, 3));

            base.Draw(gameTime);
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

					var cameraPosition = new Vector3 (0, 10, 0);
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
