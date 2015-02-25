using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CameraProject
{
	public class Camera
	{
		GraphicsDevice graphicsDevice;

		// Let's start at X = 0 so we're looking at things head-on
		Vector3 position = new Vector3(0, 20, 10);

		float angle;

		public Matrix ViewMatrix
		{
			get
			{
				var lookAtVector = new Vector3 (0, -1, -.5f);
				// We'll create a rotation matrix using our angle
				var rotationMatrix = Matrix.CreateRotationZ (angle);
				// Then we'll modify the vector using this matrix:
				lookAtVector = Vector3.Transform (lookAtVector, rotationMatrix);
				lookAtVector += position;

				var upVector = Vector3.UnitZ;

				return  Matrix.CreateLookAt (
					position, lookAtVector, upVector);
			}
		} 





		public Matrix ProjectionMatrix
		{
			get
			{
				float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
				float nearClipPlane = 1;
				float farClipPlane = 200;
				float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
				
				return Matrix.CreatePerspectiveFieldOfView(
					fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
			}
		}

		public Camera(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
		}

public void Update(GameTime gameTime)
{
	TouchCollection touchCollection = TouchPanel.GetState();

	bool isTouchingScreen = touchCollection.Count > 0;
	if (isTouchingScreen)
	{
		var xPosition = touchCollection [0].Position.X;

		float xRatio = xPosition / (float)graphicsDevice.Viewport.Width;

		if (xRatio < 1 / 3.0f)
		{
			angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
		else if (xRatio < 2 / 3.0f)
		{
			var forwardVector = new Vector3 (0, -1, 0);

			var rotationMatrix = Matrix.CreateRotationZ (angle);
			forwardVector = Vector3.Transform (forwardVector, rotationMatrix);

			const float unitsPerSecond = 3;

			this.position += forwardVector * unitsPerSecond *
				(float)gameTime.ElapsedGameTime.TotalSeconds ;
		}
		else
		{
			angle -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
	}
}
	}
}

