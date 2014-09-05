using System;
using System.Diagnostics;
using System.IO;
using System.Json;
using System.Net;
using System.Text;

namespace RotationDemo
{
		public class TwitterAuthenticationResponse
		{
				public string TokenType { get; set; }

				public string AccessToken { get; set; }
		}

		public class Twitter
		{
				public const string OAuthConsumerKey = "onoOUsBbjVWc8msLyuRJQ";
				public const string OAuthConsumerSecret = "Vxh0zMFRAWp2LbkwfDNvUKU2dQhaVgFFv3M04gDKFE";
				public const string OAuthUrl = "https://api.twitter.com/oauth2/token";
				public const string TwitterUrl = "https://api.twitter.com/1.1/search/tweets.json?q=%40xamarin&show-user=true&count=40&result-type=recent";

				private string authHeaderFormat = "Basic {0}";
				private HttpWebRequest authRequest;

				public bool Authenticated { get; private set; }

				public TwitterAuthenticationResponse AuthenticationInfo { get; private set; }

				private string AuthHeader {
						get {
								byte[] authHeaderData = Encoding.UTF8.GetBytes (Uri.EscapeDataString (OAuthConsumerKey) + ":" +
								                        Uri.EscapeDataString (OAuthConsumerSecret));

								return string.Format (authHeaderFormat, Convert.ToBase64String (authHeaderData));
						}
				}

				private WebRequest AuthenticationRequest {
						get {
								if (authRequest == null) {
										authRequest = (HttpWebRequest)WebRequest.Create (OAuthUrl);
										authRequest.Headers.Add ("Authorization", AuthHeader);
										authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
										authRequest.Method = "POST";
										authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
										authRequest.Headers.Add ("Accept-Encoding", "gzip");
										authRequest.Timeout = 15000;
										authRequest.KeepAlive = true;

										using (var stream = authRequest.GetRequestStream ()) {
												byte[] content = ASCIIEncoding.ASCII.GetBytes ("grant_type=client_credentials");
												stream.Write (content, 0, content.Length);
										}
								}

								return authRequest;
						}
				}

				public Twitter ()
				{
						Authorize ();
				}

				public string GetTweets ()
				{
						var webClient = new WebClient ();
						Debug.WriteLine ("Get remote json data");

						webClient.Headers.Set ("Authorization", string.Format ("{0} {1}", AuthenticationInfo.TokenType,
								AuthenticationInfo.AccessToken));
						webClient.Encoding = System.Text.Encoding.UTF8;
						return webClient.DownloadString (new Uri (TwitterUrl));
				}

				private void Authorize ()
				{
						try {
								var authResponse = AuthenticationRequest.GetResponse ();
								using (var reader = new StreamReader (authResponse.GetResponseStream ())) {
										var authenticationInfo = ParseAuthenticationResponse (reader.ReadToEnd ());
										if (!string.IsNullOrEmpty (authenticationInfo.AccessToken) &&
										    !string.IsNullOrEmpty (authenticationInfo.TokenType)) {
												Authenticated = true;
												AuthenticationInfo = authenticationInfo;
										} else {
												Debug.WriteLine ("Twitter authentication error occured");
										}
								}
						} catch (Exception e) {
								Debug.WriteLine ("Twitter authentication error: {0}", e.Message);
						}
				}

				private TwitterAuthenticationResponse ParseAuthenticationResponse (string tokenInfo)
				{
						if (string.IsNullOrEmpty (tokenInfo))
								throw new ArgumentNullException ("Twitter authentication error, token can't be null or emty string");

						var value = JsonValue.Parse (tokenInfo);
						return new TwitterAuthenticationResponse () {
								TokenType = value ["token_type"],
								AccessToken = value ["access_token"]
						};
				}
		}
}

