using System;
using Android.Content;
using System.Threading.Tasks;
using Android.Util;
using System.Threading;

namespace AppLifecycle
{
	/// <summary>
	/// Singleton class for Application wide objects. 
	/// </summary>
	public class App
	{
		public event EventHandler<UpdatingEventArgs> Updated = delegate {};

		public static App Current
		{
			get { return current; }
		} private static App current;

		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			// start a recurring update
			new Task (() => { 
				Random random = new Random( DateTime.Now.Millisecond );
				while(true){
					int num = random.Next(0, 1000);
					this.Updated ( this, new UpdatingEventArgs () { Message = num.ToString() } );
					System.Threading.Thread.Sleep(500);
				}
			}).Start ();
		}
	}
}

