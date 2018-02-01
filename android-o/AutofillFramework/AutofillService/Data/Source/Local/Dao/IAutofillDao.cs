using System;
using System.Collections.Generic;
using AutofillService.Model;

namespace AutofillService.Data.Source.Local.Dao
{
	//@Dao
	public interface IAutofillDao
	{
		/**
		 * Fetches a list of datasets associated to autofill fields on the page.
		 *
		 * @param allAutofillHints Filtering parameter; represents all of the hints associated with
		 *                         all of the views on the page.
		 */
		//@Query("SELECT DISTINCT id, datasetName FROM FilledAutofillField, AutofillDataset" +
		//    " WHERE AutofillDataset.id = FilledAutofillField.datasetId" +
		//    " AND FilledAutofillField.fieldTypeName IN (:allAutofillHints)")

		List<DatasetWithFilledAutofillFields> GetDatasets(List<String> allAutofillHints);

		//@Query("SELECT DISTINCT id, datasetName FROM FilledAutofillField, AutofillDataset" +
		//    " WHERE AutofillDataset.id = FilledAutofillField.datasetId")

		List<DatasetWithFilledAutofillFields> GetAllDatasets();

		/**
		 * Fetches a list of datasets associated to autofill fields. It should only return a dataset
		 * if that dataset has an autofill field associate with the view the user is focused on, and
		 * if that dataset's name matches the name passed in.
		 *
		 * @param fieldTypes Filtering parameter; represents all of the field types associated with
		 *                         all of the views on the page.
		 * @param datasetName      Filtering parameter; only return datasets with this name.
		 */
		//@Query("SELECT DISTINCT id, datasetName FROM FilledAutofillField, AutofillDataset" +
		//    " WHERE AutofillDataset.id = FilledAutofillField.datasetId" +
		//    " AND AutofillDataset.datasetName = (:datasetName)" +
		//    " AND FilledAutofillField.fieldTypeName IN (:fieldTypes)")

		List<DatasetWithFilledAutofillFields> GetDatasetsWithName(
				List<String> fieldTypes, String datasetName);

		//@Query("SELECT DISTINCT typeName, autofillTypes, saveInfo, partition, strictExampleSet, " +
		//    "textTemplate, dateTemplate" +
		//   " FROM FieldType, AutofillHint" +
		//    " WHERE FieldType.typeName = AutofillHint.fieldTypeName" +
		//    " UNION " +
		//    "SELECT DISTINCT typeName, autofillTypes, saveInfo, partition, strictExampleSet, " +
		//    "textTemplate, dateTemplate" +
		//    " FROM FieldType, ResourceIdHeuristic" +
		//    " WHERE FieldType.typeName = ResourceIdHeuristic.fieldTypeName")

		List<FieldTypeWithHeuristics> GetFieldTypesWithHints();

		//@Query("SELECT DISTINCT typeName, autofillTypes, saveInfo, partition, strictExampleSet, " +
		//    "textTemplate, dateTemplate" +
		//    " FROM FieldType, AutofillHint, ResourceIdHeuristic" +
		//    " WHERE FieldType.typeName = AutofillHint.fieldTypeName" +
		//    " AND AutofillHint.autofillHint IN (:autofillHints)" +
		//    " UNION " +
		//    "SELECT DISTINCT typeName, autofillTypes, saveInfo, partition, strictExampleSet, " +
		//    "textTemplate, dateTemplate" +
		//    " FROM FieldType, ResourceIdHeuristic" +
		//    " WHERE FieldType.typeName = ResourceIdHeuristic.fieldTypeName")

		List<FieldTypeWithHeuristics> GetFieldTypesForAutofillHints(List<String> autofillHints);

		//@Query("SELECT DISTINCT id, datasetName FROM FilledAutofillField, AutofillDataset" +
		//    " WHERE AutofillDataset.id = FilledAutofillField.datasetId" +
		//    " AND AutofillDataset.id = (:datasetId)")

		DatasetWithFilledAutofillFields GetAutofillDatasetWithId(String datasetId);

		//@Query("SELECT * FROM FilledAutofillField" +
		//    " WHERE FilledAutofillField.datasetId = (:datasetId)" +
		//    " AND FilledAutofillField.fieldTypeName = (:fieldTypeName)")

		FilledAutofillField GetFilledAutofillField(String datasetId, String fieldTypeName);

		//@Query("SELECT * FROM FieldType" +
		//    " WHERE FieldType.typeName = (:fieldTypeName)")
		//
		FieldType GetFieldType(String fieldTypeName);

		/**
		 * @param autofillFields Collection of autofill fields to be saved to the db.
		 */
		//@Insert(onConflict = OnConflictStrategy.REPLACE)

		void InsertFilledAutofillFields(ICollection<FilledAutofillField> autofillFields);

		//@Insert(onConflict = OnConflictStrategy.REPLACE)

		void InsertAutofillDataset(AutofillDataset datasets);

		//@Insert(onConflict = OnConflictStrategy.REPLACE)

		void InsertAutofillHints(List<AutofillHint> autofillHints);

		//@Insert(onConflict = OnConflictStrategy.REPLACE)

		void InsertResourceIdHeuristic(ResourceIdHeuristic resourceIdHeuristic);

		//@Insert(onConflict = OnConflictStrategy.REPLACE)

		void InsertFieldTypes(List<FieldType> fieldTypes);


		//@Query("DELETE FROM AutofillDataset")

		void ClearAll();
	}
}