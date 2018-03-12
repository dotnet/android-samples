using System;
using Android.App.Assist;
using Android.Service.Autofill;
using Android.Views;
using Android.Views.Autofill;

namespace AutofillService
{
	/**
	 * A stripped down version of a {@link ViewNode} that contains only autofill-relevant metadata. It
	 * also contains a {@code mSaveType} flag that is calculated based on the {@link ViewNode}]'s
	 * autofill hints.
	 */
	public class AutofillFieldMetadata
	{
		private SaveDataType mSaveType;
		private String[] mAutofillHints;
		private AutofillId mAutofillId;
		private AutofillType mAutofillType;
		private string[] mAutofillOptions;
		private bool mFocused;

		public AutofillFieldMetadata(AssistStructure.ViewNode view)
		{
			mAutofillId = view.AutofillId;
			mAutofillType = view.AutofillType;
			mAutofillOptions = view.GetAutofillOptions();
			mFocused = view.IsFocused;
			var hints = AutofillHints.FilterForSupportedHints(view.GetAutofillHints());
			if (hints != null)
			{
				AutofillHints.ConvertToStoredHintNames(hints);
				SetHints(hints);
			}
		}

		public string[] GetHints()
		{
			return mAutofillHints;
		}

		public void SetHints(string[] hints)
		{
			mAutofillHints = hints;
			mSaveType = AutofillHints.GetSaveTypeForHints(hints);
		}

		public SaveDataType GetSaveType()
		{
			return mSaveType;
		}

		public AutofillId GetId()
		{
			return mAutofillId;
		}

		public AutofillType GetAutofillType()
		{
			return mAutofillType;
		}

		/**
		* When the {@link ViewNode} is a list that the user needs to choose a string from (i.e. a
		* spinner), this is called to return the index of a specific item in the list.
		*/
		public int GetAutofillOptionIndex(string value)
		{
			for (int i = 0; i < mAutofillOptions.Length; i++)
			{
				if (mAutofillOptions[i] == value)
				{
					return i;
				}
			}

			return -1;
		}

		public bool IsFocused()
		{
			return mFocused;
		}
	}
}