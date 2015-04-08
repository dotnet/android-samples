using System;

namespace EntityProject
{
	public class BulletFactory
	{
		static BulletFactory self;

		// simple singleton implementation
		public static BulletFactory Self
		{
			get
			{
				if (self == null)
				{
					self = new BulletFactory ();
				}
				return self;
			}
		}

		public event Action<Bullet> BulletCreated;

		private BulletFactory()
		{

		}

		public Bullet CreateNew()
		{
			Bullet newBullet = new Bullet ();

			if (BulletCreated != null)
			{
				BulletCreated (newBullet);
			}

			return newBullet;
		}
	}
}

