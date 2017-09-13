using Android.App;
using Android.OS;
using Android.Support.Text.Emoji;
using Android.Support.V7.App;
using Android.Widget;

namespace EmojiCompatSample
{
	[Activity(Label = "EmojiCompat", MainLauncher = true, WindowSoftInputMode = Android.Views.SoftInput.StateUnchanged)]
	public class MainActivity : AppCompatActivity
	{
		// [U+1F469] (WOMAN) + [U+200D] (ZERO WIDTH JOINER) + [U+1F4BB] (PERSONAL COMPUTER)
		const string WomanTechnologist = "\uD83D\uDC69\u200D\uD83D\uDCBB";

	    // [U+1F469] (WOMAN) + [U+200D] (ZERO WIDTH JOINER) + [U+1F3A4] (MICROPHONE)
	    const string WomanSinger = "\uD83D\uDC69\u200D\uD83C\uDFA4";

		const string Emoji = WomanTechnologist + " " + WomanSinger;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			// TextView variant provided by EmojiCompat library
			var emojiTextView = (TextView) FindViewById(Resource.Id.emoji_text_view);
			emojiTextView.Text = GetString(Resource.String.emoji_text_view, Emoji);

			// EditText variant provided by EmojiCompat library
			var emojiEditText = (TextView) FindViewById(Resource.Id.emoji_edit_text);
			emojiEditText.Text = GetString(Resource.String.emoji_edit_text, Emoji);

			// Button variant provided by EmojiCompat library
			var emojiButton = (TextView) FindViewById(Resource.Id.emoji_button);
			emojiButton.Text = GetString(Resource.String.emoji_button, Emoji);

			// Regular TextView without EmojiCompat support; you have to manually process the text
			var regularTextView = (TextView) FindViewById(Resource.Id.regular_text_view);
			var textToShow = GetString(Resource.String.regular_text_view, Emoji);
			EmojiCompat.Get().RegisterInitCallback(new InitCallbackImpl 
			{
				RegularTextView = regularTextView,
				Text = textToShow
			});

			// Custom TextView
			var customTextView = (TextView) FindViewById(Resource.Id.emoji_custom_text_view);
			customTextView.Text = GetString(Resource.String.custom_text_view, Emoji);
		}

		class InitCallbackImpl : EmojiCompat.InitCallback 
		{
			public TextView RegularTextView { get; set; }
			public string Text { get; set; }

			public override void OnInitialized()
			{
				RegularTextView.TextFormatted = EmojiCompat.Get().ProcessFormatted(new Java.Lang.String(Text));
			}
		}
	}
}

