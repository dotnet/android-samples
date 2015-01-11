using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace WalkingGame
{
	public class Animation
	{
		List<AnimationFrame> frames = new List<AnimationFrame>();
		TimeSpan timeIntoAnimation;

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
		foreach(var frame in frames)
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

		public void AddFrame(Rectangle rectangle, TimeSpan duration)
		{
			AnimationFrame newFrame = new AnimationFrame()
			{
				SourceRectangle = rectangle,
				Duration = duration
			};

			frames.Add(newFrame);
		}

		public void Update(GameTime gameTime)
		{
			double secondsIntoAnimation = 
				timeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;


			double remainder = secondsIntoAnimation % Duration.TotalSeconds;

			timeIntoAnimation = TimeSpan.FromSeconds (remainder);
		}
	}
}

