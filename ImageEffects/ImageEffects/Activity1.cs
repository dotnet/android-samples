using Android.App;
using Android.Graphics;
using Android.Media.Effect;
using Android.Opengl;
using Android.Views;
using Android.OS;
using Javax.Microedition.Khronos.Opengles;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;

//using Colour = Android.Graphics.Color;
using Android.Widget;

namespace ImageEffects
{
	[Activity (Label = "Image Effects Sample", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : Activity, GLSurfaceView.IRenderer
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// InitialiseRender();

			_effectView = FindViewById<GLSurfaceView> (Resource.Id.effectsview);
			_effectView.SetEGLContextClientVersion (2);
			_effectView.SetRenderer (this);
			_effectView.RenderMode = Rendermode.WhenDirty;
			CurrentEffect = Resource.Id.none;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.options_menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.tint || item.ItemId == Resource.Id.duotone) {
				CurrentEffect = Resource.Id.none;
			} else {
				CurrentEffect = item.ItemId;
			}

			ActionBar.Title = GetTitleForEffect (item);

			_effectView.RequestRender ();
			return true;
		}

		TextureRenderer Renderer {
			get { return _textureRenderer ?? (_textureRenderer = new TextureRenderer ()); }
		}

		void LoadTextures ()
		{
			// Generate textures
			GLES20.GlGenTextures (2, _textures, 0);

			// Load input bitmap
			var bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.Lenna);
			_imageWidth = bitmap.Width;
			_imageHeight = bitmap.Height;
			Renderer.UpdateTextureSize (_imageWidth, _imageHeight);

			// Upload to texture
			GLES20.GlBindTexture (GLES20.GlTexture2d, _textures [0]);
			GLUtils.TexImage2D (GLES20.GlTexture2d, 0, bitmap, 0);

			// Set texture parameters
			GLToolbox.InitTexParams ();
		}

		public void OnDrawFrame (IGL10 gl)
		{
			if (!_isInitialised) {
				_effectContext = EffectContext.CreateWithCurrentGlContext ();
				Renderer.Init ();
				LoadTextures ();

				_isInitialised = true;
			}

			if (CurrentEffect != Resource.Id.none) {
				InitialiseEffect ();
				ApplyEffect ();
			}

			RenderResult ();
		}

		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			if (Renderer != null) {
				Renderer.UpdateViewSize (width, height);
			}
		}

		public void OnSurfaceCreated (IGL10 gl, EGLConfig config)
		{
			// throw new NotImplementedException();
		}

		void RenderResult ()
		{
			// if no effect is chosen, just render the original bitmap
			Renderer.RenderTexture (CurrentEffect == Resource.Id.none ? _textures [0] : _textures [1]);
		}

		void ApplyEffect ()
		{
			_effect.Apply (_textures [0], _imageWidth, _imageHeight, _textures [1]);
		}

		void InitialiseEffect ()
		{
			EffectFactory effectFactory = _effectContext.Factory;
			if (_effect != null) {
				_effect.Release ();
			}

			switch (CurrentEffect) {
			case Resource.Id.none:
			case Resource.Id.duotone:
			case Resource.Id.tint:
				break;

			case Resource.Id.autofix:
				_effect = effectFactory.CreateEffect (EffectFactory.EffectAutofix);
				_effect.SetParameter ("scale", 0.5f);
				break;

			case Resource.Id.bw:
				_effect = effectFactory.CreateEffect (EffectFactory.EffectBlackwhite);
				_effect.SetParameter ("black", 0.1f);
				_effect.SetParameter ("white", 0.7f);
				break;

			case Resource.Id.brightness:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectBrightness);
				_effect.SetParameter ("brightness", 2.0f);
				break;

			case Resource.Id.contrast:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectContrast);
				_effect.SetParameter ("contrast", 1.4f);
				break;

			case Resource.Id.crossprocess:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectCrossprocess);
				break;

			case Resource.Id.documentary:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectDocumentary);
				break;

			//			case Resource.Id.duotone:
			//				_effect = effectFactory.CreateEffect (
			//					EffectFactory.EffectDuotone);
			//				_effect.SetParameter ("first_color", 4293982463u);
			//				_effect.SetParameter ("second_color", 4289309097u);
			//				break;

			case Resource.Id.filllight:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectFilllight);
				_effect.SetParameter ("strength", .8f);
				break;

			case Resource.Id.fisheye:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectFisheye);
				_effect.SetParameter ("scale", .5f);
				break;

			case Resource.Id.flipvert:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectFlip);
				_effect.SetParameter ("vertical", true);
				break;

			case Resource.Id.fliphor:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectFlip);
				_effect.SetParameter ("horizontal", true);
				break;

			case Resource.Id.grain:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectGrain);
				_effect.SetParameter ("strength", 1.0f);
				break;

			case Resource.Id.grayscale:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectGrayscale);
				break;

			case Resource.Id.lomoish:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectLomoish);
				break;

			case Resource.Id.negative:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectNegative);
				break;

			case Resource.Id.posterize:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectPosterize);
				break;

			case Resource.Id.rotate:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectRotate);
				_effect.SetParameter ("angle", 180);
				break;

			case Resource.Id.saturate:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectSaturate);
				_effect.SetParameter ("scale", .5f);
				break;

			case Resource.Id.sepia:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectSepia);
				break;

			case Resource.Id.sharpen:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectSharpen);
				break;

			case Resource.Id.temperature:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectTemperature);
				_effect.SetParameter ("scale", .9f);
				break;

			//			case Resource.Id.tint:
			//				_effect = effectFactory.CreateEffect (
			//					EffectFactory.EffectTint);
			//				_effect.SetParameter ("tint", Colour.Magenta); 
			//				break;

			case Resource.Id.vignette:
				_effect = effectFactory.CreateEffect (
					EffectFactory.EffectVignette);
				_effect.SetParameter ("scale", .5f);
				break;
			}
		}

		int CurrentEffect { get; set; }

		static string GetTitleForEffect (IMenuItem item)
		{
			string menuName = ((Java.Lang.String)item.TitleFormatted).ToString ();
			return string.Format ("Effect: {0}", menuName);
		}

		GLSurfaceView _effectView;
		bool _isInitialised;
		EffectContext _effectContext;
		TextureRenderer _textureRenderer;
		readonly int[] _textures = new int[2];
		int _imageWidth;
		int _imageHeight;
		Effect _effect;
	}
}

