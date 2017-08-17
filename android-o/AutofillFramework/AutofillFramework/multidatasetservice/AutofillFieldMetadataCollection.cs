using System;
using System.Collections.Generic;
using Android.Service.Autofill;
using Android.Views.Autofill;
using Java.Util;

namespace AutofillFramework
{
	/**
	 * Data structure that stores a collection of {@code AutofillFieldMetadata}s. Contains all of the
	 * client's {@code View} hierarchy autofill-relevant metadata.
	 */
	public class AutofillFieldMetadataCollection
	{
		List<AutofillId> AutofillIds = new List<AutofillId>();
		Dictionary<string, List<AutofillFieldMetadata>> AutofillHintsToFieldsMap = new Dictionary<string, List<AutofillFieldMetadata>>();
		public List<string> AllAutofillHints { get; }
		public List<string> FocusedAutofillHints { get; }
		int Size = 0;
		public SaveDataType SaveType { get; set; }

		public AutofillFieldMetadataCollection()
		{
			SaveType = 0;
			FocusedAutofillHints = new List<string>();
			AllAutofillHints = new List<string>();
		}

		public void Add(AutofillFieldMetadata autofillFieldMetadata)
		{
			SaveType |= autofillFieldMetadata.SaveType;
			Size++;
			AutofillIds.Add(autofillFieldMetadata.AutofillId);
			Arrays.AsList();

			List<string> hintsList = (List<string>) Arrays.AsList(autofillFieldMetadata.AutofillHints);
			AllAutofillHints.AddRange(hintsList);
			if (autofillFieldMetadata.Focused)
			{
				FocusedAutofillHints.AddRange(hintsList);
			}
			foreach (var hint in autofillFieldMetadata.AutofillHints)
			{
				if (AutofillHintsToFieldsMap[hint] == null)
				{
					AutofillHintsToFieldsMap.Add(hint, new List<AutofillFieldMetadata>());
				}
				AutofillHintsToFieldsMap[hint].Add(autofillFieldMetadata);
			}
		}

		public AutofillId[] GetAutofillIds()
		{
			return AutofillIds.ToArray();
		}

		public List<AutofillFieldMetadata> GetFieldsForHint(String hint)
		{
			return AutofillHintsToFieldsMap[hint];
		}


	}
}
