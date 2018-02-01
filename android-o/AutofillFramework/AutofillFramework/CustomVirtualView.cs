using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Util;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Lang;
using Java.Util;
using String = System.String;

namespace AutofillFramework
{
    public class CustomVirtualView : View
    {
        protected static bool DEBUG = true;
        protected static bool VERBOSE = false;

        /**
		 * When set, it notifies AutofillManager of focus change as the view scrolls, so the
		 * autofill UI is continually drawn.
		 * <p>
		 * <p>This is janky and incompatible with the way the autofill UI works on native views, but
		 * it's a cool experiment!
		 */
        private static bool DRAW_AUTOFILL_UI_AFTER_SCROLL = false;

        private static String TAG = "CustomView";
        private static int DEFAULT_TEXT_HEIGHT_DP = 34;
        private static int VERTICAL_GAP = 10;
        private static Color UNFOCUSED_COLOR = Color.Black;
        private static Color FOCUSED_COLOR = Color.Red;
        private static int sNextId;
        protected static AutofillManager mAutofillManager;
        public static List<Line> mVirtualViewGroups = new List<Line>();
        private static SparseArray<Item> mVirtualViews = new SparseArray<Item>();
        private static SparseArray<Partition> mPartitionsByAutofillId = new SparseArray<Partition>();
        private Dictionary<String, Partition> mPartitionsByName = new Dictionary<String, Partition>();
        protected Line mFocusedLine;
        protected int mTopMargin;
        protected int mLeftMargin;
        private Paint mTextPaint;
        private int mTextHeight;
        private int mLineLength;

        protected CustomVirtualView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomVirtualView(Context context) : this(context, null)
        {
        }

        public CustomVirtualView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public CustomVirtualView(Context context, IAttributeSet attrs, int defStyleAttr) : this(context, attrs,
            defStyleAttr, 0)
        {
        }

        public CustomVirtualView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(
            context,
            attrs, defStyleAttr, defStyleRes)
        {
            mAutofillManager = (AutofillManager) context.GetSystemService(Class.FromType(typeof(AutofillManager)));
            mTextPaint = new Paint();
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomVirtualView, defStyleAttr,
                defStyleRes);
            var defaultHeight = (int) (DEFAULT_TEXT_HEIGHT_DP * Resources.DisplayMetrics.Density);
            mTextHeight =
                typedArray.GetDimensionPixelSize(Resource.Styleable.CustomVirtualView_internalTextSize, defaultHeight);
            typedArray.Recycle();
            ResetCoordinates();
        }

        protected void ResetCoordinates()
        {
            mTextPaint.SetStyle(Paint.Style.Fill);
            mTextPaint.TextSize = mTextHeight;
            mTopMargin = PaddingTop;
            mLeftMargin = PaddingStart;
            mLineLength = mTextHeight + VERTICAL_GAP;
        }

        public override void Autofill(SparseArray values)
        {
            Context context = Context;

            // User has just selected a Dataset from the list of autofill suggestions.
            // The Dataset is comprised of a list of AutofillValues, with each AutofillValue meant
            // to fill a specific autofillable view. Now we have to update the UI based on the
            // AutofillValues in the list, but first we make sure all autofilled values belong to the
            // same partition
            if (DEBUG) Log.Debug(TAG, "autofill(): " + values);

            // First get the name of all partitions in the values
            ArraySet partitions = new ArraySet();
            for (int i = 0; i < values.Size(); i++)
            {
                int id = values.KeyAt(i);
                var partition = mPartitionsByAutofillId.Get(id);
                if (partition == null)
                {
                    ShowError(context.GetString(Resource.String.message_autofill_no_partitions, id,
                        mPartitionsByAutofillId));
                    return;
                }

                partitions.Add(partition.mName);
            }

            // Then make sure they follow the Highlander rule (There can be only one)
            if (partitions.Size() != 1)
            {
                ShowError(context.GetString(Resource.String.message_autofill_blocked, partitions));
                return;
            }

            // Finally, autofill it.
            var df = Android.Text.Format.DateFormat.GetDateFormat(context);
            for (int i = 0; i < values.Size(); i++)
            {
                int id = values.KeyAt(i);
                var value = (AutofillValue) values.ValueAt(i);
                var item = mVirtualViews.Get(id);

                if (item == null)
                {
                    Log.Warn(TAG, "No item for id " + id);
                    continue;
                }

                if (!item.editable)
                {
                    ShowError(context.GetString(Resource.String.message_autofill_readonly, item.text.ToString()));
                    continue;
                }

                // Check if the type was properly set by the autofill service
                if (DEBUG)
                {
                    Log.Debug(TAG, "Validating " + i
                                                 + ": expectedType=" + CommonUtil.GetTypeAsString(item.type)
                                                 + "(" + item.type + "), value=" + value);
                }

                bool valid = false;
                if (value.IsText && item.type == (int) AutofillType.Text)
                {
                    item.text = value.TextValue;
                    valid = true;
                }
                else if (value.IsDate && item.type == (int) AutofillType.Date)
                {
                    item.text = df.Format(new Date(value.DateValue));
                    valid = true;
                }
                else
                {
                    Log.Warn(TAG, "Unsupported type: " + value);
                }

                if (!valid)
                {
                    item.text = context.GetString(Resource.String.message_autofill_invalid);
                }
            }

            PostInvalidate();
            ShowMessage(context.GetString(Resource.String.message_autofill_ok, partitions.ValueAt(0)));
        }

        public override void OnProvideAutofillVirtualStructure(ViewStructure structure, AutofillFlags flags)
        {
            // Build a ViewStructure that will get passed to the AutofillService by the framework
            // when it is time to find autofill suggestions.
            structure.SetClassName(Class.Name);
            int childrenSize = mVirtualViews.Size();
            if (DEBUG)
            {
                Log.Debug(TAG, "onProvideAutofillVirtualStructure(): flags = " + flags + ", items = "
                               + childrenSize + ", extras: " + CommonUtil.BundleToString(structure.Extras));
            }

            int index = structure.AddChildCount(childrenSize);
            // Traverse through the view hierarchy, including virtual child views. For each view, we
            // need to set the relevant autofill metadata and add it to the ViewStructure.
            for (int i = 0; i < childrenSize; i++)
            {
                var item = mVirtualViews.ValueAt(i);
                if (DEBUG) Log.Debug(TAG, "Adding new child at index " + index + ": " + item);
                var child = structure.NewChild(index);
                child.SetAutofillId(structure.AutofillId, item.id);
                child.SetAutofillHints(item.hints);
                child.SetAutofillType((AutofillType) item.type);
                child.SetAutofillValue(item.GetAutofillValue());
                child.SetDataIsSensitive(!item.sanitized);
                child.SetFocused(item.focused);
                child.SetVisibility(ViewStates.Visible);
                child.SetDimens(item.line.mBounds.Left, item.line.mBounds.Top, 0, 0,
                    item.line.mBounds.Width(), item.line.mBounds.Height());
                child.SetId(item.id, Context.PackageName, null, item.idEntry);
                child.SetClassName(item.GetClassName());
                child.SetDimens(item.line.mBounds.Left, item.line.mBounds.Top, 0, 0,
                    item.line.mBounds.Width(), item.line.mBounds.Height());
                index++;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (VERBOSE)
            {
                Log.Verbose(TAG, "onDraw(): " + mVirtualViewGroups.Count + " lines; canvas:" + canvas);
            }

            float x;
            float y = mTopMargin + mLineLength;
            for (int i = 0; i < mVirtualViewGroups.Count; i++)
            {
                Line line = mVirtualViewGroups[i];
                line.view = this;
                x = mLeftMargin;
                if (VERBOSE) Log.Verbose(TAG, "Drawing '" + line + "' at " + x + "x" + y);
                mTextPaint.Color = line.mFieldTextItem.focused ? FOCUSED_COLOR : UNFOCUSED_COLOR;
                var readOnlyText = line.mLabelItem.text + ":  [";
                var writeText = line.mFieldTextItem.text + "]";
                // Paints the label first...
                canvas.DrawText(readOnlyText, x, y, mTextPaint);
                // ...then paints the edit text and sets the proper boundary
                float deltaX = mTextPaint.MeasureText(readOnlyText);
                x += deltaX;
                line.mBounds.Set((int) x, (int) (y - mLineLength),
                    (int) (x + mTextPaint.MeasureText(writeText)), (int) y);
                if (VERBOSE) Log.Verbose(TAG, "setBounds(" + x + ", " + y + "): " + line.mBounds);
                canvas.DrawText(writeText, x, y, mTextPaint);
                y += mLineLength;

                if (DRAW_AUTOFILL_UI_AFTER_SCROLL)
                {
                    line.NotifyFocusChanged();
                }
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            int y = (int) e.GetY();
            OnMotion(y);
            return base.OnTouchEvent(e);
        }

        /**
     * Handles a motion event.
     *
     * @param y y coordinate.
     */
        protected void OnMotion(int y)
        {
            if (DEBUG)
            {
                Log.Debug(TAG, "onMotion(): y=" + y + ", range=" + mLineLength + ", top=" + mTopMargin);
            }

            int lowerY = mTopMargin;
            int upperY = -1;
            for (int i = 0; i < mVirtualViewGroups.Count; i++)
            {
                Line line = mVirtualViewGroups[i];
                upperY = lowerY + mLineLength;
                if (DEBUG) Log.Debug(TAG, "Line " + i + " ranges from " + lowerY + " to " + upperY);
                if (lowerY <= y && y <= upperY)
                {
                    if (mFocusedLine != null)
                    {
                        Log.Debug(TAG, "Removing focus from " + mFocusedLine);
                        mFocusedLine.ChangeFocus(false);
                    }

                    Log.Debug(TAG, "Changing focus to " + line);
                    mFocusedLine = line;
                    mFocusedLine.ChangeFocus(true);
                    Invalidate();
                    break;
                }

                lowerY += mLineLength;
            }
        }

        /**
		 * Creates a new partition with the given name.
		 *
		 * @throws IllegalArgumentException if such partition already exists.
		 */
        public Partition AddPartition(String name)
        {
            Preconditions.CheckNotNull(name, "Name cannot be null.");
            Preconditions.CheckArgument(!mPartitionsByName.ContainsKey(name),
                "Partition with such name already exists.");
            Partition partition = new Partition(name);
            mPartitionsByName.Add(name, partition);
            return partition;
        }

        private void ShowError(String message)
        {
            ShowMessage(true, message);
        }

        private void ShowMessage(String message)
        {
            ShowMessage(false, message);
        }

        private void ShowMessage(bool warning, String message)
        {
            if (warning)
            {
                Log.Warn(TAG, message);
            }
            else
            {
                Log.Info(TAG, message);
            }

            Toast.MakeText(Context, message, ToastLength.Long).Show();
        }

        public class Item
        {
            public int id;
            public string idEntry;
            public Line line;
            public bool editable;
            public bool sanitized;
            public string[] hints;
            public int type;
            public string text;
            public bool focused;
            private long date;

            public Item(Line line, int id, string idEntry, string[] hints, int type, string text,
                bool editable, bool sanitized)
            {
                this.line = line;
                this.id = id;
                this.idEntry = idEntry;
                this.text = text;
                this.editable = editable;
                this.sanitized = sanitized;
                this.hints = hints;
                this.type = type;
            }

            public override string ToString()
            {
                return id + "/" + idEntry + ": "
                       + (type == (int) AutofillType.Date
                           ? Long.ToString(date)
                           : text) // TODO: use DateFormat for date
                       + " (" + CommonUtil.GetTypeAsString(type) + ")"
                       + (editable
                           ? " (editable)"
                           : " (read-only)"
                             + (sanitized ? " (sanitized)" : " (sensitive"))
                       + (hints == null ? " (no hints)" : " ( " + hints + ")");
            }

            public string GetClassName()
            {
                return editable ? typeof(EditText).Name : typeof(TextView).Name;
            }

            public AutofillValue GetAutofillValue()
            {
                switch (type)
                {
                    case (int) AutofillType.Text:
                        return (TextUtils.GetTrimmedLength(text) > 0)
                            ? AutofillValue.ForText(text)
                            : null;
                    case (int) AutofillType.Date:
                        return AutofillValue.ForDate(date);
                    default:
                        return null;
                }
            }
        }

        /**
		 * A partition represents a logical group of items, such as credit card info.
		 */
        public class Partition
        {
            public string mName;
            private SparseArray<Line> mLines = new SparseArray<Line>();

            public Partition(String name)
            {
                mName = name;
            }

            /**
			 * Adds a new line (containining a label and an input field) to the view.
			 *
			 * @param idEntryPrefix id prefix used to identify the line - label node will be suffixed
			 *                      with {@code Label} and editable node with {@code Field}.
			 * @param autofillType  {@link View#getAutofillType() autofill type} of the field.
			 * @param label         text used in the label.
			 * @param text          initial text used in the input field.
			 * @param sensitive     whether the input is considered sensitive.
			 * @param autofillHints list of autofill hints.
			 * @return the new line.
			 */
            public Line AddLine(string idEntryPrefix, int autofillType, string label, string text, bool sensitive,
                params string[] autofillHints)
            {
                Preconditions.CheckArgument(autofillType == (int) AutofillType.Text ||
                                            autofillType == (int) AutofillType.Date,
                    "Unsupported type: " + autofillType);
                Line line = new Line(idEntryPrefix, autofillType, label, autofillHints, text, !sensitive);
                mVirtualViewGroups.Add(line);
                int id = line.mFieldTextItem.id;
                mLines.Put(id, line);
                mVirtualViews.Put(line.mLabelItem.id, line.mLabelItem);
                mVirtualViews.Put(id, line.mFieldTextItem);
                mPartitionsByAutofillId.Put(id, this);

                return line;
            }

            /**
			 * Resets the value of all items in the partition.
			 */
            public void Reset()
            {
                for (int i = 0; i < mLines.Size(); i++)
                {
                    mLines.ValueAt(i).Reset();
                }
            }

            public override String ToString()
            {
                return mName;
            }
        }

        public class Line
        {
            public View view;

            public Item mFieldTextItem;

            // Boundaries of the text field, relative to the CustomView
            public Rect mBounds = new Rect();

            public Item mLabelItem;
            private int mAutofillType;


            public Line(string idEntryPrefix, int autofillType, string label, string[] hints, string text,
                bool sanitized)
            {
                mAutofillType = autofillType;
                mLabelItem = new Item(this, ++sNextId, idEntryPrefix + "Label", null, (int) AutofillType.None,
                    label, false, true);
                mFieldTextItem = new Item(this, ++sNextId, idEntryPrefix + "Field",
                    hints,
                    autofillType, text, true, sanitized);
            }

            public void ChangeFocus(bool focused)
            {
                mFieldTextItem.focused = focused;
                NotifyFocusChanged();
            }

            public void NotifyFocusChanged()
            {
                if (mFieldTextItem.focused)
                {
                    Rect absBounds = GetAbsCoordinates();
                    if (DEBUG)
                    {
                        Log.Debug(TAG, "focus gained on " + mFieldTextItem.id + "; absBounds=" + absBounds);
                    }

                    mAutofillManager.NotifyViewEntered(view, mFieldTextItem.id, absBounds);
                }
                else
                {
                    if (DEBUG) Log.Debug(TAG, "focus lost on " + mFieldTextItem.id);
                    mAutofillManager.NotifyViewExited(view, mFieldTextItem.id);
                }
            }

            private Rect GetAbsCoordinates()
            {
                // Must offset the boundaries so they're relative to the CustomView.
                var offset = new int[2];
                view.GetLocationOnScreen(offset);
                Rect absBounds = new Rect(mBounds.Left + offset[0],
                    mBounds.Top + offset[1],
                    mBounds.Right + offset[0], mBounds.Bottom + offset[1]);
                if (VERBOSE)
                {
                    Log.Verbose(TAG, "getAbsCoordinates() for " + mFieldTextItem.id + ": bounds=" + mBounds
                                     + " offset: " + Arrays.ToString(offset) + " absBounds: " + absBounds);
                }

                return absBounds;
            }

            /**
			 * Gets the value of the input field text.
			 */
            public string GetText()
            {
                return mFieldTextItem.text;
            }

            /**
			 * Resets the value of the input field text.
			 */
            public void Reset()
            {
                mFieldTextItem.text = "        ";
            }

            public override String ToString()
            {
                return "Label: " + mLabelItem + " Text: " + mFieldTextItem + " Focused: " +
                       mFieldTextItem.focused + " Type: " + mAutofillType;
            }
        }
    }
}