using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Lang;
using Java.Util;
using static Android.Graphics.Paint;

namespace AutofillFramework.app
{
	public class CustomVirtualView : View
	{
		static string Tag = "CustomView";

    	static int TOP_MARGIN = 100;
		static int LEFT_MARGIN = 100;
		static int TEXT_HEIGHT = 90;
		static int VERTICAL_GAP = 10;
		static int LINE_HEIGHT = TEXT_HEIGHT + VERTICAL_GAP;
		static int UNFOCUSED_COLOR = Color.Black;
		static int FOCUSED_COLOR = Color.Red;
		static int sNextId;

		List<Line> VirtualViewGroups = new List<Line>();
		SparseArray<Item> VirtualViews = new SparseArray<Item>();
		AutofillManager AutofillManager;

    	Line FocusedLine;
		Paint TextPaint;

		Line UsernameLine;
		Line PasswordLine;

		public CustomVirtualView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			AutofillManager = (AutofillManager)context.GetSystemService(typeof(AutofillManager).Name);
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
			Log.Debug(Tag, "autoFill(): " + values);
			for (int i = 0; i < values.Size(); i++)
			{
				var id = values.KeyAt(i);
				AutofillValue value = (Android.Views.Autofill.AutofillValue)values.ValueAt(i);
				Item item = VirtualViews.Get(id);
				if (item != null && item.editable)
				{
					// Set the item's text to the text wrapped in the AutofillValue.
					item.text = new Java.Lang.String(value.TextValue);
				}
				else if (item == null)
				{
					Log.Warn(Tag, "No item for id " + id);
				}
				else
				{
					Log.Warn(Tag, "Item for id " + id + " is not editable: " + item);
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
			Log.Debug(Tag, "onProvideAutofillVirtualStructure(): flags = " + flags + ", items = "
			          + childrenSize + ", extras: " + CommonUtil.BundleToString(structure.Extras));
			var index = structure.AddChildCount(childrenSize);
			// Traverse through the view hierarchy, including virtual child views. For each view, we
			// need to set the relevant autofill metadata and add it to the ViewStructure.
			for (int i = 0; i < childrenSize; i++)
			{
				Item item = VirtualViews.ValueAt(i);
				Log.Debug(Tag, "Adding new child at index " + index + ": " + item);
				var child = structure.NewChild(index);
				child.SetAutofillId(structure.AutofillId, item.id);
				child.SetAutofillHints(item.hints);
				child.SetAutofillType(item.type);
				child.SetDataIsSensitive(!item.sanitized);
				child.Text = item.text.ToString();
				child.SetAutofillValue(AutofillValue.ForText(item.text));
				child.SetFocused(item.focused);
				child.SetId(item.id, Context.PackageName, null, item.line.idEntry);
				child.SetClassName(item.getClassName());
				index++;
			}
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			Log.Debug(Tag, "onDraw: " + VirtualViewGroups.Count + " lines; canvas:" + canvas);
			float x;
			float y = TOP_MARGIN + LINE_HEIGHT;
			for (int i = 0; i < VirtualViewGroups.Count; i++)
			{
				x = LEFT_MARGIN;
				Line line = VirtualViewGroups[i];
				Log.Verbose(Tag, "Drawing '" + line + "' at " + x + "x" + y);
				TextPaint.Color = new Color(line.fieldTextItem.focused ? FOCUSED_COLOR : UNFOCUSED_COLOR);
				string readOnlyText = line.labelItem.text + ":  [";
				string writeText = line.fieldTextItem.text + "]";
				// Paints the label first...
				canvas.DrawText(readOnlyText, x, y, TextPaint);
				// ...then paints the edit text and sets the proper boundary
				float deltaX = TextPaint.MeasureText(readOnlyText);
				x += deltaX;
				line.bounds = new Rect((int)x, (int)(y - LINE_HEIGHT),
						 (int)(x + TextPaint.MeasureText(writeText)), (int)y);
				Log.Debug(Tag, "setBounds(" + x + ", " + y + "): " + line.bounds);
				canvas.DrawText(writeText, x, y, TextPaint);
				y += LINE_HEIGHT;
			}
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			int y = (int) e.GetY();
			Log.Debug(Tag, "Touched: y=" + y + ", range=" + LINE_HEIGHT + ", top=" + TOP_MARGIN);
	        int lowerY = TOP_MARGIN;
			int upperY = -1;
			for (int i = 0; i < VirtualViewGroups.Count; i++) {
	            upperY = lowerY + LINE_HEIGHT;
				Line line = VirtualViewGroups[i];
				Log.Debug(Tag, "Line " + i + " ranges from " + lowerY + " to " + upperY);
	            if (lowerY <= y && y <= upperY) {
	                if (FocusedLine != null) {
						Log.Debug(Tag, "Removing focus from " + FocusedLine);
						FocusedLine.ChangeFocus(false);
	                }
					Log.Debug(Tag, "Changing focus to " + line);
	                FocusedLine = line;
	                FocusedLine.ChangeFocus(true);

					Invalidate();
	                break;
	            }
				lowerY += LINE_HEIGHT;
        	}
			return base.OnTouchEvent(e);
		}

		public ICharSequence GetUsernameText()
		{
			return UsernameLine.fieldTextItem.text;
		}

		public ICharSequence GetPasswordText()
		{
			return PasswordLine.fieldTextItem.text;
		}

		public void ResetFields()
		{
			UsernameLine.Reset();
			PasswordLine.Reset();
			PostInvalidate();
		}

		Line AddLine(string idEntry, string label, string[] hints, string text,
			bool sanitized)
		{
			Line line = new Line(idEntry, label, hints, text, sanitized) {AutofillManager = this.AutofillManager};
			VirtualViewGroups.Add(line);
			VirtualViews.Put(line.labelItem.id, line.labelItem);
			VirtualViews.Put(line.fieldTextItem.id, line.fieldTextItem);
			return line;
		}

		class Item 
		{
			public Line line { get; set; }
        	public int id { get; set; }
			public bool editable { get; set; }
	        public bool sanitized { get; set; }
	        public string[] hints { get; set; }
	        public int type { get; set; }
			public ICharSequence text { get; set; }
			public bool focused { get; set; }

			public Item(Line line, int id, string[] hints, int type, ICharSequence text, bool editable,
				bool sanitized)
			{
				this.line = line;
				this.id = id;
				this.text = text;
				this.editable = editable;
				this.sanitized = sanitized;
				this.hints = hints;
				this.type = type;
			}

			public override string ToString()
			{
				return id + ": " + text + (editable ? " (editable)" : " (read-only)"
					+ (sanitized ? " (sanitized)" : " (sensitive"));
			}

			public string getClassName() => editable ? typeof(EditText).Name : typeof(TextView).Name;
		}

		class Line
		{
			// Boundaries of the text field, relative to the CustomView
			public Rect bounds { get; set; }
			public Item labelItem;
			public Item fieldTextItem;
			public string idEntry { get; }
			public CustomVirtualView CustomVirtualView { get; set; }
			public AutofillManager AutofillManager { get; set; }

			public Line(string idEntry, string label, string[] hints, string text, bool sanitized)
			{
				this.idEntry = idEntry;
				this.bounds = new Rect();
				this.labelItem = new Item(this, ++sNextId, null, (int) AutofillType.None, new Java.Lang.String(label),
					false, true);
				this.fieldTextItem = new Item(this, ++sNextId, hints, (int) AutofillType.Text, new Java.Lang.String(text),
						true, sanitized);
			}

			public void ChangeFocus(bool focused)
			{
				fieldTextItem.focused = focused;
				if (focused)
				{
					Rect absBounds = GetAbsCoordinates();
					Log.Debug(Tag, "focus gained on " + fieldTextItem.id + "; absBounds=" + absBounds);
					AutofillManager.NotifyViewEntered(CustomVirtualView, fieldTextItem.id, absBounds);
				}
				else
				{
					Log.Debug(Tag, "focus lost on " + fieldTextItem.id);
					AutofillManager.NotifyViewExited(CustomVirtualView, fieldTextItem.id);
				}
			}

			Rect GetAbsCoordinates()
			{
				// Must offset the boundaries so they're relative to the CustomView.
				int[] offset = new int[2];
				CustomVirtualView.GetLocationOnScreen(offset);
				Rect absBounds = new Rect(bounds.Left + offset[0],
						bounds.Top + offset[1],
						bounds.Right + offset[0], bounds.Bottom + offset[1]);
				Log.Verbose(Tag, "getAbsCoordinates() for " + fieldTextItem.id + ": bounds=" + bounds
						+ " offset: " + Arrays.ToString(offset) + " absBounds: " + absBounds);
				return absBounds;
			}

			public void Reset()
			{
				fieldTextItem.text = new Java.Lang.String("        ");
			}

			public override string ToString()
			{
				return "Label: " + labelItem + " Text: " + fieldTextItem + " Focused: " +
					fieldTextItem.focused;
			}
		}
	}
}
