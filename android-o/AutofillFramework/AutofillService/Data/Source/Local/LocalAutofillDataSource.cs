using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Android.Content;
using AutofillService.Data.Source.Local.Dao;
using AutofillService.Model;
using Java.Lang;
using Object = System.Object;
using String = System.String;

namespace AutofillService.Data.Source.Local
{
	public class LocalAutofillDataSource : IAutofillDataSource
	{

		public static String SHARED_PREF_KEY = "com.example.android.autofill"
		+ ".service.datasource.LocalAutofillDataSource";
		private static String DATASET_NUMBER_KEY = "datasetNumber";
		private static Object sLock = new Object();

		private static LocalAutofillDataSource sInstance;

		private static IAutofillDao mAutofillDao;
		private static ISharedPreferences mSharedPreferences;
		private static AppExecutors mAppExecutors;

		private LocalAutofillDataSource(ISharedPreferences sharedPreferences, IAutofillDao autofillDao, AppExecutors appExecutors)
		{
			mSharedPreferences = sharedPreferences;
			mAutofillDao = autofillDao;
			mAppExecutors = appExecutors;
		}

		public static LocalAutofillDataSource GetInstance(ISharedPreferences sharedPreferences, IAutofillDao autofillDao, AppExecutors appExecutors)
		{
			lock (sLock)
			{
				if (sInstance == null)
				{
					sInstance = new LocalAutofillDataSource(sharedPreferences, autofillDao,
						appExecutors);
				}
				return sInstance;
			}
		}

		public static void ClearInstance()
		{
			lock (sLock)
			{
				sInstance = null;
			}
		}

		public void GetAutofillDatasets(List<string> allAutofillHints, IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable1
			{
				datasetsCallback = datasetsCallback,
				allAutofillHints = allAutofillHints
			}); ;
		}

		public class AppExecutorsRunable1 : Java.Lang.Object, IRunnable
		{
			public List<String> allAutofillHints;
			public IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback;
			public void Run()
			{
				var typeNames = GetFieldTypesForAutofillHints(allAutofillHints)
					.Select(x=>x.getFieldType())
					.Select(x => x.GetTypeName())
					.ToList();

				var datasetsWithFilledAutofillFields = mAutofillDao.GetDatasets(typeNames);
				mAppExecutors.mainThread.Execute(new MainThreadRunable1
				{
					datasetsCallback = datasetsCallback,
					datasetsWithFilledAutofillFields = datasetsWithFilledAutofillFields
				});

			}
		}

		public class MainThreadRunable1 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback;
			public List<DatasetWithFilledAutofillFields> datasetsWithFilledAutofillFields;
			public void Run()
			{
				datasetsCallback.OnLoaded(datasetsWithFilledAutofillFields);
			}
		}

		public void GetAllAutofillDatasets(IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable2 { datasetsCallback = datasetsCallback });
		}

		public class AppExecutorsRunable2 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback;
			public void Run()
			{
				var datasetsWithFilledAutofillFields = mAutofillDao.GetAllDatasets();
				mAppExecutors.mainThread.Execute(new MainThreadRunable2
				{
					datasetsCallback = datasetsCallback,
					datasetsWithFilledAutofillFields = datasetsWithFilledAutofillFields
				});
			}
		}

		public class MainThreadRunable2 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<List<DatasetWithFilledAutofillFields>> datasetsCallback;
			public List<DatasetWithFilledAutofillFields> datasetsWithFilledAutofillFields;
			public void Run()
			{
				datasetsCallback.OnLoaded(datasetsWithFilledAutofillFields);
			}
		}

		public void GetAutofillDataset(List<string> allAutofillHints, string datasetName, IDataCallback<DatasetWithFilledAutofillFields> datasetsCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable3
			{
				datasetName = datasetName,
				allAutofillHints = allAutofillHints,
				datasetsCallback = datasetsCallback
			});
		}

		public class AppExecutorsRunable3 : Java.Lang.Object, IRunnable
		{
			public string datasetName;
			public List<string> allAutofillHints;
			public IDataCallback<DatasetWithFilledAutofillFields> datasetsCallback;
			public void Run()
			{
				// Room does not support TypeConverters for collections.
				var autofillDatasetFields = mAutofillDao.GetDatasetsWithName(allAutofillHints, datasetName);
				if (autofillDatasetFields != null && autofillDatasetFields.Count != 0)
				{
					if (autofillDatasetFields.Count > 1)
					{
						Util.Logw("More than 1 dataset with name %s", datasetName);
					}
					DatasetWithFilledAutofillFields dataset = autofillDatasetFields[0];

					mAppExecutors.mainThread.Execute(new MainThreadRunable3OnLoaded
					{
						dataset = dataset,
						datasetsCallback = datasetsCallback
					});
				}
				else
				{
					mAppExecutors.mainThread.Execute(new MainThreadRunable3OnDataNotAvailable
					{
						datasetsCallback = datasetsCallback
					});
				}
			}
		}

		public class MainThreadRunable3OnLoaded : Java.Lang.Object, IRunnable
		{
			public DatasetWithFilledAutofillFields dataset;
			public IDataCallback<DatasetWithFilledAutofillFields> datasetsCallback;
			public void Run()
			{
				datasetsCallback.OnLoaded(dataset);
			}
		}

		public class MainThreadRunable3OnDataNotAvailable : Java.Lang.Object, IRunnable
		{
			public IDataCallback<DatasetWithFilledAutofillFields> datasetsCallback;
			public void Run()
			{
				datasetsCallback.OnDataNotAvailable("No data found.");
			}
		}

		public void SaveAutofillDatasets(ImmutableList<DatasetWithFilledAutofillFields> datasetsWithFilledAutofillFields)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable4
			{
				datasetsWithFilledAutofillFields = datasetsWithFilledAutofillFields
			});

			IncrementDatasetNumber();
		}

		public class AppExecutorsRunable4 : Java.Lang.Object, IRunnable
		{
			public ImmutableList<DatasetWithFilledAutofillFields> datasetsWithFilledAutofillFields;
			public void Run()
			{
				foreach (var datasetWithFilledAutofillFields in datasetsWithFilledAutofillFields)
				{
					var filledAutofillFields = datasetWithFilledAutofillFields.filledAutofillFields;
					AutofillDataset autofillDataset = datasetWithFilledAutofillFields.autofillDataset;
					mAutofillDao.InsertAutofillDataset(autofillDataset);
					mAutofillDao.InsertFilledAutofillFields(filledAutofillFields);
				}
			}
		}

		public void SaveResourceIdHeuristic(ResourceIdHeuristic resourceIdHeuristic)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable5
			{
				resourceIdHeuristic = resourceIdHeuristic
			});

		}

		public class AppExecutorsRunable5 : Java.Lang.Object, IRunnable
		{
			public ResourceIdHeuristic resourceIdHeuristic;
			public void Run()
			{
				mAutofillDao.InsertResourceIdHeuristic(resourceIdHeuristic);
			}
		}

		public void GetFieldTypes(IDataCallback<List<FieldTypeWithHeuristics>> fieldTypesCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable6
			{
				fieldTypesCallback = fieldTypesCallback
			});
		}

		public class AppExecutorsRunable6 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<List<FieldTypeWithHeuristics>> fieldTypesCallback;
			public void Run()
			{
				var fieldTypeWithHints = mAutofillDao.GetFieldTypesWithHints();
				mAppExecutors.mainThread.Execute(new MainThreadRunable6
				{
					fieldTypesCallback = fieldTypesCallback,
					fieldTypeWithHints = fieldTypeWithHints
				});

			}
		}

		public class MainThreadRunable6 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<List<FieldTypeWithHeuristics>> fieldTypesCallback;
			public List<FieldTypeWithHeuristics> fieldTypeWithHints;
			public void Run()
			{
				if (fieldTypeWithHints != null)
				{
					fieldTypesCallback.OnLoaded(fieldTypeWithHints);
				}
				else
				{
					fieldTypesCallback.OnDataNotAvailable("Field Types not found.");
				}
			}
		}

		public void GetFieldType(string typeName, IDataCallback<FieldType> fieldTypeCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable7
			{
				typeName = typeName,
				fieldTypeCallback = fieldTypeCallback
			});
		}

		public class AppExecutorsRunable7 : Java.Lang.Object, IRunnable
		{
			public string typeName;
			public IDataCallback<FieldType> fieldTypeCallback;
			public void Run()
			{
				FieldType fieldType = mAutofillDao.GetFieldType(typeName);
				mAppExecutors.mainThread.Execute(new MainThreadRunable7
				{
					fieldType = fieldType,
					fieldTypeCallback = fieldTypeCallback
				});
			}
		}

		public class MainThreadRunable7 : Java.Lang.Object, IRunnable
		{
			public FieldType fieldType;
			public IDataCallback<FieldType> fieldTypeCallback;
			public void Run()
			{
				fieldTypeCallback.OnLoaded(fieldType);
			}
		}

		public void GetFieldTypeByAutofillHints(IDataCallback<Dictionary<string, FieldTypeWithHeuristics>> fieldTypeMapCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable8
			{
				fieldTypeMapCallback = fieldTypeMapCallback
			});
		}

		public class AppExecutorsRunable8 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<Dictionary<string, FieldTypeWithHeuristics>> fieldTypeMapCallback;

			public void Run()
			{
				var hintMap = GetFieldTypeByAutofillHints();
				mAppExecutors.mainThread.Execute(new MainThreadRunable8
				{
					hintMap = hintMap,
					fieldTypeMapCallback = fieldTypeMapCallback
				});
			}
		}

		public class MainThreadRunable8 : Java.Lang.Object, IRunnable
		{
			public Dictionary<string, FieldTypeWithHeuristics> hintMap;
			public IDataCallback<Dictionary<string, FieldTypeWithHeuristics>> fieldTypeMapCallback;
			public void Run()
			{
				if (hintMap != null)
				{
					fieldTypeMapCallback.OnLoaded(hintMap);
				}
				else
				{
					fieldTypeMapCallback.OnDataNotAvailable("FieldTypes not found");
				}
			}
		}

		public static Dictionary<string, FieldTypeWithHeuristics> GetFieldTypeByAutofillHints()
		{
			var hintMap = new Dictionary<string, FieldTypeWithHeuristics>();
			var fieldTypeWithHints = mAutofillDao.GetFieldTypesWithHints();
			if (fieldTypeWithHints != null)
			{
				foreach (var fieldType in fieldTypeWithHints)
				{
					foreach (var hint in fieldType.autofillHints)
					{
						hintMap.Add(hint.mAutofillHint, fieldType);
					}
				}
				return hintMap;
			}
			return null;
		}

		public void GetFilledAutofillField(string datasetId, string fieldTypeName, IDataCallback<FilledAutofillField> fieldCallback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable9
			{
				datasetId = datasetId,
				fieldTypeName = fieldTypeName,
				fieldCallback = fieldCallback
			});
		}

		public class AppExecutorsRunable9 : Java.Lang.Object, IRunnable
		{
			public string datasetId;
			public string fieldTypeName;
			public IDataCallback<FilledAutofillField> fieldCallback;

			public void Run()
			{
				var filledAutofillField = mAutofillDao.GetFilledAutofillField(datasetId, fieldTypeName);
				mAppExecutors.mainThread.Execute(new MainThreadRunable9
				{
					fieldCallback = fieldCallback,
					filledAutofillField = filledAutofillField
				});
			}
		}

		public class MainThreadRunable9 : Java.Lang.Object, IRunnable
		{
			public IDataCallback<FilledAutofillField> fieldCallback;
			public FilledAutofillField filledAutofillField;
			public void Run()
			{
				fieldCallback.OnLoaded(filledAutofillField);
			}
		}

		public void Clear()
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable10());
		}

		public class AppExecutorsRunable10 : Java.Lang.Object, IRunnable
		{
			public void Run()
			{
				mAutofillDao.ClearAll();
				mSharedPreferences.Edit().PutInt(DATASET_NUMBER_KEY, 0).Apply();
			}
		}

		public void GetAutofillDatasetWithId(string datasetId, IDataCallback<DatasetWithFilledAutofillFields> callback)
		{
			mAppExecutors.diskIO.Execute(new AppExecutorsRunable11
			{
				callback = callback,
				datasetId = datasetId
			});
		}

		public class AppExecutorsRunable11 : Java.Lang.Object, IRunnable
		{
			public string datasetId;
			public IDataCallback<DatasetWithFilledAutofillFields> callback;

			public void Run()
			{
				DatasetWithFilledAutofillFields dataset = mAutofillDao.GetAutofillDatasetWithId(datasetId);
				mAppExecutors.mainThread.Execute(new MainThreadRunable11
				{
					dataset = dataset,
					callback = callback
				});
			}
		}

		public class MainThreadRunable11 : Java.Lang.Object, IRunnable
		{
			public DatasetWithFilledAutofillFields dataset;
			public IDataCallback<DatasetWithFilledAutofillFields> callback;
			public void Run()
			{
				callback.OnLoaded(dataset);
			}
		}

		private static IList<FieldTypeWithHeuristics> GetFieldTypesForAutofillHints(List<String> autofillHints)
		{
			return mAutofillDao.GetFieldTypesForAutofillHints(autofillHints);
		}

		/**
		* For simplicity, {@link Dataset}s will be named in the form {@code dataset-X.P} where
		* {@code X} means this was the Xth group of datasets saved, and {@code P} refers to the dataset
		* partition number. This method returns the appropriate {@code X}.
		*/
		public int GetDatasetNumber()
		{
			return mSharedPreferences.GetInt(DATASET_NUMBER_KEY, 0);
		}

		/**
		 * Every time a dataset is saved, this should be called to increment the dataset number.
		 * (only important for this service's dataset naming scheme).
		 */
		private void IncrementDatasetNumber()
		{
			mSharedPreferences.Edit().PutInt(DATASET_NUMBER_KEY, GetDatasetNumber() + 1).Apply();
		}
	}
}