using System;
using CocosSharp;

namespace EntityProject
{
	public class Ship : CCNode
	{
		CCSprite sprite;

        CCEventListenerTouchAllAtOnce touchListener;

public Ship () : base()
{
	sprite = new CCSprite ("ship.png");
	// Center the Sprite in this entity to simplify
	// centering the Ship on screen
	sprite.AnchorPoint = CCPoint.AnchorMiddle;
	this.AddChild(sprite);

    touchListener = new CCEventListenerTouchAllAtOnce();
	touchListener.OnTouchesMoved = HandleInput;
    AddEventListener(touchListener, this);

	Schedule (FireBullet, interval: 0.5f);

}

void FireBullet(float unusedValue)
{
	Bullet newBullet = BulletFactory.Self.CreateNew ();
	newBullet.Position = this.Position;
	newBullet.VelocityY = 100;
}

        private void HandleInput(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            if(touches.Count > 0)
            {
                CCTouch firstTouch = touches[0];

                this.PositionX = firstTouch.Location.X;
                this.PositionY = firstTouch.Location.Y;
            }
        }
	}
}

