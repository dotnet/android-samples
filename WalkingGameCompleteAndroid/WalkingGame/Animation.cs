using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace WalkingGame
{
	public class Animation
	{
		// The frames in this animation
		List<AnimationFrame> frames = new List<AnimationFrame> ();

		// The amount of time into the animation
		TimeSpan timeIntoAnimation;

		// The length of the entire animation
		TimeSpan Duration
		{
			get
			{
				double totalSeconds = 0;
				foreach (var frame in frames)
				{
					totalSeconds += frame.Duration.TotalSeconds;
				}

				return TimeSpan.FromSeconds (totalSeconds);
			}
		}

		public Rectangle CurrentRectangle
		{
			get
			{
				AnimationFrame currentFrame = null;

				// See if we can find the frame
				TimeSpan accumulatedTime;
				foreach (var frame in frames)
				{
					if (accumulatedTime + frame.Duration >= timeIntoAnimation)
					{
						currentFrame = frame;
						break;
					}
					else
					{
						accumulatedTime += frame.Duration;
					}
				}

				// If no frame was found, then try the last frame, 
				// just in case timeIntoAnimation somehow exceeds Duration
				if (currentFrame == null)
				{
					currentFrame = frames.LastOrDefault ();
				}

				// If we found a frame, return its rectangle, otherwise
				// return an empty rectangle (one with no width or height)
				if (currentFrame != null)
				{
					return currentFrame.SourceRectangle;
				}
				else
				{
					return Rectangle.Empty;
				}
			}
		}

		// Adds a single frame to this animation.
		public void AddFrame (Rectangle rectangle, TimeSpan duration)
		{
			AnimationFrame newFrame = new AnimationFrame () {
				SourceRectangle = rectangle,
				Duration = duration
			};

			frames.Add (newFrame);
		}

		// Increases the timeIntoAnimation value according to the
		// frame time as obtained from gameTime
		public void Update (GameTime gameTime)
		{
			double secondsIntoAnimation = 
				timeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;


			double remainder = secondsIntoAnimation % Duration.TotalSeconds;

			timeIntoAnimation = TimeSpan.FromSeconds (remainder);
		}
	}
}