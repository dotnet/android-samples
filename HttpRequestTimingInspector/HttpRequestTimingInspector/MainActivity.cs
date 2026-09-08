using System.Runtime.Versioning;
using System.Text;
using Android.Graphics;
using Android.Net.Http;
using Android.Views;
using Java.Nio;
using Java.Time;
using Java.Util.Concurrent;
using Debug = System.Diagnostics.Debug;

namespace HttpRequestTimingInspector;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
	const string DefaultUrl = "https://www.example.com/";

	EditText? _urlInput;
	Button? _runOnceButton;
	Button? _runTwiceButton;
	TextView? _statusView;
	TextView? _resultsView;
	HttpEngine? _httpEngine;
	IExecutorService? _executor;
	UrlRequest? _activeRequest;
	bool _isDestroyed;

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		SetContentView(CreateContentView());

		if (!OperatingSystem.IsAndroidVersionAtLeast(37, 1))
		{
			SetStatus("Android API 37.1 is required.", isError: true);
			SetControlsEnabled(false);
			return;
		}

		InitializeNetworking();
	}

	View CreateContentView()
	{
		var padding = Dp(20);
		var content = new LinearLayout(this)
		{
			Orientation = Orientation.Vertical
		};
		content.SetPadding(padding, Dp(28), padding, padding);

		var title = CreateTextView("HTTP Request Timing Inspector", 24, Color.Rgb(28, 36, 48));
		title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);

		var description = CreateTextView(
			"Inspect DNS, connection, TLS, sending, and total HTTPS request time reported by Android API 37.1.",
			15,
			Color.Rgb(72, 82, 96));
		description.SetPadding(0, Dp(6), 0, Dp(20));

		var urlLabel = CreateTextView("Request URL", 14, Color.Rgb(50, 58, 70));
		urlLabel.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);

		_urlInput = new EditText(this)
		{
			InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationUri,
			Text = DefaultUrl
		};
		_urlInput.SetSingleLine();
		_urlInput.SetSelectAllOnFocus(true);

		var buttons = new LinearLayout(this)
		{
			Orientation = Orientation.Horizontal
		};
		buttons.SetPadding(0, Dp(10), 0, Dp(8));

		_runOnceButton = new Button(this) { Text = "Run once" };
		_runTwiceButton = new Button(this) { Text = "Run twice" };
		buttons.AddView(_runOnceButton, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1));
		buttons.AddView(_runTwiceButton, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1)
		{
			LeftMargin = Dp(8)
		});

		_statusView = CreateTextView(string.Empty, 14, Color.Rgb(43, 103, 79));
		_statusView.SetPadding(0, 0, 0, Dp(12));

		_resultsView = CreateTextView("Timing results will appear here.", 14, Color.Rgb(29, 38, 51));
		_resultsView.SetTypeface(Android.Graphics.Typeface.Monospace, Android.Graphics.TypefaceStyle.Normal);
		_resultsView.SetTextIsSelectable(true);
		_resultsView.SetPadding(Dp(14), Dp(14), Dp(14), Dp(14));
		_resultsView.SetBackgroundColor(Color.Rgb(244, 247, 250));

		content.AddView(title);
		content.AddView(description);
		content.AddView(urlLabel);
		content.AddView(_urlInput, MatchWidth());
		content.AddView(buttons, MatchWidth());
		content.AddView(_statusView, MatchWidth());
		content.AddView(_resultsView, MatchWidth());

		var scrollView = new ScrollView(this);
		scrollView.AddView(content);
		return scrollView;
	}

	[SupportedOSPlatform("android37.1")]
	void InitializeNetworking()
	{
		_httpEngine = new HttpEngine.Builder(this).Build();
		_executor = Executors.NewSingleThreadExecutor();

		Debug.Assert(_runOnceButton is not null);
		Debug.Assert(_runTwiceButton is not null);
		_runOnceButton.Click += async (_, _) => await RunRequestsAsync(1);
		_runTwiceButton.Click += async (_, _) => await RunRequestsAsync(2);
		SetStatus("Ready. Run twice to compare a fresh request with connection reuse.");
	}

	[SupportedOSPlatform("android37.1")]
	async Task RunRequestsAsync(int requestCount)
	{
		if (_httpEngine is null || _executor is null)
			return;

		var resultsView = _resultsView;
		Debug.Assert(resultsView is not null);

		var url = _urlInput?.Text?.Trim();
		if (!System.Uri.TryCreate(url, UriKind.Absolute, out var parsedUrl) ||
			parsedUrl.Scheme != Uri.UriSchemeHttps)
		{
			SetStatus("Enter an absolute HTTPS URL.", isError: true);
			return;
		}

		SetBusy(true);
		resultsView.Text = string.Empty;

		try
		{
			var hadError = false;
			for (var index = 1; index <= requestCount; index++)
			{
				SetStatus($"Running request {index} of {requestCount}...");
				var result = await SendRequestAsync(parsedUrl.AbsoluteUri);
				if (_isDestroyed)
					return;

				hadError |= result.ErrorMessage is not null;
				if (index > 1)
					resultsView.Append("\n");
				resultsView.Append(FormatResult(index, result));
			}

			SetStatus(
				hadError
					? "Completed with a request error. See the timing details below."
					: requestCount == 1
						? "Request complete."
						: "Both requests complete. Compare the socket reuse and phase timings.",
				isError: hadError);
		}
		catch (Exception exception)
		{
			if (!_isDestroyed)
				SetStatus($"Request could not be started: {exception.Message}", isError: true);
		}
		finally
		{
			if (_isDestroyed)
				ShutdownNetworking();
			else
				SetBusy(false);
		}
	}

	[SupportedOSPlatform("android37.1")]
	async Task<RequestResult> SendRequestAsync(string url)
	{
		Debug.Assert(_httpEngine is not null);
		Debug.Assert(_executor is not null);

		var callback = new RequestCallback();
		var request = _httpEngine.NewUrlRequestBuilder(url, _executor, callback)
			.SetHttpMethod("GET")
			.Build();

		_activeRequest = request;

		try
		{
			_executor.Execute(new Java.Lang.Runnable(() =>
			{
				try
				{
					request.Start();
				}
				catch (Exception exception)
				{
					callback.OnStartFailed(exception);
				}
			}));
			return await callback.Completion;
		}
		finally
		{
			_activeRequest = null;
			request.Dispose();
			callback.Dispose();
		}
	}

	[SupportedOSPlatform("android37.1")]
	static string FormatResult(int requestNumber, RequestResult result)
	{
		var output = new StringBuilder();
		output.AppendLine($"REQUEST {requestNumber}");
		output.AppendLine(new string('-', 34));

		if (result.Response is not null)
		{
			var protocol = result.Response.NegotiatedProtocol;
			output.AppendLine($"HTTP:       {result.Response.HttpStatusCode} {result.Response.HttpStatusText}");
			output.AppendLine($"Protocol:   {(string.IsNullOrEmpty(protocol) ? "(not reported)" : protocol)}");
			output.AppendLine($"From cache: {result.Response.WasCached()}");
		}

		if (result.ErrorMessage is not null)
			output.AppendLine($"Result:     {result.ErrorMessage}");

		if (result.Timings is null)
		{
			output.AppendLine("Timing data is unavailable.");
			return output.ToString();
		}

		var timings = result.Timings;
		output.AppendLine($"Socket reused: {timings.WasSocketReused()}");
		output.AppendLine();
		output.AppendLine("PHASE DURATIONS");
		output.AppendLine($"DNS:        {FormatDuration(timings.DnsStart, timings.DnsEnd)}");
		output.AppendLine($"Connection: {FormatDuration(timings.ConnectingStart, timings.ConnectingEnd)}");
		output.AppendLine($"TLS:        {FormatDuration(timings.TlsHandshakeStart, timings.TlsHandshakeEnd)}");
		output.AppendLine($"Sending:    {FormatDuration(timings.SendingStart, timings.SendingEnd)}");
		output.AppendLine($"Total:      {FormatDuration(timings.RequestStart, timings.RequestEnd)}");
		output.AppendLine();
		output.AppendLine($"Bytes sent:     {timings.SentByteCount:N0}");
		output.AppendLine($"Bytes received: {timings.ReceivedByteCount:N0}");
		return output.ToString();
	}

	[SupportedOSPlatform("android37.1")]
	static string FormatDuration(Instant? start, Instant? end)
	{
		if (start is null || end is null)
			return "not used";

		var duration = Duration.Between(start, end);
		Debug.Assert(duration is not null);
		return $"{duration.ToMillis()} ms";
	}

	void SetBusy(bool isBusy)
	{
		SetControlsEnabled(!isBusy);
	}

	void SetControlsEnabled(bool enabled)
	{
		Debug.Assert(_urlInput is not null);
		Debug.Assert(_runOnceButton is not null);
		Debug.Assert(_runTwiceButton is not null);
		_urlInput.Enabled = enabled;
		_runOnceButton.Enabled = enabled;
		_runTwiceButton.Enabled = enabled;
	}

	void SetStatus(string message, bool isError = false)
	{
		Debug.Assert(_statusView is not null);
		_statusView.Text = message;
		_statusView.SetTextColor(isError
			? Color.Rgb(176, 42, 55)
			: Color.Rgb(43, 103, 79));
	}

	TextView CreateTextView(string text, float textSize, Color color)
	{
		var view = new TextView(this)
		{
			Text = text,
			TextSize = textSize
		};
		view.SetTextColor(color);
		return view;
	}

	static LinearLayout.LayoutParams MatchWidth() =>
		new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

	int Dp(int value)
	{
		var resources = Resources;
		Debug.Assert(resources is not null);
		var displayMetrics = resources.DisplayMetrics;
		Debug.Assert(displayMetrics is not null);
		return (int)(value * displayMetrics.Density + 0.5f);
	}

	protected override void OnDestroy()
	{
		_isDestroyed = true;
		if (OperatingSystem.IsAndroidVersionAtLeast(34) &&
			_activeRequest is not null &&
			_executor is not null)
		{
			var request = _activeRequest;
			_executor.Execute(new Java.Lang.Runnable(request.Cancel));
		}

		if (_activeRequest is null)
			ShutdownNetworking();

		base.OnDestroy();
	}

	void ShutdownNetworking()
	{
		if (OperatingSystem.IsAndroidVersionAtLeast(34))
			_httpEngine?.Shutdown();
		_httpEngine?.Dispose();
		_httpEngine = null;

		_executor?.Shutdown();
		_executor = null;
	}

	[SupportedOSPlatform("android37.1")]
	sealed class RequestCallback : Java.Lang.Object, UrlRequest.ICallback
	{
		const int BufferSize = 32 * 1024;
		readonly TaskCompletionSource<RequestResult> _completion =
			new(TaskCreationOptions.RunContinuationsAsynchronously);

		public Task<RequestResult> Completion => _completion.Task;

		public void OnRedirectReceived(UrlRequest request, UrlResponseInfo info, string newLocationUrl) =>
			request.FollowRedirect();

		public void OnResponseStarted(UrlRequest request, UrlResponseInfo info) =>
			request.Read(ByteBuffer.AllocateDirect(BufferSize));

		public void OnReadCompleted(UrlRequest request, UrlResponseInfo info, ByteBuffer byteBuffer)
		{
			byteBuffer.Clear();
			request.Read(byteBuffer);
		}

		public void OnSucceeded(UrlRequest request, UrlResponseInfo info) =>
			Complete(request, info, errorMessage: null);

		public void OnFailed(UrlRequest request, UrlResponseInfo? info, HttpException error) =>
			Complete(request, info, $"Failed: {error.Message}");

		public void OnCanceled(UrlRequest request, UrlResponseInfo? info) =>
			Complete(request, info, "Canceled");

		public void OnStartFailed(Exception exception) =>
			_completion.TrySetResult(new RequestResult(
				Response: null,
				Timings: null,
				ErrorMessage: $"Failed to start: {exception.Message}"));

		void Complete(UrlRequest request, UrlResponseInfo? info, string? errorMessage)
		{
			_completion.TrySetResult(new RequestResult(
				info,
				request.FinishedRequestTimings,
				errorMessage));
		}
	}

	sealed record RequestResult(
		UrlResponseInfo? Response,
		FinishedRequestTimings? Timings,
		string? ErrorMessage);
}
