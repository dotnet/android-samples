using System.Collections.Immutable;

namespace AutofillService.Model
{
	public class DatasetWithFilledAutofillFields
	{
		//[Embedded]
		public AutofillDataset autofillDataset;

		//[Relation(parentColumn = "id", entityColumn = "datasetId", entity = FilledAutofillField.class)]
		public ImmutableList<FilledAutofillField> filledAutofillFields;

		public void Add(FilledAutofillField filledAutofillField)
		{
			if (filledAutofillFields == null)
			{
				filledAutofillFields = ImmutableList.Create<FilledAutofillField>();
			}
			filledAutofillFields.Add(filledAutofillField);
		}

		public override bool Equals(object o)
		{
			if (this == o) return true;
			if (o == null || GetType() != o.GetType()) return false;

			DatasetWithFilledAutofillFields that = (DatasetWithFilledAutofillFields)o;

			if (autofillDataset != null ? !autofillDataset.Equals(that.autofillDataset) :
				that.autofillDataset != null)
				return false;
			return filledAutofillFields != null ?
				filledAutofillFields.Equals(that.filledAutofillFields) :
				that.filledAutofillFields == null;

		}

		public override int GetHashCode()
		{
			int result = autofillDataset != null ? autofillDataset.GetHashCode() : 0;
			result = 31 * result + (filledAutofillFields != null ? filledAutofillFields.GetHashCode() : 0);
			return result;
		}
	}
}