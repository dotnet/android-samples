using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutofillService.Model;

namespace AutofillService.Data.Source
{
	public interface IAutofillDataSource
	{

		/**
		 * Asynchronously gets saved list of {@link DatasetWithFilledAutofillFields} that contains some
		 * objects that can autofill fields with these {@code autofillHints}.
		 */
		void GetAutofillDatasets(List<String> allAutofillHints,
			IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback);

		void GetAllAutofillDatasets(
			IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback);

		/**
		 * Asynchronously gets a saved {@link DatasetWithFilledAutofillFields} for a specific
		 * {@code datasetName} that contains some objects that can autofill fields with these
		 * {@code autofillHints}.
		 */
		void GetAutofillDataset(List<String> allAutofillHints,
			String datasetName, IDataCallback<DatasetWithFilledAutofillFields> datasetsCallback);

		/**
		 * Stores a collection of Autofill fields.
		 */
		void SaveAutofillDatasets(ImmutableList<DatasetWithFilledAutofillFields>
			datasetsWithFilledAutofillFields);

		void SaveResourceIdHeuristic(ResourceIdHeuristic resourceIdHeuristic);

		/**
		 * Gets all autofill field types.
		 */
		void GetFieldTypes(IDataCallback<List<FieldTypeWithHeuristics>> fieldTypesCallback);

		/**
		 * Gets all autofill field types.
		 */
		void GetFieldType(String typeName, IDataCallback<FieldType> fieldTypeCallback);

		void GetFieldTypeByAutofillHints(
			IDataCallback<Dictionary<string, FieldTypeWithHeuristics>> fieldTypeMapCallback);

		void GetFilledAutofillField(String datasetId, String fieldTypeName, IDataCallback<FilledAutofillField> fieldCallback);

		/**
		 * Clears all data.
		 */
		void Clear();
	}
}