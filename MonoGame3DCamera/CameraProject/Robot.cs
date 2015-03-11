using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CameraProject
{
	public class Robot
	{
		Model model;

		// new code:
		float angle;

		public void Initialize(ContentManager contentManager)
		{
			model = contentManager.Load<Model> ("robot");

		}

public void Update(GameTime gameTime)
{
	// TotalSeconds is a double so we need to cast to float
	angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
}

public void Draw(Camera camera)
{
	foreach (var mesh in model.Meshes)
	{
		foreach (BasicEffect effect in mesh.Effects)
		{
			effect.EnableDefaultLighting ();
			effect.PreferPerPixelLighting = true;

			effect.World = GetWorldMatrix();
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
		}

		mesh.Draw ();
	}
}

Matrix GetWorldMatrix()
{
	const float circleRadius = 8;
	const float heightOffGround = 3;
	
	// this matrix moves the model "out" from the origin
	Matrix translationMatrix = Matrix.CreateTranslation (
		circleRadius, 0, heightOffGround);

	// this matrix rotates everything around the origin
	Matrix rotationMatrix = Matrix.CreateRotationZ (angle);

	// We combine the two to have the model move in a circle:
			Matrix combined = translationMatrix * rotationMatrix;

	return combined;
}
	}
}

