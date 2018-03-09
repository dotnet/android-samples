using System.Collections.Generic;
using System.Linq;
using Android.Service.Autofill;
using Android.Views;
using AutofillService.Model;

namespace AutofillService
{
	/**
	 * Holds the properties associated with an autofill hint in this Autofill Service.
	 */
	public class AutofillHintProperties
	{
		private string mAutofillHint;
		private IFakeFieldGenerator mFakeFieldGenerator;
		private HashSet<AutofillType> mValidTypes;
		private SaveDataType mSaveType;
		private int mPartition;

		public AutofillHintProperties(string autofillHint, SaveDataType saveType, int partitionNumber,
			IFakeFieldGenerator fakeFieldGenerator, params AutofillType[] validTypes)
		{
			mAutofillHint = autofillHint;
			mSaveType = saveType;
			mPartition = partitionNumber;
			mFakeFieldGenerator = fakeFieldGenerator;
			mValidTypes = new HashSet<AutofillType>(validTypes.ToList());
		}

		/**
		 * Generates dummy autofill field data that is relevant to the autofill hint.
		 */
		public FilledAutofillField GenerateFakeField(int seed)
		{
			return mFakeFieldGenerator.Generate(seed);
		}

		/**
		 * Returns autofill hint associated with these properties. If you save a field that uses a W3C
		 * hint, there is a chance this will return a different but analogous hint, when applicable.
		 * For example, W3C has hint 'email' and {@link android.view.View} has hint 'emailAddress', so
		 * the W3C hint should map to the hint defined in {@link android.view.View} ('emailAddress').
		 */
		public string GetAutofillHint()
		{
			return mAutofillHint;
		}

		/**
		 * Returns how this hint maps to a {@link android.service.autofill.SaveInfo} type.
		 */
		public SaveDataType GetSaveType()
		{
			return mSaveType;
		}

		/**
		 * Returns which data partition this autofill hint should be a part of. See partitions defined
		 * in {@link AutofillHints}.
		 */
		public int GetPartition()
		{
			return mPartition;
		}


		/**
		 * Sometimes, data for a hint should only be stored as a certain AutofillValue type. For
		 * example, it is recommended that data representing a Credit Card Expiration date, annotated
		 * with the hint {@link android.view.View.AUTOFILL_HINT_CREDIT_CARD_EXPIRATION_DATE}, should
		 * only be stored as {@link android.view.View.AUTOFILL_TYPE_DATE}.
		 */
		public bool IsValidType(AutofillType type)
		{
			return mValidTypes.Contains(type);
		}
	}
}