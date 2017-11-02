using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Java.Lang;
using static Android.Graphics.Paint;

namespace AutofillFramework.app
{
	/// <summary>
	/// Custom View with virtual child views for Username/Password text fields.
	/// </summary>
	[Register("com.xamarin.AutofillFramework.app.CustomVirtualView")]
	public class CustomVirtualView : View
	{
		static string LogTag = "CustomVirtualView";

    	static int TOP_MARGIN = 100;
		static int LEFT_MARGIN = 100;
		static int TEXT_HEIGHT = 90;
		static int VERTICAL_GAP = 10;
		static int LINE_HEIGHT = TEXT_HEIGHT + VERTICAL_GAP;
		static int UNFOCUSED_COLOR = Color.Black;
		static int FOCUSED_COLOR = Color.Red;
		public static int sNextId;

		List<Line> VirtualViewGroups = new List<Line>();
		SparseArray<Item> VirtualViews = new SparseArray<Item>();
		AutofillManager AutofillManager;

    	Line FocusedLine;
		Paint TextPaint;

		Line UsernameLine;
		Line PasswordLine;

		public CustomVirtualView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			AutofillManager = (AutofillManager)context.GetSystemService(Class.FromType(typeof(AutofillManager)));
			TextPaint = new Paint();
			TextPaint.SetStyle(Style.Fill);
			TextPaint.TextSize = TEXT_HEIGHT;
			UsernameLine = AddLine("usernameField", context.GetString(Resource.String.username_label),
						new string[] { AutofillHintUsername }, "         ", true);
			PasswordLine = AddLine("passwordField", context.GetString(Resource.String.password_label),
						new string[] { AutofillHintPassword }, "         ", false);
		}

		public override void Autofill(SparseArray values)
		{
			// User has just selected a Dataset from the list of autofill suggestions.
			// The Dataset is comprised of a list of AutofillValues, with each AutofillValue meant
			// to fill a specific autofillable view. Now we have to update the UI based on the
			// AutofillValues in the list.
			Log.Debug(LogTag, "autoFill(): " + values);
			for (int i = 0; i < values.Size(); i++)
			{
				var id = values.KeyAt(i);
				var value = (AutofillValue)values.ValueAt(i);
				Item item = VirtualViews.Get(id);
				if (item != null && item.Editable)
				{
					// Set the item's text to the text wrapped in the AutofillValue.
					item.Text = value.TextValue;
				}
				else if (item == null)
				{
					Log.Warn(LogTag, "No item for id " + id);
				}
				else
				{
					Log.Warn(LogTag, "Item for id " + id + " is not editable: " + item);
				}
			}
			PostInvalidate();
		}

		public override void OnProvideAutofillVirtualStructure(ViewStructure structure, AutofillFlags flags)
		{
			// Build a ViewStructure that will get passed to the AutofillService by the framework
			// when it is time to find autofill suggestions.
			structure.SetClassName(Class.Name);
			var childrenSize = VirtualViews.Size();
			Log.Debug(LogTag, "onProvideAutofillVirtualStructure(): flags = " + flags + ", items = "
			          + childrenSize + ", extras: " + CommonUtil.BundleToString(structure.Extras));
			var index = structure.AddChildCount(childrenSize);
			// Traverse through the view hierarchy, including virtual child views. For each view, we
			// need to set the relevant autofill metadata and add it to the ViewStructure.
			for (int i = 0; i < childrenSize; i++)
			{
				Item item = VirtualViews.ValueAt(i);
				Log.Debug(LogTag, "Adding new child at index " + index + ": " + item);
				var child = structure.NewChild(index);
				child.SetAutofillId(structure.AutofillId, item.Id);
				child.SetAutofillHints(item.Hints);
				child.SetAutofillType(item.Type);
				child.SetDataIsSensitive(!item.Sanitized);
				child.Text = item.Text;
				child.SetAutofillValue(AutofillValue.ForText(item.Text));
				child.SetFocused(item.Focused);
				child.SetId(item.Id, Context.PackageName, null, item.Line.IdEntry);
				child.SetClassName(item.getClassName());
				index++;
			}
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			Log.Debug(LogTag, "onDraw: " + VirtualViewGroups.Count + " lines; canvas:" + canvas);
			float x;
			float y = TOP_MARGIN + LINE_HEIGHT;
			for (int i = 0; i < VirtualViewGroups.Count; i++)
			{
				x = LEFT_MARGIN;
				Line line = VirtualViewGroups[i];
				Log.Verbose(LogTag, "Drawing '" + line + "' at " + x + "x" + y);
				TextPaint.Color = new Color(line.FieldTextItem.Focused ? FOCUSED_COLOR : UNFOCUSED_COLOR);
				string readOnlyText = line.LabelItem.Text + ":  [";
				string writeText = line.FieldTextItem.Text + "]";
				// Paints the label first...
				canvas.DrawText(readOnlyText, x, y, TextPaint);
				// ...then paints the edit text and sets the proper boundary
				float deltaX = TextPaint.MeasureText(readOnlyText);
				x += deltaX;
				line.Bounds = new Rect((int)x, (int)(y - LINE_HEIGHT),
						 (int)(x + TextPaint.MeasureText(writeText)), (int)y);
				Log.Debug(LogTag, "setBounds(" + x + ", " + y + "): " + line.Bounds);
				canvas.DrawText(writeText, x, y, TextPaint);
				y += LINE_HEIGHT;
			}
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			int y = (int) e.GetY();
			Log.Debug(LogTag, "Touched: y=" + y + ", range=" + LINE_HEIGHT + ", top=" + TOP_MARGIN);
	        int lowerY = TOP_MARGIN;
			int upperY = -1;
			for (int i = 0; i < VirtualViewGroups.Count; i++) {
	            upperY = lowerY + LINE_HEIGHT;
				Line line = VirtualViewGroups[i];
				Log.Debug(LogTag, "Line " + i + " ranges from " + lowerY + " to " + upperY);
	            if (lowerY <= y && y <= upperY) {
	                if (FocusedLine != null) {
						Log.Debug(LogTag, "Removing focus from " + FocusedLine);
						FocusedLine.ChangeFocus(false);
	                }
					Log.Debug(LogTag, "Changing focus to " + line);
	                FocusedLine = line;
	                FocusedLine.ChangeFocus(true);

					Invalidate();
	                break;
	            }
				lowerY += LINE_HEIGHT;
        	}
			return base.OnTouchEvent(e);
		}

		public string UsernameText => UsernameLine.FieldTextItem.Text;

		public string PasswordText => PasswordLine.FieldTextItem.Text;

		public void ResetFields()
		{
			UsernameLine.Reset();
			PasswordLine.Reset();
			PostInvalidate();
		}

		Line AddLine(string idEntry, string label, string[] hints, string text, bool sanitized)
		{
			Line line = new Line(idEntry, label, hints, text, sanitized)
			{
				CustomVirtualView = this,
				AutofillManager = AutofillManager
			};
			VirtualViewGroups.Add(line);
			VirtualViews.Put(line.LabelItem.Id, line.LabelItem);
			VirtualViews.Put(line.FieldTextItem.Id, line.FieldTextItem);
			return line;
		}
	}
}
