using Android.App;
using Android.OS;

namespace EmojiCompat
{
	[Activity(Label = "EmojiCompat", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);
		}
	}
}

