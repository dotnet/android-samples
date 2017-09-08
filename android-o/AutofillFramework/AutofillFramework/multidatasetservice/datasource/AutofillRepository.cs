using System.Collections.Generic;
using AutofillFramework.multidatasetservice.model;

namespace AutofillFramework.multidatasetservice.datasource
{
	public interface IAutofillRepository
	{
		/// <summary>
		/// Gets saved FilledAutofillFieldCollection that contains some objects that can autofill fields with these
		/// {@code autofillHints}.
		/// </summary>
		/// <returns>The filled autofill field collection.</returns>
		/// <param name="focusedAutofillHints">Focused autofill hints.</param>
		/// <param name="allAutofillHints">All autofill hints.</param>
		Dictionary<string, FilledAutofillFieldCollection> GetFilledAutofillFieldCollection(List<string> focusedAutofillHints,
				List<string> allAutofillHints);

		/// <summary>
		/// Saves LoginCredential under this datasetName.
		/// </summary>
		/// <param name="filledAutofillFieldCollection">Filled autofill field collection.</param>
		void SaveFilledAutofillFieldCollection(FilledAutofillFieldCollection filledAutofillFieldCollection);

		/// <summary>
		/// Clears all data.
		/// </summary>
		void Clear();
	}
}
