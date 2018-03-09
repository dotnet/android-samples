using System.Collections.Generic;
using System.Linq;
using Android.Service.Autofill;
using Android.Support.Annotation;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Newtonsoft.Json;

namespace AutofillService.Model
{
	[JsonObject(MemberSerialization.OptIn)]
	public class FilledAutofillFieldCollection
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, FilledAutofillField> mHintMap { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string mDatasetName { get; set; }

		[JsonConstructor]
		public FilledAutofillFieldCollection() : this(null, new Dictionary<string, FilledAutofillField>())
		{
		}

		public FilledAutofillFieldCollection(string datasetName,
			Dictionary<string, FilledAutofillField> hintMap)
		{
			mHintMap = hintMap;
			mDatasetName = datasetName;
		}


		private static bool IsW3CSectionPrefix(string hint)
		{
			return hint.StartsWith(W3cHints.PREFIX_SECTION);
		}

		private static bool IsW3CAddressType(string hint)
		{
			switch (hint)
			{
				case W3cHints.SHIPPING:
				case W3cHints.BILLING:
					return true;
			}

			return false;
		}

		private static bool IsW3CTypePrefix(string hint)
		{
			switch (hint)
			{
				case W3cHints.PREFIX_WORK:
				case W3cHints.PREFIX_FAX:
				case W3cHints.PREFIX_HOME:
				case W3cHints.PREFIX_PAGER:
					return true;
			}

			return false;
		}

		private static bool IsW3CTypeHint(string hint)
		{
			switch (hint)
			{
				case W3cHints.TEL:
				case W3cHints.TEL_COUNTRY_CODE:
				case W3cHints.TEL_NATIONAL:
				case W3cHints.TEL_AREA_CODE:
				case W3cHints.TEL_LOCAL:
				case W3cHints.TEL_LOCAL_PREFIX:
				case W3cHints.TEL_LOCAL_SUFFIX:
				case W3cHints.TEL_EXTENSION:
				case W3cHints.EMAIL:
				case W3cHints.IMPP:
					return true;
			}

			Log.Warn(CommonUtil.TAG, "Invalid W3C type hint: " + hint);
			return false;
		}

		/**
		 * Returns the name of the {@link Dataset}.
		 */
		public string GetDatasetName()
		{
			return mDatasetName;
		}

		/**
		 * Sets the {@link Dataset} name.
		 */
		public void SetDatasetName(string datasetName)
		{
			mDatasetName = datasetName;
		}

		/**
		* Adds a {@code FilledAutofillField} to the collection, indexed by all of its hints.
		*/
		public void Add([NonNull] FilledAutofillField filledAutofillField)
		{
			string[] autofillHints = filledAutofillField.GetAutofillHints();
			string nextHint = null;
			for (int i = 0; i < autofillHints.Length; i++)
			{
				string hint = autofillHints[i];
				if (i < autofillHints.Length - 1)
				{
					nextHint = autofillHints[i + 1];
				}

				// First convert the compound W3C autofill hints
				if (IsW3CSectionPrefix(hint) && i < autofillHints.Length - 1)
				{
					hint = autofillHints[++i];
					if (CommonUtil.DEBUG)
						Log.Debug(CommonUtil.TAG, "Hint is a W3C section prefix; using " + hint + " instead");
					if (i < autofillHints.Length - 1)
					{
						nextHint = autofillHints[i + 1];
					}
				}

				if (IsW3CTypePrefix(hint) && nextHint != null && IsW3CTypeHint(nextHint))
				{
					hint = nextHint;
					i++;
					if (CommonUtil.DEBUG)
						Log.Debug(CommonUtil.TAG, "Hint is a W3C type prefix; using " + hint + " instead");
				}

				if (IsW3CAddressType(hint) && nextHint != null)
				{
					hint = nextHint;
					i++;
					if (CommonUtil.DEBUG)
						Log.Debug(CommonUtil.TAG, "Hint is a W3C address prefix; using " + hint + " instead");
				}

				// Then check if the "actual" hint is supported.


				if (AutofillHints.IsValidHint(hint))
				{
					mHintMap[hint] = filledAutofillField;
				}
				else
				{
					Log.Error(CommonUtil.TAG, "Invalid hint: " + autofillHints[i]);
				}
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
			List<string> allHints = autofillFieldMetadataCollection.GetAllHints();
			for (var hintIndex = 0; hintIndex < allHints.Count; hintIndex++)
			{
				string hint = allHints[hintIndex];
				var fillableAutofillFields = autofillFieldMetadataCollection.GetFieldsForHint(hint);
				if (fillableAutofillFields == null)
				{
					continue;
				}

				for (var autofillFieldIndex = 0;
					autofillFieldIndex < fillableAutofillFields.Count;
					autofillFieldIndex++)
				{
					var filledAutofillField = mHintMap.FirstOrDefault(x => x.Key == hint);
					if (filledAutofillField.Value == null)
					{
						continue;
					}

					var autofillFieldMetadata = fillableAutofillFields[autofillFieldIndex];
					AutofillId autofillId = autofillFieldMetadata.GetId();
					var autofillType = autofillFieldMetadata.GetAutofillType();
					switch (autofillType)
					{
						case AutofillType.List:
							int listValue =
								autofillFieldMetadata.GetAutofillOptionIndex(filledAutofillField.Value.GetTextValue());
							if (listValue != -1)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForList(listValue));
								setValueAtLeastOnce = true;
							}

							break;
						case AutofillType.Date:
							var dateValue = filledAutofillField.Value.GetDateValue();
							if (dateValue != null)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForDate(dateValue));
								setValueAtLeastOnce = true;
							}

							break;
						case AutofillType.Text:
							var textValue = filledAutofillField.Value.GetTextValue();
							if (textValue != null)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForText(textValue));
								setValueAtLeastOnce = true;
							}

							break;
						case AutofillType.Toggle:
							var toggleValue = filledAutofillField.Value.GetToggleValue();
							if (toggleValue != null)
							{
								datasetBuilder.SetValue(autofillId, AutofillValue.ForToggle(toggleValue));
								setValueAtLeastOnce = true;
							}

							break;
						case  AutofillType.None:
							break;
						default:
							Log.Warn(CommonUtil.TAG, "Invalid autofill type - " + autofillType);
							break;
					}
				}
			}

			return setValueAtLeastOnce;
		}

		/**
		 * Takes in a list of autofill hints (autofillHints), usually associated with a View or set of
		 * Views. Returns whether any of the filled fields on the page have at least 1 of these
		 * autofillHints.
		 */
		public bool HelpsWithHints(List<string> autofillHints)
		{
			for (int i = 0; i < autofillHints.Count; i++)
			{
				var autofillHint = autofillHints[i];
				if (mHintMap.ContainsKey(autofillHint))
				{
					return true;
				}
			}

			return false;
		}
	}
}