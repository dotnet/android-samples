using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WalkingGame
{
	public class MainGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		CharacterEntity character;

		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			character = new CharacterEntity (this.GraphicsDevice);

			base.Initialize();
		} 


		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
		} 


		protected override void Update(GameTime gameTime)
		{ 
			character.Update (gameTime);
			base.Update(gameTime);
		} 

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			    // We'll start all of our drawing here:
			spriteBatch.Begin ();

			// Now we can do any entity rendering:
			character.Draw(spriteBatch);
			// End renders all sprites to the screen:
			spriteBatch.End ();

			base.Draw(gameTime);
		} 
	}
}