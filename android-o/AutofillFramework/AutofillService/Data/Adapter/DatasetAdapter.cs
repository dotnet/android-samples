using System.Collections.Generic;
using System.Linq;
using Android.App.Assist;
using Android.Content;
using Android.Service.Autofill;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AutofillService.Model;
using Java.Lang;
using Java.Util.Functions;

namespace AutofillService.Data.Adapter
{
	public class DatasetAdapter
	{
		private ClientParser mClientParser;

		public DatasetAdapter(ClientParser clientParser)
		{
			mClientParser = clientParser;
		}

		/**
		 * Wraps autofill data in a {@link Dataset} object which can then be sent back to the client.
		 */
		public Dataset BuildDataset(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields,
			RemoteViews remoteViews)
		{
			return BuildDataset(fieldTypesByAutofillHint, datasetWithFilledAutofillFields, remoteViews, null);
		}

		public Dataset BuildDatasetForFocusedNode(FilledAutofillField filledAutofillField, FieldType fieldType,
			RemoteViews remoteViews)
		{
			Dataset.Builder datasetBuilder = new Dataset.Builder(remoteViews);
			bool setAtLeastOneValue = BindDatasetToFocusedNode(filledAutofillField, fieldType, datasetBuilder);
			if (!setAtLeastOneValue)
			{
				return null;
			}
			return datasetBuilder.Build();
		}

		/**
		 * Wraps autofill data in a {@link Dataset} object with an IntentSender, which can then be
		 * sent back to the client.
		 */
		public Dataset BuildDataset(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields,
			RemoteViews remoteViews, IntentSender intentSender)
		{
			Dataset.Builder datasetBuilder = new Dataset.Builder(remoteViews);
			if (intentSender != null)
			{
				datasetBuilder.SetAuthentication(intentSender);
			}
			bool setAtLeastOneValue = BindDataset(fieldTypesByAutofillHint,
				datasetWithFilledAutofillFields, datasetBuilder);
			if (!setAtLeastOneValue)
			{
				return null;
			}
			return datasetBuilder.Build();
		}

		/**
		 * Build an autofill {@link Dataset} using saved data and the client's AssistStructure.
		 */
		private bool BindDataset(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields,
			Dataset.Builder datasetBuilder)
		{
			MutableBoolean setValueAtLeastOnce = new MutableBoolean(false);

			var filledAutofillFieldsByTypeName = 
				datasetWithFilledAutofillFields
				.filledAutofillFields
				.ToDictionary(x => x.GetFieldTypeName(), x => x);

			mClientParser.Parse(new BindDatasetNodeProcessor
			{
				datasetBuilder = datasetBuilder,
				setValueAtLeastOnce = setValueAtLeastOnce,
				fieldTypesByAutofillHint = fieldTypesByAutofillHint,
				filledAutofillFieldsByTypeName = filledAutofillFieldsByTypeName,
				datasetAdapter = this

			});

			return setValueAtLeastOnce.Value;
		}

		private bool BindDatasetToFocusedNode(FilledAutofillField field, FieldType fieldType, Dataset.Builder builder)
		{
			MutableBoolean setValueAtLeastOnce = new MutableBoolean(false);
			mClientParser.Parse(new BindDatasetToFocusedNodeProcessor()
			{
				field = field,
				builder = builder,
				datasetAdapter = this,
				setValueAtLeastOnce = setValueAtLeastOnce
			});
			return setValueAtLeastOnce.Value;
		}

		public class BindDatasetNodeProcessor : ClientParser.INodeProcessor
		{
			public Dataset.Builder datasetBuilder;
			public MutableBoolean setValueAtLeastOnce;
			public Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint;
			public Dictionary<string, FilledAutofillField> filledAutofillFieldsByTypeName;

			public DatasetAdapter datasetAdapter;

			public void ProcessNode(AssistStructure.ViewNode node)
			{
				datasetAdapter.ParseAutofillFields(node, fieldTypesByAutofillHint, filledAutofillFieldsByTypeName,
					datasetBuilder, setValueAtLeastOnce);
			}
		}

		public class BindDatasetToFocusedNodeProcessor : ClientParser.INodeProcessor
		{
			public FilledAutofillField field;
			public Dataset.Builder builder;
			public DatasetAdapter datasetAdapter;
			public MutableBoolean setValueAtLeastOnce;

			public void ProcessNode(AssistStructure.ViewNode node)
			{
				if (node.IsFocused && node.AutofillId != null)
				{
					datasetAdapter.BindValueToNode(node, field, builder, setValueAtLeastOnce);
				}
			}
		}

		private void ParseAutofillFields(AssistStructure.ViewNode viewNode,
			Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
			Dictionary<string, FilledAutofillField> filledAutofillFieldsByTypeName,
			Dataset.Builder builder, MutableBoolean setValueAtLeastOnce)
		{
			var rawHints = viewNode.GetAutofillHints();
			if (rawHints == null || rawHints.Length == 0)
			{
				Util.Logv("No af hints at ViewNode - %s", viewNode.IdEntry);
				return;
			}
			string fieldTypeName = AutofillHints.GetFieldTypeNameFromAutofillHints(
				fieldTypesByAutofillHint, rawHints.ToList());
			if (fieldTypeName == null)
			{
				return;
			}
			FilledAutofillField field = filledAutofillFieldsByTypeName[fieldTypeName];
			if (field == null)
			{
				return;
			}
			BindValueToNode(viewNode, field, builder, setValueAtLeastOnce);
		}

		void BindValueToNode(AssistStructure.ViewNode viewNode,
			FilledAutofillField field, Dataset.Builder builder,
			MutableBoolean setValueAtLeastOnce)
		{
			AutofillId autofillId = viewNode.AutofillId;
			if (autofillId == null)
			{
				Util.Logw("Autofill ID null for %s", viewNode.ToString());
				return;
			}
			int autofillType = (int)viewNode.AutofillType;
			switch (autofillType)
			{
				case (int)AutofillType.List:
					var options = viewNode.GetAutofillOptions();
					int listValue = -1;
					if (options != null)
					{
						listValue = Util.IndexOf(viewNode.GetAutofillOptions(), field.GetTextValue());
					}
					if (listValue != -1)
					{
						builder.SetValue(autofillId, AutofillValue.ForList(listValue));
						setValueAtLeastOnce.Value = true;
					}
					break;
				case (int)AutofillType.Date:
					var dateValue = field.GetDateValue();
					if (dateValue != null)
					{
						builder.SetValue(autofillId, AutofillValue.ForDate(dateValue));
						setValueAtLeastOnce.Value = true;
					}
					break;
				case (int)AutofillType.Text:
					string textValue = field.GetTextValue();
					if (textValue != null)
					{
						builder.SetValue(autofillId, AutofillValue.ForText(textValue));
						setValueAtLeastOnce.Value = true;
					}
					break;
				case (int)AutofillType.Toggle:
					var toggleValue = field.GetToggleValue();
					if (toggleValue != null)
					{
						builder.SetValue(autofillId, AutofillValue.ForToggle(toggleValue));
						setValueAtLeastOnce.Value = true;
					}
					break;
				case (int)AutofillType.None:
					break;
				default:
					Util.Logw("Invalid autofill type - %d", autofillType);
					break;
			}
		}
	}
}