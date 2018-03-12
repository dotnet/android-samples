using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Android.App.Assist;
using Android.Support.Annotation;
using Android.Views;
using AutofillService.Model;
using Java.Lang;
using String = System.String;

namespace AutofillService.Data
{
	public class ClientAutofillDataBuilder : IAutofillDataBuilder
	{

		private ClientParser mClientParser;
		private Dictionary<string, FieldTypeWithHeuristics> mFieldTypesByAutofillHint;
		private String mPackageName;

		public ClientAutofillDataBuilder(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
			String packageName, ClientParser clientParser)
		{
			mClientParser = clientParser;
			mFieldTypesByAutofillHint = fieldTypesByAutofillHint;
			mPackageName = packageName;
		}

		public ImmutableList<DatasetWithFilledAutofillFields> BuildDatasetsByPartition(int datasetNumber)
		{
			//Guava
			var listBuilder = ImmutableList.Create<DatasetWithFilledAutofillFields>();
			foreach (int partition in AutofillHints.PARTITIONS)
			{
				AutofillDataset autofillDataset = new AutofillDataset(Guid.NewGuid().ToString(),
					"dataset-" + datasetNumber + "." + partition);
				DatasetWithFilledAutofillFields dataset =
					BuildDatasetForPartition(autofillDataset, partition);
				if (dataset != null && dataset.filledAutofillFields != null)
				{
					listBuilder.Add(dataset);
				}
			}
			return listBuilder;
		}

		/**
     * Parses a client view structure and build a dataset (in the form of a
     * {@link DatasetWithFilledAutofillFields}) from the view metadata found.
     */
		private DatasetWithFilledAutofillFields BuildDatasetForPartition(AutofillDataset dataset,
			int partition)
		{
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields = new DatasetWithFilledAutofillFields();
			datasetWithFilledAutofillFields.autofillDataset = dataset;
			mClientParser.Parse(new PartitionNodeProcessor
			{
				that = this,
				partition = partition,
				datasetWithFilledAutofillFields = datasetWithFilledAutofillFields
			});
			return datasetWithFilledAutofillFields;

		}

		public class PartitionNodeProcessor : Java.Lang.Object, ClientParser.INodeProcessor
		{
			public int partition;
			public ClientAutofillDataBuilder that;
			public DatasetWithFilledAutofillFields datasetWithFilledAutofillFields;
			public void ProcessNode(AssistStructure.ViewNode node)
			{
				that.ParseAutofillFields(node, datasetWithFilledAutofillFields, partition);
			}
		}

		private void ParseAutofillFields(AssistStructure.ViewNode viewNode,
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields, int partition)
		{
			string[] hints = viewNode.GetAutofillHints();
			if (hints == null || hints.Length == 0)
			{
				return;
			}
			var autofillValue = viewNode.AutofillValue;
			string textValue = null;
			long dateValue = Long.MinValue;
			bool toggleValue = false;
			string[] autofillOptions = null;
			int listIndex = Int32.MinValue;

			if (autofillValue != null)
			{
				if (autofillValue.IsText)
				{
					// Using toString of AutofillValue.getTextValue in order to save it to
					// SharedPreferences.
					textValue = autofillValue.TextValue;
				}
				else if (autofillValue.IsDate)
				{
					dateValue = autofillValue.DateValue;
				}
				else if (autofillValue.IsList)
				{
					autofillOptions = viewNode.GetAutofillOptions();
					listIndex = autofillValue.ListValue;
				}
				else if (autofillValue.IsToggle)
				{
					toggleValue = autofillValue.ToggleValue;
				}
			}
			AppendViewMetadata(datasetWithFilledAutofillFields, hints, partition, textValue, dateValue, toggleValue, autofillOptions, listIndex);
		}
		private void AppendViewMetadata([NonNull] DatasetWithFilledAutofillFields

			datasetWithFilledAutofillFields, [NonNull] string[] hints, int partition,
			[Nullable] string textValue, [Nullable] long dateValue, [Nullable] bool toggleValue,
			[Nullable] string[] autofillOptions, [Nullable] int listIndex)
		{
			for (int i = 0; i < hints.Length; i++)
			{
				String hint = hints[i];
				// Then check if the "actual" hint is supported.
				FieldTypeWithHeuristics fieldTypeWithHeuristics = mFieldTypesByAutofillHint[hint];
				if (fieldTypeWithHeuristics != null)
				{
					FieldType fieldType = fieldTypeWithHeuristics.fieldType;
					if (!AutofillHints.MatchesPartition(fieldType.GetPartition(), partition))
					{
						continue;
					}
					// Only add the field if the hint is supported by the type.
					if (textValue != null)
					{
						if (!fieldType.GetAutofillTypes().ints.Contains((int)AutofillType.Text))
						{
							Util.Loge("Text is invalid type for hint '%s'", hint);
						}
					}
					if (autofillOptions != null && listIndex != null && autofillOptions.Length > listIndex)
					{
						if (!fieldType.GetAutofillTypes().ints.Contains((int)AutofillType.List))
						{
							Util.Loge("List is invalid type for hint '%s'", hint);
						}
						textValue = autofillOptions[listIndex];
					}
					if (dateValue != null)
					{
						if (!fieldType.GetAutofillTypes().ints.Contains((int)AutofillType.Date))
						{
							Util.Loge("Date is invalid type for hint '%s'", hint);
						}
					}
					if (toggleValue != null)
					{
						if (!fieldType.GetAutofillTypes().ints.Contains((int)AutofillType.Toggle))
						{
							Util.Loge("Toggle is invalid type for hint '%s'", hint);
						}
					}
					String datasetId = datasetWithFilledAutofillFields.autofillDataset.GetId();
					datasetWithFilledAutofillFields.Add(new FilledAutofillField(datasetId,
							mPackageName, fieldType.GetTypeName(), textValue, dateValue, toggleValue));
				}
				else
				{
					Util.Loge("Invalid hint: %s", hint);
				}
			}
		}
	}
}