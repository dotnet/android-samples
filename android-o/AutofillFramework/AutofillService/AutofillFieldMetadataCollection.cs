using System;
using System.Collections.Generic;
using System.Linq;
using Android.Service.Autofill;
using Android.Views.Autofill;

namespace AutofillService
{
    public class AutofillFieldMetadataCollection
    {
        private List<AutofillId> mAutofillIds = new List<AutofillId>();

        private Dictionary<string, List<AutofillFieldMetadata>> mAutofillHintsToFieldsMap =
            new Dictionary<string, List<AutofillFieldMetadata>>();

        private List<string> mAllAutofillHints = new List<string>();
        private List<string> mFocusedAutofillHints = new List<string>();
        private int mSize;
        private SaveDataType mSaveType;

        public void Add(AutofillFieldMetadata autofillFieldMetadata)
        {
            mSaveType |= autofillFieldMetadata.GetSaveType();
            mSize++;
            mAutofillIds.Add(autofillFieldMetadata.GetId());
            var hintsList = autofillFieldMetadata.GetHints().ToArray();
            mAllAutofillHints.AddRange(hintsList);
            if (autofillFieldMetadata.IsFocused())
            {
                mFocusedAutofillHints.AddRange(hintsList);
            }

            foreach (var hint in autofillFieldMetadata.GetHints())
            {
                if (!mAutofillHintsToFieldsMap.ContainsKey(hint))
                {
                    mAutofillHintsToFieldsMap.Add(hint, new List<AutofillFieldMetadata>());
                }

                mAutofillHintsToFieldsMap[hint].Add(autofillFieldMetadata);
            }
        }

        public SaveDataType GetSaveType()
        {
            return mSaveType;
        }

        public AutofillId[] GetAutofillIds()
        {
            return mAutofillIds.ToArray();
        }

        public List<AutofillFieldMetadata> GetFieldsForHint(string hint)
        {
            return mAutofillHintsToFieldsMap[hint];
        }

        public List<string> GetFocusedHints()
        {
            return mFocusedAutofillHints;
        }

        public List<String> GetAllHints()
        {
            return mAllAutofillHints;
        }
    }
}