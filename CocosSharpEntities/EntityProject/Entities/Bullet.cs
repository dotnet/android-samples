using System;
using CocosSharp;

namespace EntityProject
{
	public class Bullet : CCNode
	{
		CCSprite sprite;

		public float VelocityX
		{
			get;
			set;
		}

		public float VelocityY
		{
			get;
			set;
		}

		public Bullet () : base()
		{
			sprite = new CCSprite ("bullet.png");
			// Making the Sprite be centered makes
			// positioning easier.
			sprite.AnchorPoint = CCPoint.AnchorMiddle;
			this.AddChild(sprite);

			this.Schedule (ApplyVelocity);
		}

		void ApplyVelocity(float time)
		{
			PositionX += VelocityX * time;
			PositionY += VelocityY * time;
		}
	}
}

