using System;
using System.Collections.Generic;
using Android.Service.Autofill;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Java.Lang;

namespace AutofillFramework.multidatasetservice.model
{
	/**
 	 * FilledAutofillFieldCollection is the model that holds all of the data on a client app's page,
 	 * plus the dataset name associated with it.
 	 */
	public class FilledAutofillFieldCollection
	{
		// @Expose
		public Dictionary<string, FilledAutofillField> HintMap { get; }
		// @Expose
		public string DatasetName { get; set; }

		public FilledAutofillFieldCollection(string datasetName, Dictionary<string, FilledAutofillField> hintMap)
		{
			HintMap = hintMap;
			DatasetName = datasetName;
		}

		public FilledAutofillFieldCollection() : this(null, new Dictionary<string, FilledAutofillField>()) {}

		/**
     	 * Adds a {@code FilledAutofillField} to the collection, indexed by all of its hints.
     	 */
		public void Add(FilledAutofillField filledAutofillField)
		{
			string[] autofillHints = filledAutofillField.AutofillHints;
			foreach (string hint in autofillHints)
			{
				HintMap.Add(hint, filledAutofillField);
			}
		}

		/**
	     * Populates a {@link Dataset.Builder} with appropriate values for each {@link AutofillId}
	     * in a {@code AutofillFieldMetadataCollection}.
	     *
	     * In other words, it constructs an autofill
	     * {@link Dataset.Builder} by applying saved values (from this {@code FilledAutofillFieldCollection})
	     * to Views specified in a {@code AutofillFieldMetadataCollection}, which represents the current
	     * page the user is on.
	     */
		public bool ApplyToFields(AutofillFieldMetadataCollection autofillFieldMetadataCollection,
			Dataset.Builder datasetBuilder)
		{
			bool setValueAtLeastOnce = false;
			List<string> allHints = autofillFieldMetadataCollection.AllAutofillHints;
			for (int hintIndex = 0; hintIndex < allHints.Count; hintIndex++)
			{
				string hint = allHints[hintIndex];
				List<AutofillFieldMetadata> fillableAutofillFields = autofillFieldMetadataCollection.GetFieldsForHint(hint);
				if (fillableAutofillFields == null)
				{
					continue;
				}
				for (int autofillFieldIndex = 0; autofillFieldIndex < fillableAutofillFields.Count; autofillFieldIndex++)
				{
					FilledAutofillField filledAutofillField = HintMap[hint];
					if (filledAutofillField == null)
					{
						continue;
					}
					AutofillFieldMetadata autofillFieldMetadata = fillableAutofillFields[autofillFieldIndex];
					var autofillId = autofillFieldMetadata.AutofillId;
					var autofillType = autofillFieldMetadata.AutofillType;
					switch (autofillType)
					{
						case (int) AutofillType.List:
							var listValue = autofillFieldMetadata.GetAutofillOptionIndex(filledAutofillField.TextValue);
							if (listValue != -1)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForList(listValue));
								setValueAtLeastOnce = true;
							}
							break;
						case (int) AutofillType.Date:
							var dateValue = filledAutofillField.DateValue;
							datasetBuilder.SetValue(autofillId, AutofillValue.ForDate(dateValue));
							setValueAtLeastOnce = true;
							break;
						case (int) AutofillType.Text:
							var textValue = filledAutofillField.TextValue;
							if (textValue != null)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForText(textValue));
								setValueAtLeastOnce = true;
							}
							break;
						case (int) AutofillType.Toggle:
							var toggleValue = filledAutofillField.ToggleValue;
							if (toggleValue != null)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForToggle(toggleValue.BooleanValue()));
								setValueAtLeastOnce = true;
							}
							break;
						default:
							Log.Warn(CommonUtil.Tag, "Invalid autofill type - " + autofillType);
							break;
					}
				}
			}
			return setValueAtLeastOnce;
		}

		/**
     	 * Takes in a list of autofill hints (`autofillHints`), usually associated with a View or set of
     	 * Views. Returns whether any of the filled fields on the page have at least 1 of these
     	 * `autofillHint`s.
     	 */
		public bool HelpsWithHints(List<string> autofillHints)
		{
			for (int i = 0; i < autofillHints.Count; i++)
			{
				var autofillHint = autofillHints[i];
				if (HintMap.ContainsKey(autofillHint) && !HintMap[autofillHint].IsNull())
				{
					return true;
				}
			}
			return false;
		}

		public static implicit operator FilledAutofillFieldCollection(Java.Lang.Object v)
		{
			throw new NotImplementedException();
		}
	}
}
