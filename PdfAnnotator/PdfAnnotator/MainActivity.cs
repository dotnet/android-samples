using Android.Graphics;
using Android.Graphics.Pdf;
using Android.Graphics.Pdf.Component;
using Android.OS;
using Android.Views;

namespace PdfAnnotator
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
		private ImageView? _image;
		private Button? _btnAddHighlight, _btnList, _btnClear;
		private ParcelFileDescriptor? _fileDescriptor;
		private PdfRenderer? _renderer;
		private PdfRenderer.Page? _page;
		private Bitmap? _bitmap;

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var root = new LinearLayout(this) { Orientation = Orientation.Vertical };
			root.SetPadding(32, 64, 32, 32);

			var title = new TextView(this) { Text = "PDF Annotations (API 36.1)", TextSize = 22f };
			var subtitle = new TextView(this)
			{
				Text = OperatingSystem.IsAndroidVersionAtLeast(36, 1)
					? "Device is 36.1+ ? — using new PDF APIs"
					: "Device is <36.1 ? — buttons are no-ops"
			};

			_image = new ImageView(this);
			_btnAddHighlight = new Button(this) { Text = "Add yellow highlight" };
			_btnList = new Button(this) { Text = "List annotations" };
			_btnClear = new Button(this) { Text = "Remove all annotations" };

			_btnAddHighlight.Click += (_, __) => AddHighlightAtCenter();
			_btnList.Click += (_, __) => ListAnnotations();
			_btnClear.Click += (_, __) => RemoveAllAnnotations();

			root.AddView(title);
			root.AddView(subtitle);
			root.AddView(_image, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1f));
			root.AddView(_btnAddHighlight);
			root.AddView(_btnList);
			root.AddView(_btnClear);
			SetContentView(root);

			// Build a tiny one-page PDF and open it
			var pdfFile = CreateOnePagePdf();
			_fileDescriptor = ParcelFileDescriptor.Open(pdfFile, ParcelFileMode.ReadWrite);
			ArgumentNullException.ThrowIfNull(_fileDescriptor);
			_renderer = new PdfRenderer(_fileDescriptor);
			_page = _renderer.OpenPage(0);

			RenderPage();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_page?.Close();
			_renderer?.Close();
			_fileDescriptor?.Close();
			_bitmap?.Dispose();
		}

		private void AddHighlightAtCenter()
		{
			if (!OperatingSystem.IsAndroidVersionAtLeast(36, 1))
			{
				ShowToast("36.1+ required for annotations");
				return;
			}
			if (_page == null) return;

			var highlight = new HighlightAnnotation([new RectF(0f, 0f, _page.Width, _page.Height)]);

			// Semi-transparent yellow so content shows through
			var color = Color.Yellow;
			color.A = 128; // 50% transparent
			highlight.SetColor(color);

			// Add the annotation to this page (36.1+ API)
			var id = _page.AddPageAnnotation(highlight);

			ShowToast($"Added highlight id={id}");
			RenderPage(); // re-render so the highlight shows
		}

		private void ListAnnotations()
		{
			if (!OperatingSystem.IsAndroidVersionAtLeast(36, 1))
			{
				ShowToast("36.1+ required for annotations");
				return;
			}
			if (_page == null) return;

			// 36.1-only
			ShowToast($"Annotations: {_page.PageAnnotations?.Count ?? 0}");
		}

		private void RemoveAllAnnotations()
		{
			if (!OperatingSystem.IsAndroidVersionAtLeast(36, 1))
			{
				ShowToast("36.1+ required for annotations");
				return;
			}
			if (_page == null) return;

			var pairs = _page.PageAnnotations; // 36.1-only
			if (pairs == null || pairs.Count == 0)
			{
				ShowToast("No annotations to remove");
				return;
			}

			foreach (var pair in pairs)
			{
				if (pair.First is Java.Lang.Integer integer)
				{
					_page.RemovePageAnnotation(integer.IntValue());
				}
			}

			ShowToast("Removed all annotations");
			RenderPage();
		}

		private Java.IO.File CreateOnePagePdf()
		{
			var outFile = new Java.IO.File(CacheDir, "demo.pdf");
			using var pdf = new PdfDocument();
			var pageInfo = new PdfDocument.PageInfo.Builder(1200, 1600, 1).Create();
			using (var page = pdf.StartPage(pageInfo))
			{
				var c = page?.Canvas;
				ArgumentNullException.ThrowIfNull(c);
				c.DrawColor(Color.White);
				using var paint = new Paint { Color = Color.Rgb(33, 150, 243), TextSize = 48f, AntiAlias = true };
				c.DrawText("Hello PDF (tap buttons below)", 80, 200, paint);
				paint.Color = Color.Argb(30, 0, 0, 0);
				c.DrawRect(new RectF(80, 260, 1120, 360), paint);

				pdf.FinishPage(page);
			}
			using var fs = File.Create(outFile.AbsolutePath);
			pdf.WriteTo(fs);
			fs.Flush();
			return outFile;
		}

		private void RenderPage()
		{
			if (_page == null || _image == null) return;

			var builder = new RenderParams.Builder((int)RenderParamsRenderMode.ForDisplay);
			builder.SetRenderFlags((int)(RenderParamsRenderFlag.HighlightAnnotations | RenderParamsRenderFlag.TextAnnotations));

			_bitmap ??= Bitmap.CreateBitmap(_page.Width, _page.Height, Bitmap.Config.Argb8888!);
			_page.Render(_bitmap, null, null, builder.Build());
			_image.SetImageBitmap(_bitmap);
		}

		private void ShowToast(string message)
		{
			var toasty = Toast.MakeText(this, message, ToastLength.Short);
			ArgumentNullException.ThrowIfNull(toasty);
			toasty.Show();
		}
	}
}