using System;
using CocosSharp;

namespace BouncingGame
{
	public class GameScene : CCScene
	{
		CCLayer mainLayer;
		CCSprite paddleSprite;

		CCSprite ballSprite;
		CCLabelTtf scoreLabel;

		float ballXVelocity;
		float ballYVelocity;
		// How much to modify the ball's y velocity per second:
		const float gravity = 140;

		int score;

		CCEventListenerTouchAllAtOnce touchListener;

		public GameScene(CCWindow mainWindow) : base(mainWindow)
		{
			mainLayer = new CCLayer ();
			AddChild (mainLayer);

			paddleSprite = new CCSprite ("paddle");
			paddleSprite.PositionX = 100;
			paddleSprite.PositionY = 100;
			mainLayer.AddChild (paddleSprite);

			ballSprite = new CCSprite ("ball");
			ballSprite.PositionX = 320;
			ballSprite.PositionY = 600;
			mainLayer.AddChild (ballSprite);

			scoreLabel = new CCLabelTtf ("Score: 0", "arial", 22);
			scoreLabel.PositionX = mainLayer.VisibleBoundsWorldspace.MinX + 20;
			scoreLabel.PositionY = mainLayer.VisibleBoundsWorldspace.MaxY - 20;
			scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;

			mainLayer.AddChild (scoreLabel);

			Schedule (RunGameLogic);

			// New code:
			touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesMoved = HandleTouchesMoved; 
			AddEventListener (touchListener, this);

		}

		void RunGameLogic(float frameTimeInSeconds)
		{
			// This is a linear approximation, so not 100% accurate
			ballYVelocity += frameTimeInSeconds * -gravity;
			ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
			ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;

			// Check if the two CCSprites overlap...
			bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(
				paddleSprite.BoundingBoxTransformedToParent);
			// ... and if the ball is moving downward.
			bool isMovingDownward = ballYVelocity < 0;
			if (doesBallOverlapPaddle && isMovingDownward)
			{
				// First let's invert the velocity:
				ballYVelocity *= -1;
				// Then let's assign a random to the ball's x velocity:
				const float minXVelocity = -300;
				const float maxXVelocity = 300;
				ballXVelocity = CCRandom.GetRandomFloat (minXVelocity, maxXVelocity);

				// New code:
				score++;
				scoreLabel.Text = "Score: " + score;
			}


			// Check if the ball is either too far to the right or left:
			float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
			float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;

			float screenRight = mainLayer.VisibleBoundsWorldspace.MaxX;
			float screenLeft = mainLayer.VisibleBoundsWorldspace.MinX;

			bool shouldReflectXVelocity = 
				(ballRight > screenRight && ballXVelocity > 0) ||
				(ballLeft < screenLeft && ballXVelocity < 0);

			if (shouldReflectXVelocity)
			{
				ballXVelocity *= -1;
			}

		}

		void HandleTouchesMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			// we only care about the first touch:
			var locationOnScreen = touches [0].LocationOnScreen;
			paddleSprite.PositionX = locationOnScreen.X;
		}

	}

}

