using System;
using System.Collections.Generic;
using AutofillFramework.multidatasetservice.model;

namespace AutofillFramework.multidatasetservice.datasource
{
	public interface IAutofillRepository
	{
		/**
     	 * Gets saved FilledAutofillFieldCollection that contains some objects that can autofill fields with these
     	 * {@code autofillHints}.
      	 */
		Dictionary<string, FilledAutofillFieldCollection> GetFilledAutofillFieldCollection(List<string> focusedAutofillHints,
				List<string> allAutofillHints);

		/**
     	 * Saves LoginCredential under this datasetName.
     	 */
		void SaveFilledAutofillFieldCollection(FilledAutofillFieldCollection filledAutofillFieldCollection);

		/**
		 * Clears all data.
		 */
		void Clear();
	}
}
