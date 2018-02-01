using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Util;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace AutofillFramework
{
    public class CreditCardExpirationDatePickerView : AppCompatEditText
    {
        private static int CC_EXP_YEARS_COUNT = 5;

        /**
         * Calendar instance used for month / year calculations. Should be reset before each use.
         */
        private Calendar mTempCalendar;

        public int mMonth;
        public int mYear;

        protected CreditCardExpirationDatePickerView(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference, transfer)
        {
        }

        public CreditCardExpirationDatePickerView(Context context) : this(context, null)
        {
        }

        public CreditCardExpirationDatePickerView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public CreditCardExpirationDatePickerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(
            context, attrs, defStyleAttr)
        {
            // Use the current date as the initial date in the picker.
            mTempCalendar = Calendar.Instance;
            mYear = mTempCalendar.Get(Calendar.Year);
            mMonth = mTempCalendar.Get(Calendar.Month);
        }

        /**
         * Gets a temporary calendar set with the View's year and month.
         */
        private Calendar GetCalendar()
        {
            mTempCalendar.Clear();
            mTempCalendar.Set(Calendar.Year, mYear);
            mTempCalendar.Set(Calendar.Month, mMonth);
            mTempCalendar.Set(Calendar.Date, 1);
            return mTempCalendar;
        }

        public override AutofillValue AutofillValue
        {
            get
            {
                var c = GetCalendar();
                var value = AutofillValue.ForDate(c.TimeInMillis);
                if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "getAutofillValue(): " + value);
                return value;
            }
        }

        public override void Autofill(AutofillValue value)
        {
            if (value == null || !value.IsDate)
            {
                Log.Warn(CommonUtil.TAG, "autofill(): invalid value " + value);
                return;
            }
            var time = value.DateValue;
            mTempCalendar.TimeInMillis = time;
            var year = mTempCalendar.Get(Calendar.Year);
            var month = mTempCalendar.Get(Calendar.Month);
            if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "autofill(" + value + "): " + month + "/" + year);
            SetDate(year, month);
        }

        private void SetDate(int year, int month)
        {
            mYear = year;
            mMonth = month;
            var selectedDate = new Date(GetCalendar().TimeInMillis);
            var dateString = DateFormat.GetDateFormat(Context).Format(selectedDate);
            Text = dateString;
        }

        public override AutofillType AutofillType => AutofillType.Text;

        public void Reset()
        {
            mTempCalendar.TimeInMillis = DateTime.Now.Millisecond;
            SetDate(mTempCalendar.Get(Calendar.Year), mTempCalendar.Get(Calendar.Month));
        }

        public void ShowDatePickerDialog(FragmentManager fragmentManager)
        {
            var newFragment = new DatePickerFragment();
            newFragment.mParent = this;
            newFragment.Show(fragmentManager, "datePicker");
        }

        public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
        {
            public CreditCardExpirationDatePickerView that;
            public CreditCardExpirationDatePickerView mParent;

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var dialog = new DatePickerDialog(Activity, Resource.Style.CustomDatePickerDialogTheme, this,
                    mParent.mYear, mParent.mMonth, 1);

                DatePicker datePicker = dialog.DatePicker;

                // Limit range.
                Calendar c = mParent.GetCalendar();
                datePicker.MinDate = c.TimeInMillis;
                c.Set(Calendar.Year, mParent.mYear + CC_EXP_YEARS_COUNT - 1);
                datePicker.MaxDate = c.TimeInMillis;

                // Remove day.
                datePicker.FindViewById(Resources.GetIdentifier("day", "id", "android"))
                    .Visibility = ViewStates.Gone;
                return dialog;
            }

            public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
            {
                mParent.SetDate(year, month);
            }
        }
    }
}