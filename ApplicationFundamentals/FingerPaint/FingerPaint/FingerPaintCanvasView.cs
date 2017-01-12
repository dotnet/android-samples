using System;
using System.Collections.Generic;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics;


// TODO: Properties and methods for color, stroke-width, and Clear
// ===============================================================


namespace FingerPaint
{
    public class FingerPaintCanvasView : View
    {
        // Two collections for storing polylines
        Dictionary<int, FingerPaintPolyline> inProgressPolylines = new Dictionary<int, FingerPaintPolyline>();
        List<FingerPaintPolyline> completedPolylines = new List<FingerPaintPolyline>();

        Paint paint = new Paint();

        public FingerPaintCanvasView(Context context) : base(context)
        {
            Initialize();
        }

        public FingerPaintCanvasView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        void Initialize()
        {
        }

        public override bool OnTouchEvent(MotionEvent args)
        {
            // Get the pointer index
            int pointerIndex = args.ActionIndex;

            // Get the id to identify a finger over the course of its progress
            int id = args.GetPointerId(pointerIndex);

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:

                    // Create a Polyline, set the initial point, and store it
                    FingerPaintPolyline polyline = new FingerPaintPolyline
                    {
                        Color = Color.Blue,
                        StrokeWidth = 10
                    };

                    polyline.Path.MoveTo(args.GetX(pointerIndex),
                                         args.GetY(pointerIndex));

                    inProgressPolylines.Add(id, polyline);
                    break;

                case MotionEventActions.Move:

                    // Multiple Move events can be bundled, so handle them differently
                    for (pointerIndex = 0; pointerIndex < args.PointerCount; pointerIndex++)
                    {
                        id = args.GetPointerId(pointerIndex);

                        inProgressPolylines[id].Path.LineTo(args.GetX(pointerIndex),
                                                            args.GetY(pointerIndex));
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:

                    inProgressPolylines[id].Path.LineTo(args.GetX(pointerIndex),
                                                        args.GetY(pointerIndex));

                    // Transfer the in-progress polyline to a completed polyline
                    completedPolylines.Add(inProgressPolylines[id]);
                    inProgressPolylines.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    inProgressPolylines.Remove(id);
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine(args.Action);
                    break;
            }
        
            // Invalidate to update the view
            Invalidate();

            // Request continued touch input
            return true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            // Clear screen to white
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = Color.White;
            canvas.DrawPaint(paint);

            // Draw strokes
            paint.SetStyle(Paint.Style.Stroke);

            // Draw the completed polylines
            foreach (FingerPaintPolyline polyline in completedPolylines)
            {
                paint.Color = polyline.Color;
                paint.StrokeWidth = polyline.StrokeWidth;
                canvas.DrawPath(polyline.Path, paint);
            }

            // Draw the in-progress polylines
            foreach (FingerPaintPolyline polyline in inProgressPolylines.Values)
            {
                paint.Color = polyline.Color;
                paint.StrokeWidth = polyline.StrokeWidth;
                canvas.DrawPath(polyline.Path, paint);
            }
        }
    }
}