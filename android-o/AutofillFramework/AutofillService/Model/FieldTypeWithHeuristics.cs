using System.Collections.Generic;

namespace AutofillService.Model
{
	public class FieldTypeWithHeuristics
	{
		//[Embedded]
		public FieldType fieldType;

		//[Relation(parentColumn = "typeName", entityColumn = "fieldTypeName", entity = AutofillHint.class)]
		public List<AutofillHint> autofillHints;

		//[Relation(parentColumn = "typeName", entityColumn = "fieldTypeName", entity = ResourceIdHeuristic.class)]
		public List<ResourceIdHeuristic> resourceIdHeuristics;

		public FieldType getFieldType()
		{
			return fieldType;
		}

		public List<AutofillHint> getAutofillHints()
		{
			return autofillHints;
		}

		public List<ResourceIdHeuristic> getResourceIdHeuristics()
		{
			return resourceIdHeuristics;
		}
	}
}