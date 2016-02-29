using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;

using Java.Security.Cert;
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

		public ClientStatus CurrentStatus { get; set; }

		X509Certificate2 dotNetCertificate;
		X509Certificate2 DotNetCertificate {
			get {
				if (dotNetCertificate == null) {
					using (Stream cs = Assets.Open (certificateFileName)) {
						var bytes = cs.ReadAllBytes ();
						dotNetCertificate = new X509Certificate2 (bytes);
					}
				}

				return dotNetCertificate;
			}
		}

		Certificate nativeCertificate;
		Certificate NativeCertificate {
			get {
				if (nativeCertificate == null) {
					var cf = CertificateFactory.GetInstance ("X.509");
					using (Stream cs = Assets.Open (certificateFileName))
						nativeCertificate = cf.GenerateCertificate (cs);
				}

				return nativeCertificate;
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			var netRequestButton = FindViewById<Button> (Resource.Id.NetRequestButton);
			netRequestButton.Click += async delegate {
				netRequestButton.Enabled = !netRequestButton.Enabled;

				WebResponse msg = await RunRequest ();
				using (var stream = msg.GetResponseStream ())
					ProcessResponseStream (stream);

				netRequestButton.Enabled = !netRequestButton.Enabled;
			};

			var nativeRequestButton = FindViewById<Button> (Resource.Id.NativeRequestButton);
			nativeRequestButton.Click += async delegate {
				nativeRequestButton.Enabled = !nativeRequestButton.Enabled;

				AndroidHttpResponseMessage msg = await RunNativeRequest ();
				using (var stream = await msg.Content.ReadAsStreamAsync ())
					ProcessResponseStream (stream);

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
		}

		async Task<WebResponse> RunRequest ()
		{
			return await Task.Factory.StartNew (() => {

				var actual = ServicePointManager.SecurityProtocol;
				WebResponse msg = null;

				try {
					// NOTE: diagnostic will show TLS1.0 even if we enforce TLS1.2
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

					var request = new HttpWebRequest (new Uri (serverUrl));
					ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

					request.ClientCertificates.Add (DotNetCertificate);
					ShowMessage ("Executing request");
					msg = request.GetResponse ();
				} catch (Exception ex) {
					ShowMessage ("Web exception occurred");
					Console.WriteLine (ex.Message);
				} finally {
					ServicePointManager.SecurityProtocol = actual;
				}

				return msg;
			}).ConfigureAwait (false);
		}

		async Task<AndroidHttpResponseMessage> RunNativeRequest ()
		{
			return await Task.Factory.StartNew (() => {

				AndroidHttpResponseMessage msg = null;

				try {
					var handler = new AndroidClientHandler {
						UseCookies = true,
						AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
						PreAuthenticate = true,
						TrustedCerts = new List<Certificate> { NativeCertificate }
					};

					var httpClient = new HttpClient (handler);
					msg = httpClient.GetAsync (serverUrl)?.Result as AndroidHttpResponseMessage;
				} catch (Exception ex) {
					ShowMessage ("Web exception occurred");
					Console.WriteLine (ex.Message);
				}

				return msg;
			}).ConfigureAwait (false);
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
		}

		void ShowMessage (string message)
		{
			Snackbar.Make (layout, message, Snackbar.LengthLong).Show ();
		}
	}
}

