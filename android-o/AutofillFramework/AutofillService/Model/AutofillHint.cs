using System;
using Android.Support.Annotation;

namespace AutofillService.Model
{
//[Entity(primaryKeys = { "autofillHint"}, foreignKeys = @ForeignKey( entity = FieldType.class, parentColumns = "typeName", childColumns = "fieldTypeName", onDelete = ForeignKey.CASCADE))]
	public class AutofillHint
	{
		[NonNull]
		//[ColumnInfo(name = "autofillHint")]
		public String mAutofillHint;

		[NonNull]
		//[ColumnInfo(name = "fieldTypeName")]

		public String mFieldTypeName;

		public AutofillHint([NonNull] string autofillHint, [NonNull] string fieldTypeName)
		{
			mAutofillHint = autofillHint;
			mFieldTypeName = fieldTypeName;
		}
	}
}