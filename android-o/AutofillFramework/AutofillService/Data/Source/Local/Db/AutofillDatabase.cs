using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using AutofillService.Data.Source.Local.Dao;
using AutofillService.Model;

namespace AutofillService.Data.Source.Local.Db
{
	//@Database(entities = {
	//FilledAutofillField.class,
	//AutofillDataset.class,
	//FieldType.class,
	//AutofillHint.class,
	//ResourceIdHeuristic.class
	//}, version = 1)
	//@TypeConverters({ Converters.class})
	public abstract class AutofillDatabase /*: RoomDatabase*/
	{
		private static Object sLock = new Object();
		private static AutofillDatabase sInstance;

		public static AutofillDatabase GetInstance(Context context, IDefaultFieldTypesSource defaultFieldTypesSource, AppExecutors appExecutors)
		{
			if (sInstance == null)
			{
				lock (sLock)
				{
					//sInstance = Room.databaseBuilder(context.getApplicationContext(),
					//	AutofillDatabase.class, "AutofillSample.db")
					//	.addCallback(new RoomDatabase.Callback() {
					//	@Override
					//	public void onCreate(@NonNull SupportSQLiteDatabase db)
					//	{
					//	appExecutors.diskIO().execute(()-> {
					//	List<DefaultFieldTypeWithHints> fieldTypes =
					//	defaultFieldTypesSource.getDefaultFieldTypes();
					//	AutofillDatabase autofillDatabase =
					//	getInstance(context, defaultFieldTypesSource,
					//	appExecutors);
					//	autofillDatabase.saveDefaultFieldTypes(fieldTypes);
					//});
					//}

					//@Override
					//public void onOpen(@NonNull SupportSQLiteDatabase db)
					//{
					//	super.onOpen(db);
					//}
					//})
					//.build();

				}
			}
			return sInstance;
		}

		private void saveDefaultFieldTypes(List<DefaultFieldTypeWithHints> defaultFieldTypes)
		{
			var storedFieldTypes = new List<FieldType>();
			var storedAutofillHints = new List<AutofillHint>();
			foreach (DefaultFieldTypeWithHints defaultType in defaultFieldTypes)
			{
				DefaultFieldTypeWithHints.DefaultFieldType defaultFieldType = defaultType.fieldType;
				List<String> autofillHints = defaultType.autofillHints;
				Converters.IntList autofillTypes = new Converters.IntList(defaultFieldType.autofillTypes);
				DefaultFieldTypeWithHints.DefaultFakeData defaultFakeData = defaultType.fieldType.fakeData;
				FakeData fakeData = new FakeData(new Converters.StringList(
						defaultFakeData.strictExampleSet), defaultFakeData.textTemplate,
					defaultFakeData.dateTemplate);
				FieldType storedFieldType = new FieldType(defaultFieldType.typeName, autofillTypes,
					defaultFieldType.saveInfo, defaultFieldType.partition, fakeData);
				storedFieldTypes.Add(storedFieldType);
				storedAutofillHints.AddRange(autofillHints.Select(x => new AutofillHint(x, storedFieldType.GetTypeName())).ToList());
			}
			var autofillDao = AutofillDao();
			autofillDao.InsertFieldTypes(storedFieldTypes);
			autofillDao.InsertAutofillHints(storedAutofillHints);
		}

		public abstract IAutofillDao AutofillDao();
	}
}