using Android.Crypto.Hpke;
using Android.Views;

namespace HpkeEncryption;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
	private EditText? _input;
	private TextView? _ciphertextView, _plaintextView, _statusView;
	private Button? _btnSeal, _btnOpen;
	private Java.Security.KeyPair _keyPair = null!;
	private Hpke _hpke = null!;
	private Message? _sealedMessage;

	static readonly byte[] Info = System.Text.Encoding.UTF8.GetBytes("hpke-sample");

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		var root = new LinearLayout(this) { Orientation = Orientation.Vertical };
		root.SetPadding(32, 64, 32, 32);

		var title = new TextView(this) { Text = "HPKE Encryption (API 37)", TextSize = 22f };
		var subtitle = new TextView(this)
		{
			Text = OperatingSystem.IsAndroidVersionAtLeast(37)
				? "Device is 37+ \u2714 — using android.crypto.hpke APIs"
				: "Device is <37 \u274c — buttons are no-ops"
		};
		subtitle.SetPadding(0, 0, 0, 16);

		var inputLabel = new TextView(this) { Text = "Message to encrypt:" };
		_input = new EditText(this) { Hint = "Type a message here" };

		_btnSeal = new Button(this) { Text = "Seal (Encrypt)" };
		_btnSeal.Click += (_, _) => SealMessage();

		var ciphertextLabel = new TextView(this) { Text = "Ciphertext (Base64):" };
		ciphertextLabel.SetPadding(0, 16, 0, 0);
		_ciphertextView = new TextView(this) { Text = "(none yet)" };
		_ciphertextView.SetTextIsSelectable(true);

		_btnOpen = new Button(this) { Text = "Open (Decrypt)" };
		_btnOpen.Click += (_, _) => OpenMessage();

		var plaintextLabel = new TextView(this) { Text = "Decrypted plaintext:" };
		plaintextLabel.SetPadding(0, 16, 0, 0);
		_plaintextView = new TextView(this) { Text = "(none yet)" };

		_statusView = new TextView(this);
		_statusView.SetPadding(0, 16, 0, 0);

		root.AddView(title);
		root.AddView(subtitle);
		root.AddView(inputLabel);
		root.AddView(_input, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
		root.AddView(_btnSeal);
		root.AddView(ciphertextLabel);
		root.AddView(_ciphertextView);
		root.AddView(_btnOpen);
		root.AddView(plaintextLabel);
		root.AddView(_plaintextView);
		root.AddView(_statusView);
		SetContentView(root);

		InitializeHpke();
	}

	private void InitializeHpke()
	{
		if (!OperatingSystem.IsAndroidVersionAtLeast(37))
		{
			SetStatus("HPKE requires Android 37+", error: true);
			return;
		}

		// Generate an X25519 key pair
		var keyPairGenerator = Java.Security.KeyPairGenerator.GetInstance("X25519");
		_keyPair = keyPairGenerator!.GenerateKeyPair()!;

		// Build the HPKE suite name and get an instance
		string suiteName = Hpke.GetSuiteName(
			KemParameterSpec.DhkemX25519HkdfSha256!,
			KdfParameterSpec.HkdfSha256!,
			AeadParameterSpec.Aes128Gcm!)!;

		_hpke = Hpke.GetInstance(suiteName);

		SetStatus("HPKE initialized — X25519 + HKDF-SHA256 + AES-128-GCM");
	}

	private void SealMessage()
	{
		if (!OperatingSystem.IsAndroidVersionAtLeast(37))
		{
			SetStatus("37+ required for HPKE", error: true);
			return;
		}

		var userInput = _input?.Text;
		if (string.IsNullOrEmpty(userInput))
		{
			SetStatus("Enter a message first", error: true);
			return;
		}

		try
		{
			byte[] plaintext = System.Text.Encoding.UTF8.GetBytes(userInput);
			_sealedMessage = _hpke.Seal(_keyPair.Public!, Info, plaintext, null);

			// Display Base64-encoded ciphertext and encapsulated key
			var enc = Convert.ToBase64String(_sealedMessage!.GetEncapsulated()!);
			var ct = Convert.ToBase64String(_sealedMessage.GetCiphertext()!);
			_ciphertextView!.Text = $"enc: {enc}\nct: {ct}";

			_plaintextView!.Text = "(not yet decrypted)";
			SetStatus("Sealed successfully \u2714");
		}
		catch (Exception ex)
		{
			SetStatus($"Seal failed: {ex.Message}", error: true);
		}
	}

	private void OpenMessage()
	{
		if (!OperatingSystem.IsAndroidVersionAtLeast(37))
		{
			SetStatus("37+ required for HPKE", error: true);
			return;
		}

		if (_sealedMessage == null)
		{
			SetStatus("Seal a message first", error: true);
			return;
		}

		try
		{
			byte[] decrypted = _hpke.Open(_keyPair.Private!, Info, _sealedMessage, null)!;
			string result = System.Text.Encoding.UTF8.GetString(decrypted);
			_plaintextView!.Text = result;
			SetStatus("Opened successfully \u2714");
		}
		catch (Exception ex)
		{
			SetStatus($"Open failed: {ex.Message}", error: true);
		}
	}

	private void SetStatus(string message, bool error = false)
	{
		if (_statusView == null) return;
		_statusView.Text = message;
		_statusView.SetTextColor(error
			? Android.Graphics.Color.Red
			: Android.Graphics.Color.ParseColor("#4CAF50"));
	}
}