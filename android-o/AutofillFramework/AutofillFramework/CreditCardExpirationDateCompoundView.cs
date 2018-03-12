using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using Java.Lang;
using Java.Util;

namespace AutofillFramework
{
    public class CreditCardExpirationDateCompoundView : FrameLayout
    {
        private static int CC_EXP_YEARS_COUNT = 5;

        private string[] mYears = new string[CC_EXP_YEARS_COUNT];

        private Spinner mCcExpMonthSpinner;
        private Spinner mCcExpYearSpinner;

        protected CreditCardExpirationDateCompoundView(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference, transfer)
        {
        }

        public CreditCardExpirationDateCompoundView(Context context) : this(context, null)
        {
        }

        public CreditCardExpirationDateCompoundView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public CreditCardExpirationDateCompoundView(Context context, IAttributeSet attrs, int defStyleAttr) : this(
            context, attrs, defStyleAttr, 0)
        {
        }

        public CreditCardExpirationDateCompoundView(Context context, IAttributeSet attrs, int defStyleAttr,
            int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            var rootView = LayoutInflater.From(context).Inflate(Resource.Layout.cc_exp_date, this);
            mCcExpMonthSpinner = rootView.FindViewById<Spinner>(Resource.Id.ccExpMonth);
            mCcExpYearSpinner = rootView.FindViewById<Spinner>(Resource.Id.ccExpYear);
            ImportantForAutofill = ImportantForAutofillYesExcludeDescendants;
            var monthAdapter = ArrayAdapter.CreateFromResource(context, Resource.Array.month_array,
                Android.Resource.Layout.SimpleSpinnerItem);
            monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            mCcExpMonthSpinner.Adapter = monthAdapter;
            int year = Calendar.Instance.Get(Calendar.Year);
            for (int i = 0; i < mYears.Length; i++)
            {
                mYears[i] = (year + i).ToString();
            }

            mCcExpYearSpinner.Adapter =
                new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem, mYears);
            var onItemSelectedListener = new ItemSelectedListener
            {
                that = this,
                context = context
            };

            mCcExpMonthSpinner.OnItemSelectedListener = onItemSelectedListener;
            mCcExpYearSpinner.OnItemSelectedListener = onItemSelectedListener;
        }

        public class ItemSelectedListener : Java.Lang.Object, AdapterView.IOnItemSelectedListener
        {
            public Context context;
            public CreditCardExpirationDateCompoundView that;

            public void OnItemSelected(AdapterView parent, View view, int position, long id)
            {
                ((AutofillManager) context.GetSystemService(Class.FromType(typeof(AutofillManager))))
                    .NotifyValueChanged(that);
            }

            public void OnNothingSelected(AdapterView parent)
            {
            }
        }

        public override AutofillValue AutofillValue
        {
            get
            {
                var calendar = Calendar.Instance;
                // Set hours, minutes, seconds, and millis to 0 to ensure getAutofillValue() == the value
                // set by autofill(). Without this line, the view will not turn yellow when updated.
                calendar.Clear();
                var year = Integer.ParseInt(mCcExpYearSpinner.SelectedItem.ToString());
                var month = mCcExpMonthSpinner.SelectedItemPosition;
                calendar.Set(Calendar.Year, year);
                calendar.Set(Calendar.Month, month);
                var unixTime = calendar.TimeInMillis;
                return AutofillValue.ForDate(unixTime);
            }
        }

        public override void Autofill(AutofillValue value)
        {
            if (!value.IsDate)
            {
                Log.Warn(CommonUtil.TAG, "Ignoring autofill() because service sent a non-date value:" + value);
                return;
            }

            var calendar = Calendar.Instance;
            calendar.TimeInMillis = value.DateValue;
            var month = calendar.Get(Calendar.Month);
            var year = calendar.Get(Calendar.Year);
            mCcExpMonthSpinner.SetSelection(month);
            mCcExpYearSpinner.SetSelection(year - Integer.ParseInt(mYears[0]));
        }

        public override AutofillType AutofillType => AutofillType.Date;

        public void Reset()
        {
            mCcExpMonthSpinner.SetSelection(0);
            mCcExpYearSpinner.SetSelection(0);
        }
    }
}