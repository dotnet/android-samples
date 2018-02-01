using System;
using Android.Support.Annotation;

namespace AutofillService.Model
{
	//[Entity(primaryKeys = { "resourceIdHeuristic", "packageName"}, foreignKeys = @ForeignKey(
	//entity = FieldType.class, parentColumns = "typeName", childColumns = "fieldTypeName",
	//onDelete = ForeignKey.CASCADE))
	public class ResourceIdHeuristic
	{

		[NonNull]
		//[ColumnInfo(name = "resourceIdHeuristic")

		public String mResourceIdHeuristic;

		[NonNull]
		//ColumnInfo(name = "packageName")

		public String mPackageName;
		[NonNull]
		//ColumnInfo(name = "fieldTypeName")

		public String mFieldTypeName;

		public ResourceIdHeuristic([NonNull] String resourceIdHeuristic, [NonNull] String fieldTypeName,
			[NonNull] String packageName)
		{
			mResourceIdHeuristic = resourceIdHeuristic;
			mFieldTypeName = fieldTypeName;
			mPackageName = packageName;
		}
	}
}