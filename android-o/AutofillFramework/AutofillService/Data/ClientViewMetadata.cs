using System;
using System.Collections.Generic;
using Android.Views.Autofill;
using Java.Util;

namespace AutofillService.Data.Source
{
	public class ClientViewMetadata
	{
		public List<String> mAllHints { get; }
		public int mSaveType { get; }
		public AutofillId[] mAutofillIds { get; }
		public String mWebDomain { get; }
		public AutofillId[] mFocusedIds { get; }

		public ClientViewMetadata(List<string> allHints, int saveType, AutofillId[] autofillIds,
			AutofillId[] focusedIds, string webDomain)
		{
			mAllHints = allHints;
			mSaveType = saveType;
			mAutofillIds = autofillIds;
			mWebDomain = webDomain;
			mFocusedIds = focusedIds;
		}

		public override string ToString()
		{
			return "ClientViewMetadata{" +
						  "mAllHints=" + mAllHints +
						  ", mSaveType=" + mSaveType +
						  ", mAutofillIds=" + Arrays.ToString(mAutofillIds) +
						  ", mWebDomain='" + mWebDomain + '\'' +
						  ", mFocusedIds=" + Arrays.ToString(mFocusedIds) +
						  '}';
		}
	}
}