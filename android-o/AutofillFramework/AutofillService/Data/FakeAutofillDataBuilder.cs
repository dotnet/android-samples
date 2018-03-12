using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutofillService.Model;

namespace AutofillService.Data
{
	public class FakeAutofillDataBuilder : IAutofillDataBuilder
	{

	private List<FieldTypeWithHeuristics> mFieldTypesWithHints;
	private string mPackageName;
	private int mSeed;

	public FakeAutofillDataBuilder(List<FieldTypeWithHeuristics> fieldTypesWithHints,
		String packageName, int seed)
	{
		mFieldTypesWithHints = fieldTypesWithHints;
		mSeed = seed;
		mPackageName = packageName;
	}

		public ImmutableList<DatasetWithFilledAutofillFields> BuildDatasetsByPartition(int datasetNumber)
		{
			var listBuilder = ImmutableList.Create<DatasetWithFilledAutofillFields>();
			foreach (int partition in AutofillHints.PARTITIONS)
			{
				AutofillDataset autofillDataset = new AutofillDataset(Guid.NewGuid().ToString(),
					"dataset-" + datasetNumber + "." + partition);
				DatasetWithFilledAutofillFields datasetWithFilledAutofillFields =
					BuildCollectionForPartition(autofillDataset, partition);
				if (datasetWithFilledAutofillFields != null &&
				    datasetWithFilledAutofillFields.filledAutofillFields != null &&
				    datasetWithFilledAutofillFields.filledAutofillFields.Count != 0)
				{
					listBuilder.Add(datasetWithFilledAutofillFields);
				}
			}
			return listBuilder;
		}

		private DatasetWithFilledAutofillFields BuildCollectionForPartition(
			AutofillDataset dataset, int partition)
		{
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields =
				new DatasetWithFilledAutofillFields();
			datasetWithFilledAutofillFields.autofillDataset = dataset;
			foreach (var fieldTypeWithHeuristics in mFieldTypesWithHints)
			{
				if (AutofillHints.MatchesPartition(
					fieldTypeWithHeuristics.getFieldType().GetPartition(), partition))
				{
					var fakeField = AutofillHints.generateFakeField(fieldTypeWithHeuristics, mPackageName,
							mSeed, dataset.GetId());
					datasetWithFilledAutofillFields.Add(fakeField);
				}
			}
			return datasetWithFilledAutofillFields;
		}
	}
}