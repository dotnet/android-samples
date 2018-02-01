using System;
using System.Collections.Generic;
using Android.Content;
using AutofillService.Model;

namespace AutofillService.Datasource
{
	public interface IAutofillDataSource
	{
		/**
		 * Gets saved FilledAutofillFieldCollection that contains some objects that can autofill fields
		 * with these {@code autofillHints}.
		 */
		Dictionary<string, FilledAutofillFieldCollection> GetFilledAutofillFieldCollection(Context context,
			List<String> focusedAutofillHints, List<String> allAutofillHints);

		/**
		 * Stores a collection of Autofill fields.
		 */
		void SaveFilledAutofillFieldCollection(Context context,
			FilledAutofillFieldCollection filledAutofillFieldCollection);

		/**
		 * Clears all data.
		 */
		void Clear(Context context);
	}
}