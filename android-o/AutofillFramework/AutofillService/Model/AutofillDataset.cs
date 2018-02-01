using Android.Support.Annotation;

namespace AutofillService.Model
{
	//[Entity(primaryKeys = {"id"})]
	public class AutofillDataset
	{
		[NonNull]
		//[ColumnInfo(name = "id")]
		private string mId;

		[NonNull]
		//[ColumnInfo(name = "datasetName")]
		private string mDatasetName;

		public AutofillDataset([NonNull] string id, [NonNull] string datasetName)
		{
			mId = id;
			mDatasetName = datasetName;
		}

		[NonNull]
		public string GetId() => mId;

		[NonNull]
		public string GetDatasetName() => mDatasetName;

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if (obj == null || this.GetType().Name != typeof(object).Name) return false;

			AutofillDataset that = (AutofillDataset)obj;

			if (!mId.Equals(that.mId)) return false;
			return mDatasetName.Equals(that.mDatasetName);
		}

		public override int GetHashCode()
		{
			int result = mId.GetHashCode();
			result = 31 * result + mDatasetName.GetHashCode();
			return result;
		}
	}
}