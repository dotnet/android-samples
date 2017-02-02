using System;
using System.Threading;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Text.Format;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.Wearable.Watchface;
using Android.Service.Wallpaper;
using Java.Util.Concurrent;

namespace WatchFace
{
    // WatchFaceService is a subclass of WallpaperService. It implements only one
    // method, OnCreateEngine, and it defines a nested class that is derived from
    // CanvasWatchFaceService.Engine.

    public class WatchFaceService : CanvasWatchFaceService
    {
        // Used for logging:
        const String Tag = "WatchFaceService";

        public WatchFaceService ()
        {
        }

        // Must be implemented to return a new instance of the wallpaper's engine:

        public override WallpaperService.Engine OnCreateEngine () 
        {
            return new WatchFaceEngine (this);
        }

        // Class for a watch face that draws on a Canvas:

        public class WatchFaceEngine : CanvasWatchFaceService.Engine 
        {
            // Update every second:
            static long InterActiveUpdateRateMs = TimeUnit.Seconds.ToMillis (1);

            // Reference to the CanvasWatchFaceService that instantiates this engine:
            CanvasWatchFaceService owner;

            // For painting the hands of the watch:
            Paint hourPaint;
            Paint minutePaint;
            Paint secondPaint;

            // For painting the tick marks around the edge of the clock face:
            Paint tickPaint;

            // The current time:
            public Time time;
            Timer timerSeconds;

            // Broadcast receiver for handling time zone changes:
            TimeZoneReceiver timeZoneReceiver;

            // Whether the display supports fewer bits for each color in ambient mode. 
            // When true, we disable anti-aliasing in ambient mode:
            bool lowBitAmbient;

            // Bitmaps for drawing the watch face background:
            Bitmap backgroundBitmap;
            Bitmap backgroundScaledBitmap;

            // Saves a reference to the outer CanvasWatchFaceService
            public WatchFaceEngine (CanvasWatchFaceService owner) : base(owner)
            {
                this.owner = owner;
            }

            // Called when the engine is created for the first time: 
            public override void OnCreate (ISurfaceHolder holder) 
            {
                base.OnCreate(holder);

                // Configure the system UI. Instantiates a WatchFaceStyle object that causes 
                // notifications to appear as small peek cards that are shown only briefly 
                // when interruptive. Also disables the system-style UI time from being drawn:

                SetWatchFaceStyle (new WatchFaceStyle.Builder (owner)
                    .SetCardPeekMode (WatchFaceStyle.PeekModeShort)
                    .SetBackgroundVisibility (WatchFaceStyle.BackgroundVisibilityInterruptive)
                    .SetShowSystemUiTime (false)
                    .Build ());

                // Configure the background image:
                var backgroundDrawable = 
                    Application.Context.Resources.GetDrawable (Resource.Drawable.xamarin_background);
                backgroundBitmap = (backgroundDrawable as BitmapDrawable).Bitmap;

                // Initialize paint objects for drawing the clock hands and tick marks:

                // Hour hand:
                hourPaint = new Paint();
                hourPaint.SetARGB(255, 200, 200, 200);
                hourPaint.StrokeWidth = 5.0f;
                hourPaint.AntiAlias = true;
                hourPaint.StrokeCap = Paint.Cap.Round;

                // Minute hand:
                minutePaint = new Paint();
                minutePaint.SetARGB(255, 200, 200, 200);
                minutePaint.StrokeWidth = 3.0f;
                minutePaint.AntiAlias = true;
                minutePaint.StrokeCap = Paint.Cap.Round;

                // Seconds hand:
                secondPaint = new Paint();
                secondPaint.SetARGB(255, 255, 0, 0);
                secondPaint.StrokeWidth = 2.0f;
                secondPaint.AntiAlias = true;
                secondPaint.StrokeCap = Paint.Cap.Round;

                // Tick marks around the edge of the face:
                tickPaint = new Paint();
                tickPaint.SetARGB(100, 255, 255, 255);
                tickPaint.StrokeWidth = 2.0f;
                tickPaint.AntiAlias = true;

                // Instantiate the time object:
                time = new Time ();

                // How to stop the timer? It shouldn't run in ambient mode...
                timerSeconds = new Timer (new TimerCallback (state => {
                    Invalidate ();
                }), null, 
                    TimeSpan.FromMilliseconds (InterActiveUpdateRateMs), 
                    TimeSpan.FromMilliseconds (InterActiveUpdateRateMs));
            }

            // Called when the properties of the Wear device are determined, specifically 
            // low bit ambient mode (the screen supports fewer bits for each color in
            // ambient mode)::

            public override void OnPropertiesChanged(Bundle properties) 
            {
                base.OnPropertiesChanged (properties);

                lowBitAmbient = properties.GetBoolean (WatchFaceService.PropertyLowBitAmbient);

                if (Log.IsLoggable (Tag, LogPriority.Debug))
                    Log.Debug (Tag, "OnPropertiesChanged: low-bit ambient = " + lowBitAmbient);
            }

            // Called periodically to update the time shown by the watch face: at least 
            // once per minute in ambient and interactive modes, and whenever the date, 
            // time, or timezone has changed.

            public override void OnTimeTick ()
            {
                base.OnTimeTick ();

                if (Log.IsLoggable (Tag, LogPriority.Debug))
                    Log.Debug (Tag, "onTimeTick: ambient = " + IsInAmbientMode);
                
                Invalidate ();
            }

            // Called when the device enters or exits ambient mode. In ambient mode,
            // the watch face disables anti-aliasing while drawing.

            public override void OnAmbientModeChanged (bool inAmbientMode) 
            {
                base.OnAmbientModeChanged (inAmbientMode);

                if (Log.IsLoggable (Tag, LogPriority.Debug))
                    Log.Debug (Tag, "OnAmbientMode");
                
                if (lowBitAmbient) {
                    bool antiAlias = !inAmbientMode;
                    hourPaint.AntiAlias = antiAlias;
                    minutePaint.AntiAlias = antiAlias;
                    secondPaint.AntiAlias = antiAlias;
                    tickPaint.AntiAlias = antiAlias;
                }
                Invalidate ();
            }

            // Called to draw the watch face:

            public override void OnDraw (Canvas canvas, Rect bounds)
            {
                // Get the current time:
                time.SetToNow ();

                // Determine the bounds of the drawing surface:
                int width = bounds.Width ();
                int height = bounds.Height ();

                // Draw the background, scaled to fit:
                if (backgroundScaledBitmap == null
                    || backgroundScaledBitmap.Width != width
                    || backgroundScaledBitmap.Height != height) {
                    backgroundScaledBitmap = Bitmap.CreateScaledBitmap(backgroundBitmap,
                        width, height, true /* filter */);
                }
                canvas.DrawColor (Color.Black);
                canvas.DrawBitmap(backgroundScaledBitmap, 0, 0, null);

                // Determine the center of the drawing surface:
                float centerX = width / 2.0f;
                float centerY = height / 2.0f;

                // Draw the ticks:
                float innerTickRadius = centerX - 10;
                float outerTickRadius = centerX;
                for (int tickIndex = 0; tickIndex < 12; tickIndex++) {
                    float tickRot = (float) (tickIndex * Math.PI * 2 / 12);
                    float innerX = (float) Math.Sin(tickRot) * innerTickRadius;
                    float innerY = (float) -Math.Cos(tickRot) * innerTickRadius;
                    float outerX = (float) Math.Sin(tickRot) * outerTickRadius;
                    float outerY = (float) -Math.Cos(tickRot) * outerTickRadius;
                    canvas.DrawLine(centerX + innerX, centerY + innerY,
                        centerX + outerX, centerY + outerY, tickPaint);
                }

                // Calculate the angle of rotation and length for each hand:
                float secRot = time.Second / 30f * (float) Math.PI;
                int minutes = time.Minute;
                float minRot = minutes / 30f * (float) Math.PI;
                float hrRot = ((time.Hour + (minutes / 60f)) / 6f ) * (float) Math.PI;

                float secLength = centerX - 20;
                float minLength = centerX - 40;
                float hrLength = centerX - 80;

                // Draw the second hand only in interactive mode:
                if (!IsInAmbientMode) {
                    float secX = (float) Math.Sin(secRot) * secLength;
                    float secY = (float) -Math.Cos(secRot) * secLength;
                    canvas.DrawLine(centerX, centerY, centerX + secX, centerY + secY, secondPaint);
                }

                // Draw the minute hand:
                float minX = (float) Math.Sin(minRot) * minLength;
                float minY = (float) -Math.Cos(minRot) * minLength;
                canvas.DrawLine(centerX, centerY, centerX + minX, centerY + minY, minutePaint);

                // Draw the hour hand:
                float hrX = (float) Math.Sin(hrRot) * hrLength;
                float hrY = (float) -Math.Cos(hrRot) * hrLength;
                canvas.DrawLine(centerX, centerY, centerX + hrX, centerY + hrY, hourPaint);
            }

            // Called whenever the watch face is becoming visible or hidden. Note that
            // you must call base.OnVisibilityChanged first:

            public override void OnVisibilityChanged (bool visible)
            {
                base.OnVisibilityChanged (visible);

                if (Log.IsLoggable (Tag, LogPriority.Debug))
                    Log.Debug (Tag, "OnVisibilityChanged: " + visible);
                
                // If the watch face became visible, register the timezone receiver
                // and get the current time. Else, unregister the timezone receiver:

                if (visible) {
                    RegisterTimezoneReceiver ();
                    time.Clear (Java.Util.TimeZone.Default.ID);
                    time.SetToNow ();
                } else {
                    UnregisterTimezoneReceiver ();
                }
            }

            // Run the timer only when visible and in interactive mode:
            bool ShouldTimerBeRunning() 
            {
                return IsVisible && !IsInAmbientMode;
            }

            bool registeredTimezoneReceiver = false;

            // Registers the time zone broadcast receiver (defined at the end of 
            // this file) to handle time zone change events:

            void RegisterTimezoneReceiver()
            {
                if (registeredTimezoneReceiver) {
                    return;
                } else  {
                    if (timeZoneReceiver == null) {
                        timeZoneReceiver = new TimeZoneReceiver ();
                        timeZoneReceiver.Receive = (intent) => {
                            time.Clear (intent.GetStringExtra ("time-zone"));
                            time.SetToNow ();
                        };
                    }
                    registeredTimezoneReceiver = true;
                    IntentFilter filter = new IntentFilter(Intent.ActionTimezoneChanged);
                    Application.Context.RegisterReceiver (timeZoneReceiver, filter);
                }
            }

            // Unregisters the timezone Broadcast receiver:

            void UnregisterTimezoneReceiver() 
            {
                if (!registeredTimezoneReceiver)
                    return;
                
                registeredTimezoneReceiver = false;
                Application.Context.UnregisterReceiver (timeZoneReceiver);
            }

        }

    }

    // Time zone broadcast receiver. OnReceive is called when the
    // time zone changes:

    public class TimeZoneReceiver: BroadcastReceiver 
    {
        public Action<Intent> Receive { get; set; }

        public override void OnReceive (Context context, Intent intent)
        {
            if (Receive != null)
                Receive (intent);
        }
    }
}
