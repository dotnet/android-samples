using Android.Util;
using GoogleGson.Annotations;
using Newtonsoft.Json;

namespace AutofillService.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FilledAutofillField
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mTextValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long mDateValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool mToggleValue { get; set; }

        //TODO add explicit mListValue

        /**
         * Does not need to be serialized into persistent storage, so it's not exposed.
         */
        private string[] mAutofillHints;

        [JsonConstructor]
        public FilledAutofillField()
        {
        }

        public FilledAutofillField(params string[] hints)
        {
            mAutofillHints = AutofillHints.FilterForSupportedHints(hints);
            AutofillHints.ConvertToStoredHintNames(mAutofillHints);
        }

        public void SetListValue(string[] autofillOptions, int listValue)
        {
            /* Only set list value when a hint is allowed to store list values. */
            //Preconditions.CheckArgument(AutofillHints.IsValidTypeForHints(mAutofillHints, AutofillType.List),
            //    "List is invalid autofill type for hint(s) - %s",
            //    mAutofillHints.ToString());
            if (autofillOptions != null && autofillOptions.Length > 0)
            {
                mTextValue = autofillOptions[listValue];
            }
            else
            {
                Log.Warn(CommonUtil.TAG, "autofillOptions should have at least one entry.");
            }
        }

        public string[] GetAutofillHints()
        {
            return mAutofillHints;
        }

        public string GetTextValue()
        {
            return mTextValue;
        }

        public void SetTextValue(string textValue)
        {
            /* Only set text value when a hint is allowed to store text values. */
            //Preconditions.CheckArgument(AutofillHints.IsValidTypeForHints(mAutofillHints, AutofillType.Text),
            //    "Text is invalid autofill type for hint(s) - %s",
            //    mAutofillHints.ToString());
            mTextValue = textValue;
        }

        public long GetDateValue()
        {
            return mDateValue;
        }

        public void SetDateValue(long dateValue)
        {
            /* Only set date value when a hint is allowed to store date values. */
            //Preconditions.CheckArgument(AutofillHints.IsValidTypeForHints(mAutofillHints, AutofillType.Date),
            //    "Date is invalid autofill type for hint(s) - %s"
            //    , mAutofillHints.ToString());
            mDateValue = dateValue;
        }

        public bool GetToggleValue()
        {
            return mToggleValue;
        }

        public void SetToggleValue(bool toggleValue)
        {
            /* Only set toggle value when a hint is allowed to store toggle values. */
            //Preconditions.CheckArgument(AutofillHints.IsValidTypeForHints(mAutofillHints, AutofillType.Toggle),
            //    "Toggle is invalid autofill type for hint(s) - %s",
            //    mAutofillHints.ToString());
            mToggleValue = toggleValue;
        }

        public bool IsNull()
        {
            return mTextValue == null && mDateValue == null && mToggleValue == null;
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            FilledAutofillField that = (FilledAutofillField) o;

            if (mTextValue != null ? !mTextValue.Equals(that.mTextValue) : that.mTextValue != null)
                return false;
            if (mDateValue != null ? !mDateValue.Equals(that.mDateValue) : that.mDateValue != null)
                return false;
            return mToggleValue != null ? mToggleValue.Equals(that.mToggleValue) : that.mToggleValue == null;
        }

        public override int GetHashCode()
        {
            int result = mTextValue != null ? mTextValue.GetHashCode() : 0;
            result = 31 * result + (mDateValue != null ? mDateValue.GetHashCode() : 0);
            result = 31 * result + (mToggleValue != null ? mToggleValue.GetHashCode() : 0);
            return result;
        }
    }
}