using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;

using Xamarin.Android.Net;
using Newtonsoft.Json;

namespace HowsMyTls {
	[Activity (Label = "How's my TLS?", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity {
		const string certificateFileName = "cert.pem";
		const string serverUrl = "https://howsmyssl.com:443/a/check";

		View layout;
		TextView tlsVersion;
		TextView splitting;
		TextView beastVuln;
		TextView cipherSuite;
		TextView tlsCompression;
		TextView ticketSupported;
		TextView ephemeralKeysSupported;
		TextView rating;

		public ClientStatus CurrentStatus { get; set; }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			var netRequestButton = FindViewById<Button> (Resource.Id.NetRequestButton);
			netRequestButton.Click += async delegate {
				netRequestButton.Enabled = !netRequestButton.Enabled;

				string json = await RunRequest ();
				CurrentStatus = JsonConvert.DeserializeObject<ClientStatus> (json);
				UpdateUI (CurrentStatus);

				netRequestButton.Enabled = !netRequestButton.Enabled;
			};

			var nativeRequestButton = FindViewById<Button> (Resource.Id.NativeRequestButton);
			nativeRequestButton.Click += async delegate {
				nativeRequestButton.Enabled = !nativeRequestButton.Enabled;

				string json = await RunNativeRequest ();
				CurrentStatus = JsonConvert.DeserializeObject<ClientStatus>(json);
				UpdateUI (CurrentStatus);

				nativeRequestButton.Enabled = !nativeRequestButton.Enabled;
			};

			layout = FindViewById (Resource.Id.sample_main_layout);
			tlsVersion = FindViewById<TextView> (Resource.Id.TlsVersion);
			splitting = FindViewById<TextView> (Resource.Id.Splitting);
			beastVuln = FindViewById<TextView> (Resource.Id.BeastVuln);
			cipherSuite = FindViewById<TextView> (Resource.Id.CipherSuite);
			tlsCompression = FindViewById<TextView> (Resource.Id.TlsCompression);
			ticketSupported = FindViewById<TextView> (Resource.Id.TicketSupported);
			ephemeralKeysSupported = FindViewById<TextView> (Resource.Id.EphemeralKeysSupported);
			rating = FindViewById<TextView> (Resource.Id.Rating);
		}

		async Task<string> RunRequest ()
		{
			var actual = ServicePointManager.SecurityProtocol;
			string msg = string.Empty;

			try {
				//// NOTE: diagnostic will show TLS1.0 for legacy provider even if we enforce TLS1.2
				using (var client = new HttpClient ()) {
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
					ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
					using (var response = await client.GetAsync (serverUrl)) {
						msg = await response.Content.ReadAsStringAsync ();
					}
				}
			} catch (Exception ex) {
				ShowMessage ("Web exception occurred");
				Console.WriteLine (ex.Message);
			} finally {
				ServicePointManager.SecurityProtocol = actual;
			}

			return msg;
		}

		async Task<string> RunNativeRequest ()
		{
			string msg = string.Empty;

			try {
				var handler = new AndroidClientHandler {
					UseCookies = true,
					AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
					PreAuthenticate = true
				};

				var httpClient = new HttpClient (handler);
				var responseMessage = await httpClient.GetAsync (serverUrl);
				msg = await ((AndroidHttpResponseMessage)responseMessage).Content.ReadAsStringAsync ();
			} catch (Exception ex) {
				ShowMessage ("Web exception occurred");
				Console.WriteLine (ex.Message);
			}

			return msg;
		}

		public void ProcessResponseStream (Stream stream)
		{
			if (stream == null)
				return;

			using (var reader = new StreamReader (stream)) {
				var json = reader.ReadToEnd ();
				CurrentStatus = JsonConvert.DeserializeObject<ClientStatus> (json);
				UpdateUI (CurrentStatus);
			}
		}

		void UpdateUI (ClientStatus status)
		{
			if (status == null)
				return;

			tlsVersion.Text = status.TlsVersion;
			splitting.Text = status.AbleToDetectNMinusOneSplitting.ToString ();
			beastVuln.Text = status.BeastVuln.ToString ();
			cipherSuite.Text = status.UnknownCipherSuiteSupported.ToString ();
			tlsCompression.Text = status.TlsCompressionSupported.ToString ();
			ticketSupported.Text = status.SessionTicketSupported.ToString ();
			ephemeralKeysSupported.Text = status.EphemeralKeysSupported.ToString ();
			rating.Text = status.Rating;

		}

		void ShowMessage (string message)
		{
			Snackbar.Make (layout, message, Snackbar.LengthLong).Show ();
		}
	}
}

