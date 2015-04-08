using System;
using System.Collections.Generic;
using CocosSharp;

namespace EntityProject
{
	public class GameLayer : CCLayer
	{
		Ship ship;
		List<Bullet> bullets;

	public GameLayer ()
	{
		ship = new Ship ();
		ship.PositionX = 240;
		ship.PositionY = 50;
		this.AddChild (ship);

		bullets = new List<Bullet> ();
		BulletFactory.Self.BulletCreated += HandleBulletCreated;
	}

	void HandleBulletCreated(Bullet newBullet)
	{
		AddChild (newBullet);
		bullets.Add (newBullet);
	}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();

			// Use the bounds to layout the positioning of our drawable assets
			CCRect bounds = VisibleBoundsWorldspace;

			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;
			AddEventListener (touchListener, this);
		}

		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
				// Perform touch handling here
			}
		}
	}
}
