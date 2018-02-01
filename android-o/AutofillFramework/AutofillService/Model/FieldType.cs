using System;
using Android.Support.Annotation;
using AutofillService.Data.Source.Local.Db;

namespace AutofillService.Model
{
	//[Entity(primaryKeys = { "typeName"})]
	public class FieldType
	{
		[NonNull]
		//[ColumnInfo(name = "typeName")

		private String mTypeName;

		[NonNull]
		//[ColumnInfo(name = "autofillTypes")

		private Converters.IntList mAutofillTypes;

		[NonNull]
		//[ColumnInfo(name = "saveInfo")

		private int mSaveInfo;

		[NonNull]
		//[ColumnInfo(name = "partition")

		private int mPartition;

		[NonNull]
		//[Embedded

		private FakeData mFakeData;

		public FieldType([NonNull] String typeName, [NonNull] Converters.IntList autofillTypes,
			[NonNull] int saveInfo, [NonNull] int partition, [NonNull] FakeData fakeData)
		{
			mTypeName = typeName;
			mAutofillTypes = autofillTypes;
			mSaveInfo = saveInfo;
			mPartition = partition;
			mFakeData = fakeData;
		}

		[NonNull]
		public String GetTypeName()
		{
			return mTypeName;
		}

		[NonNull]
		public Converters.IntList GetAutofillTypes()
		{
			return mAutofillTypes;
		}

		[NonNull]
		public int GetSaveInfo()
		{
			return mSaveInfo;
		}

		[NonNull]
		public int GetPartition()
		{
			return mPartition;
		}

		[NonNull]
		public FakeData GetFakeData()
		{
			return mFakeData;
		}
	}
}