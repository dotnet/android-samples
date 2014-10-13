using System;
using CocosSharp;

namespace BouncingGame
{
	public class GameAppDelegate : CCApplicationDelegate
	{
		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";

			mainWindow.SupportedDisplayOrientations = CCDisplayOrientation.Portrait;

			GameScene gameScene = new GameScene (mainWindow);
			mainWindow.RunWithScene (gameScene);

		}

		public override void ApplicationDidEnterBackground (CCApplication application)
		{
		}

		public override void ApplicationWillEnterForeground (CCApplication application)
		{
		}
	}
}
