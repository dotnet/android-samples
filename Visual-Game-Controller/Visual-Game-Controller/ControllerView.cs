
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using VisualGameController;

namespace VisualGameController
{
	public class ControllerView : SurfaceView,ISurfaceHolderCallback
	{

		private const String TAG = "ControllerView";
		private const float IMAGE_RESOLUTION_HEIGHT = 1080.0F;
		private const int MAX_CONTROLLERS = 4;
		private IWindowManager window_manager;

		//bitmaps for different sections of the controller
		private Bitmap controller_bitmap;
		private Bitmap axis_bitmap;
		private Bitmap blue_led_bitmap;
		private Bitmap right_directional_bitmap;
		private Bitmap top_directional_bitmap;
		private Bitmap left_directional_bitmap;
		private Bitmap bottom_directional_bitmap;
		private Bitmap right_paddle_bitmap;
		private Bitmap left_paddle_bitmap;
		private Bitmap gradient_bitmap;

		//paints for the view
		private Paint background_paint;
		private Paint image_paint;
		private Paint circle_paint;
		private Paint led_paint;
		private Paint directional_paint;
		private Paint gradient_paint;
		private Point size = new Point();
		private float display_ratio = 1.0f;

		//arrays to keep track of the buttons and axes
		private int[] buttons;
		private float[] axes;

		//the current controller number
		private int current_controller_number = -1;

		//image asset locations
		private float[] y_button = {
			823 / IMAGE_RESOLUTION_HEIGHT, 276 / IMAGE_RESOLUTION_HEIGHT, 
			34 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] x_button = {
			744 / IMAGE_RESOLUTION_HEIGHT, 355 / IMAGE_RESOLUTION_HEIGHT, 
			34 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] b_button = {
			903 / IMAGE_RESOLUTION_HEIGHT, 355 / IMAGE_RESOLUTION_HEIGHT, 
			34 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] a_button = {
			823 / IMAGE_RESOLUTION_HEIGHT, 434 / IMAGE_RESOLUTION_HEIGHT, 
			34 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] power_button = {
			533 / IMAGE_RESOLUTION_HEIGHT, 353 / IMAGE_RESOLUTION_HEIGHT, 
			50 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] home_button = {
			624 / IMAGE_RESOLUTION_HEIGHT, 353 / IMAGE_RESOLUTION_HEIGHT, 
			30 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] back_button = {
			443 / IMAGE_RESOLUTION_HEIGHT, 353 / IMAGE_RESOLUTION_HEIGHT,
			30 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] led_buttons = {
			463 / IMAGE_RESOLUTION_HEIGHT, 449 / IMAGE_RESOLUTION_HEIGHT,
			502 / IMAGE_RESOLUTION_HEIGHT, 449 / IMAGE_RESOLUTION_HEIGHT,
			539 / IMAGE_RESOLUTION_HEIGHT,
			449 / IMAGE_RESOLUTION_HEIGHT, 574 / IMAGE_RESOLUTION_HEIGHT,
			449 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_directional_button = {
			264 / IMAGE_RESOLUTION_HEIGHT, 336 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] top_directional_button = {
			218 / IMAGE_RESOLUTION_HEIGHT, 263 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_directional_button = {
			144 / IMAGE_RESOLUTION_HEIGHT, 337 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] bottom_directional_button = {
			217 / IMAGE_RESOLUTION_HEIGHT, 384 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_axis = {
			305 / IMAGE_RESOLUTION_HEIGHT, 485 / IMAGE_RESOLUTION_HEIGHT,
			63 / IMAGE_RESOLUTION_HEIGHT, 50 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_axis = {
			637 / IMAGE_RESOLUTION_HEIGHT, 485 / IMAGE_RESOLUTION_HEIGHT,
			63 / IMAGE_RESOLUTION_HEIGHT, 50 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_paddle = {
			705 / IMAGE_RESOLUTION_HEIGHT, 166 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_paddle_pressed = {
			705 / IMAGE_RESOLUTION_HEIGHT, 180 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_paddle = {
			135 / IMAGE_RESOLUTION_HEIGHT, 166 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_paddle_pressed = {
			135 / IMAGE_RESOLUTION_HEIGHT, 180 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_axis_button = {
			368 / IMAGE_RESOLUTION_HEIGHT, 548 / IMAGE_RESOLUTION_HEIGHT,
			64 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_axis_button = {
			700 / IMAGE_RESOLUTION_HEIGHT, 548 / IMAGE_RESOLUTION_HEIGHT,
			64 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] right_gradient = {
			705 / IMAGE_RESOLUTION_HEIGHT, 125 / IMAGE_RESOLUTION_HEIGHT
		};
		private float[] left_gradient = {
			125 / IMAGE_RESOLUTION_HEIGHT, 125 / IMAGE_RESOLUTION_HEIGHT
		};
		private float axis_leftX, axis_leftY;
		private float axis_rightX, axis_rightY;


		public ControllerView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public ControllerView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public ControllerView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			window_manager = (Context.GetSystemService (Context.WindowService)).JavaCast<IWindowManager> ();

			//set background paint properties
			background_paint = new Paint ();
			background_paint.SetStyle (Paint.Style.Fill);
			background_paint.Dither = true;
			background_paint.AntiAlias = true;

			image_paint = new Paint ();

			//set properties for the other paints
			circle_paint = new Paint ();
			circle_paint.SetStyle (Paint.Style.Fill);
			circle_paint.Dither = true;
			circle_paint.AntiAlias = true;

			led_paint = new Paint ();
			led_paint.SetStyle (Paint.Style.Fill);
			led_paint.Dither = true;
			led_paint.AntiAlias = true;
			BlurMaskFilter blur_mask_filter = new BlurMaskFilter (20.0f, BlurMaskFilter.Blur.Outer);
			led_paint.SetMaskFilter (blur_mask_filter);

			directional_paint = new Paint ();
			directional_paint.Dither = true;
			directional_paint.AntiAlias = true;
			directional_paint.Alpha = 204;

			gradient_paint = new Paint ();
			gradient_paint.Dither = true;
			gradient_paint.AntiAlias = true;
			gradient_paint.Alpha = 204;
		}

		private void LoadBitmaps(int displayWidth, int displayHeight)
		{
			//set each bitmap to the corresponding resource
			controller_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.game_controller_paddles);
			int controller_bitmap_width = controller_bitmap.Width;
			int controller_bitmap_height = controller_bitmap.Height;
			axis_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.axis);
			blue_led_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.led_blue);
			right_directional_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.directional_right);
			top_directional_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.directional_top);
			bottom_directional_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.directional_bottom);
			left_directional_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.directional_left);
			right_paddle_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.right_paddle);
			left_paddle_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.left_paddle);
			gradient_bitmap = BitmapFactory.DecodeResource (Resources, Resource.Drawable.gradient);

			controller_bitmap = Bitmap.CreateScaledBitmap (controller_bitmap, displayHeight, displayHeight, true);

			//scale the bitmaps based on the display ratio
			display_ratio = displayHeight * 1.0f / controller_bitmap_height;
			axis_bitmap = Bitmap.CreateScaledBitmap (axis_bitmap,
				(int)(axis_bitmap.Width * display_ratio),
				(int)(axis_bitmap.Height * display_ratio),
				true);
			blue_led_bitmap = Bitmap.CreateScaledBitmap (blue_led_bitmap,
				(int)(blue_led_bitmap.Width * display_ratio),
				(int)(blue_led_bitmap.Height * display_ratio),
				true);
			right_directional_bitmap = Bitmap.CreateScaledBitmap (right_directional_bitmap,
				(int)(right_directional_bitmap.Width * display_ratio),
				(int)(right_directional_bitmap.Height * display_ratio),
				true);
			top_directional_bitmap = Bitmap.CreateScaledBitmap (top_directional_bitmap,
				(int)(top_directional_bitmap.Width * display_ratio),
				(int)(top_directional_bitmap.Height * display_ratio),
				true);
			left_directional_bitmap = Bitmap.CreateScaledBitmap (left_directional_bitmap,
				(int)(left_directional_bitmap.Width * display_ratio),
				(int)(left_directional_bitmap.Height * display_ratio),
				true);
			bottom_directional_bitmap = Bitmap.CreateScaledBitmap (bottom_directional_bitmap,
				(int)(bottom_directional_bitmap.Width * display_ratio),
				(int)(bottom_directional_bitmap.Height * display_ratio),
				true);
			right_paddle_bitmap = Bitmap.CreateScaledBitmap (right_paddle_bitmap,
				(int)(right_paddle_bitmap.Width * display_ratio),
				(int)(right_paddle_bitmap.Height * display_ratio),
				true);
			left_paddle_bitmap = Bitmap.CreateScaledBitmap (left_paddle_bitmap,
				(int)(left_paddle_bitmap.Width * display_ratio),
				(int)(left_paddle_bitmap.Height * display_ratio),
				true);
			gradient_bitmap=Bitmap.CreateScaledBitmap(gradient_bitmap,
				(int)(gradient_bitmap.Width*display_ratio),
				(int)(gradient_bitmap.Height*display_ratio),
				true);

		}

		public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
		{
		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{

		}

		protected override void OnMeasure(int WidthMeasureSpec, int HeightMeasureSpec)
		{
			int width = 0;
			int height = 0;

			Display display = window_manager.DefaultDisplay;
			display.GetSize (size);
			int display_width = size.X;
			int display_height = size.Y;
			display_width = Width;
			display_height = Height;

			int width_spec_mode = (int)MeasureSpec.GetMode (WidthMeasureSpec);
			int width_spec_size = MeasureSpec.GetSize (WidthMeasureSpec);
			int height_spec_mode = (int)MeasureSpec.GetMode (HeightMeasureSpec);
			int height_spec_size = MeasureSpec.GetSize (HeightMeasureSpec);
			Log.Debug (TAG, "width_spec_size=" + width_spec_size + ", height_spec_size=" + height_spec_size);

			if (width_spec_mode == (int)MeasureSpecMode.Exactly) {
				width = width_spec_size;
			} else if (width_spec_mode == (int)MeasureSpecMode.AtMost) {
				width = Math.Min (display_width, width_spec_size);
			} else {
				width = display_width;
			}

			if (height_spec_mode == (int)MeasureSpecMode.Exactly) {
				height = height_spec_size;
			} else if (height_spec_mode == (int)MeasureSpecMode.AtMost) {
				height = Math.Min (display_height, height_spec_size);
			} else {
				height = display_height;
			}

			SetMeasuredDimension (width, height);
			if (width > 0 && height > 0) {
				LoadBitmaps (width, height);
			}
		}


		protected override void OnDraw (Canvas canvas)
		{
			int offset = Width / 2 - Height / 2;

			//draw background
			canvas.DrawColor (Color.Black);
			canvas.DrawRect (0, 0, Width, Height, background_paint);

			//draw brake/gas indicators
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_BRAKE)] > 0.0f) {
				gradient_paint.Alpha = ((int)(axes[AxesMapping.OrdinalValue(AxesMapping.AXIS_BRAKE)]*100)+155);
				canvas.DrawBitmap (gradient_bitmap, offset + left_gradient [0] * Height,
					left_gradient [1] * Height, gradient_paint);
			}

			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_GAS)] > 0.0f) {
				gradient_paint.Alpha = ((int)(axes[AxesMapping.OrdinalValue(AxesMapping.AXIS_GAS)]*100)+155);
				canvas.DrawBitmap (gradient_bitmap, offset + right_gradient [0] * Height,
					right_gradient [1] * Height, gradient_paint);
			}

			//draw the paddles
			canvas.DrawColor (Color.Transparent);
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_R1)] == 0) {
				canvas.DrawBitmap (right_paddle_bitmap, offset + right_paddle [0] * Height,
					right_paddle [1] * Height, image_paint);
			} else if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_R1)] == 1) {
				canvas.DrawBitmap (right_paddle_bitmap, offset+ right_paddle_pressed [0] * Height,
					right_paddle_pressed [1] * Height, image_paint);
			}
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_L1)] == 0) {
				canvas.DrawBitmap (left_paddle_bitmap, offset + left_paddle [0] * Height,
					left_paddle [1] * Height, image_paint);
			} else if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_L1)] == 1) {
				canvas.DrawBitmap (left_paddle_bitmap, offset + left_paddle_pressed [0] * Height,
					left_paddle_pressed [1] * Height, image_paint);
			}

			//draw the controller body
			canvas.DrawBitmap (controller_bitmap, offset, 0, image_paint);

			//draw axes
			axis_leftX = offset + left_axis [0] * Height;
			axis_leftY = left_axis [1] * Height;
			axis_rightX = offset + right_axis [0] * Height;
			axis_rightY = right_axis [1] * Height;
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_X)] != 0.0f) {
				axis_leftX = axis_leftX + left_axis [3] * Height 
					* axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_X)];
			}
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_Y)] != 0.0f) {
				axis_leftY = axis_leftY + left_axis [3] * Height 
					* axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_Y)];
			}
				
			//Android Gamepad:AXIS_Z and AXIS_RZ respectively
			//Xbox Gamepad:AXIS_RX and AXIX_RZ respectively
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_RX)] != 0.0f) {
				axis_rightX = axis_rightX + right_axis [3] * Height 
					* axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_RX)];
			}
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_RY)] != 0.0f) {
				axis_rightY = axis_rightY + right_axis [3] * Height 
					* axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_RY)];
			}
				
			canvas.DrawBitmap (axis_bitmap, axis_rightX, axis_rightY, image_paint);
			canvas.DrawBitmap (axis_bitmap, axis_leftX, axis_leftY, image_paint);

			//Draw the LED light
			if (current_controller_number > 0 && current_controller_number <= MAX_CONTROLLERS) {
				canvas.DrawBitmap (blue_led_bitmap, offset
				+ led_buttons [2 * current_controller_number - 2] * Height,
					led_buttons [2 * current_controller_number - 1] * Height, led_paint);
			}

			//Draw the directional buttons
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_HAT_X)] == 1.0f) {
				canvas.DrawBitmap (right_directional_bitmap, offset + right_directional_button [0]
				* Height,right_directional_button [1] * Height, directional_paint);
			}
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_HAT_Y)] == -1.0f) {
				canvas.DrawBitmap (top_directional_bitmap, offset + top_directional_button [0]
					* Height,top_directional_button [1] * Height, directional_paint);
			}
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_HAT_X)] == -1.0f) {
				canvas.DrawBitmap (left_directional_bitmap, offset + left_directional_button [0]
					* Height,left_directional_button [1] * Height, directional_paint);
			}
			if (axes [AxesMapping.OrdinalValue (AxesMapping.AXIS_HAT_Y)] == 1.0f) {
				canvas.DrawBitmap (bottom_directional_bitmap, offset + bottom_directional_button [0]
					* Height,bottom_directional_button [1] * Height, directional_paint);
			}

			//draw the A/B/X/Y buttons
			canvas.DrawColor (Color.Transparent);
			circle_paint.Color = Resources.GetColor (Resource.Color.transparent_black);
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_Y)] == 1) {
				canvas.DrawCircle (offset + y_button [0] * Height, y_button [1] * Height,
					y_button [2] * Height, circle_paint);
			}
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_X)] == 1) {
				canvas.DrawCircle (offset + x_button [0] * Height, x_button [1] * Height,
					x_button [2] * Height, circle_paint);
			}
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_B)] == 1) {
				canvas.DrawCircle (offset + b_button [0] * Height, b_button [1] * Height,
					b_button [2] * Height, circle_paint);
			}
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_A)] == 1) {
				canvas.DrawCircle (offset + a_button [0] * Height, a_button [1] * Height,
					a_button [2] * Height, circle_paint);
			}

			//draw the center buttons
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.POWER)] == 1) {
				canvas.DrawCircle (offset + power_button [0] * Height,
					power_button [1] * Height,
					power_button [2] * Height, circle_paint);
			}
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_START)] == 1) {
				canvas.DrawCircle (offset + home_button [0] * Height,
					home_button [1] * Height,
					home_button [2] * Height, circle_paint);
			}

			//Android Gamepad: BUTTON_BACK, Xbox Gamepad: BUTTON_SELECT
			if (buttons [ButtonMapping.OrdinalValue (ButtonMapping.BUTTON_SELECT)] == 1) {
				canvas.DrawCircle (offset + back_button [0] * Height,
					back_button [1] * Height,
					back_button [2] * Height, circle_paint);
			}

			// Draw the axes
			if (buttons[ButtonMapping.OrdinalValue(ButtonMapping.BUTTON_THUMBL)] == 1) {
				canvas.DrawCircle(left_axis_button[2] * Height + axis_leftX, left_axis_button[2]
					* Height + axis_leftY,
					left_axis_button[2] * Height, circle_paint);
			}
			if (buttons[ButtonMapping.OrdinalValue(ButtonMapping.BUTTON_THUMBR)] == 1) {
				canvas.DrawCircle(right_axis_button[2] * Height + axis_rightX, right_axis_button[2]
					* Height + axis_rightY,
					right_axis_button[2] * Height, circle_paint);
			}

		}
		public void SetButtonAxes(int[] buttons, float[] axes) 
		{
			this.buttons = buttons;
			this.axes = axes;
		}

		public void SetCurrentControllerNumber(int number) 
		{
			Log.Debug (TAG, "SetCurrentControllerNumber: " + number);
			current_controller_number = number;
		}
			
	}
}

