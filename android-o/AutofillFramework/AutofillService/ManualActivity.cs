using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Android.App;
using Android.App.Assist;
using Android.Content;
using Android.OS;
using Android.Service.Autofill;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Autofill;
using Android.Widget;
using AutofillService.Data;
using AutofillService.Data.Adapter;
using AutofillService.Data.Source;
using AutofillService.Data.Source.Local;
using AutofillService.Data.Source.Local.Db;
using AutofillService.Model;
using GoogleGson;

namespace AutofillService
{
	public class ManualActivity : AppCompatActivity
	{
		private static int RC_SELECT_FIELD = 1;

		// Unique id for dataset intents.
		private static int sDatasetPendingIntentId = 0;

		private LocalAutofillDataSource mLocalAutofillDataSource;
		private DatasetAdapter mDatasetAdapter;
		private ResponseAdapter mResponseAdapter;
		private ClientViewMetadata mClientViewMetadata;
		private String mPackageName;
		private Intent mReplyIntent;
		private MyPreferences mPreferences;
		private List<DatasetWithFilledAutofillFields> mAllDatasets;
		private RecyclerView mRecyclerView;

		public static IntentSender GetManualIntentSenderForResponse(Context context)
		{
			var intent = new Intent(context, typeof(ManualActivity));
			return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent).IntentSender;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.multidataset_service_manual_activity);
			var sharedPreferences = GetSharedPreferences(LocalAutofillDataSource.SHARED_PREF_KEY, FileCreationMode.Private);
			var defaultFieldTypesSource = DefaultFieldTypesLocalJsonSource.GetInstance(Resources, new GsonBuilder().Create());
			var autofillDao = AutofillDatabase.GetInstance(this, defaultFieldTypesSource, new AppExecutors()).AutofillDao();
			mLocalAutofillDataSource = LocalAutofillDataSource.GetInstance(sharedPreferences, autofillDao, new AppExecutors());
			mPackageName = PackageName;
			mPreferences = MyPreferences.GetInstance(this);
			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.suggestionsList);
			mRecyclerView.AddItemDecoration(new DividerItemDecoration(this, OrientationHelper.Vertical));
			mLocalAutofillDataSource.GetAllAutofillDatasets(new DataCallback { that = this });
		}

		public class DataCallback : Java.Lang.Object, IDataCallback<List<DatasetWithFilledAutofillFields>>
		{
			public ManualActivity that;

			public void OnLoaded(List<DatasetWithFilledAutofillFields> datasets)
			{
				that.mAllDatasets = datasets;
				that.BuildAdapter();
			}

			public void OnDataNotAvailable(string msg, params object[] pObjects)
			{
			}
		}

		private void BuildAdapter()
		{
			var datasetIds = new List<string>();
			var datasetNames = new List<string>();
			var allFieldTypes = new List<List<string>>();
			foreach (var dataset in mAllDatasets)
			{
				string datasetName = dataset.autofillDataset.GetDatasetName();
				string datasetId = dataset.autofillDataset.GetId();
				var fieldTypes = new List<string>();
				foreach (var filledAutofillField in dataset.filledAutofillFields)
				{
					fieldTypes.Add(filledAutofillField.GetFieldTypeName());
				}
				datasetIds.Add(datasetId);
				datasetNames.Add(datasetName);
				allFieldTypes.Add(fieldTypes);
			}
			var adapter = new AutofillDatasetsAdapter(datasetIds, datasetNames, allFieldTypes, this);
			mRecyclerView.SetAdapter(adapter);
		}

		public override void Finish()
		{
			if (mReplyIntent != null)
			{
				SetResult(Result.Ok, mReplyIntent);
			}
			else
			{
				SetResult(Result.Canceled);
			}
			base.Finish();
		}

		private void OnFieldSelected(FilledAutofillField field, FieldType fieldType)
		{
			DatasetWithFilledAutofillFields datasetWithFilledAutofillFields = new DatasetWithFilledAutofillFields();
			String newDatasetId = Guid.NewGuid().ToString();
			FilledAutofillField copyOfField = new FilledAutofillField(newDatasetId, PackageName,
				field.GetFieldTypeName(), field.GetTextValue(), field.GetDateValue(), field.GetToggleValue());
			String datasetName = "dataset-manual";
			AutofillDataset autofillDataset = new AutofillDataset(newDatasetId, datasetName);
			datasetWithFilledAutofillFields.filledAutofillFields =
				new List<FilledAutofillField>() { copyOfField }.ToImmutableList();
			datasetWithFilledAutofillFields.autofillDataset = autofillDataset;
			Intent intent = Intent;
			var structure = (AssistStructure)intent.GetParcelableExtra(AutofillManager.ExtraAssistStructure);
			ClientParser clientParser = new ClientParser(structure);
			mReplyIntent = new Intent();
			mLocalAutofillDataSource.GetFieldTypeByAutofillHints(new DataCallback2
			{
				that = this,
				datasetName = datasetName,
				fieldType = fieldType,
				field = field,
				clientParser = clientParser
			});
		}

		public class DataCallback2 : Java.Lang.Object, IDataCallback<Dictionary<string, FieldTypeWithHeuristics>>
		{
			public string datasetName;
			public FieldType fieldType;
			public ManualActivity that;
			public FilledAutofillField field;
			public ClientParser clientParser;

			public void OnLoaded(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint)
			{
				ClientViewMetadataBuilder builder = new ClientViewMetadataBuilder(clientParser,
					fieldTypesByAutofillHint);
				that.mClientViewMetadata = builder.BuildClientViewMetadata();
				that.mDatasetAdapter = new DatasetAdapter(clientParser);
				that.mResponseAdapter =
					new ResponseAdapter(that, that.mClientViewMetadata, that.mPackageName, that.mDatasetAdapter);
				var fillResponse = that.mResponseAdapter.BuildResponseForFocusedNode(datasetName, field, fieldType);
				that.SetResponseIntent(fillResponse);
				that.Finish();
			}

			public void OnDataNotAvailable(string msg, params object[] pObjects)
			{
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (requestCode != RC_SELECT_FIELD || resultCode != Result.Ok)
			{
				Util.Logd("Ignoring requestCode == %d | resultCode == %d", requestCode, resultCode);
				return;
			}
			string datasetId = data.GetStringExtra(ManualFieldPickerActivity.EXTRA_SELECTED_FIELD_DATASET_ID);
			string fieldTypeName = data.GetStringExtra(ManualFieldPickerActivity.EXTRA_SELECTED_FIELD_TYPE_NAME);
			mLocalAutofillDataSource.GetFilledAutofillField(datasetId, fieldTypeName, new DataCallback3 { that = this });
		}

		public class DataCallback3 : Java.Lang.Object, IDataCallback<FilledAutofillField>
		{
			public ManualActivity that;

			public void OnLoaded(FilledAutofillField field)
			{
				that.mLocalAutofillDataSource.GetFieldType(field.GetFieldTypeName(), new DataCallback4
				{
					that = that,
					field = field
				});
			}

			public void OnDataNotAvailable(string msg, params object[] pObjects)
			{
			}
		}

		public class DataCallback4 : Java.Lang.Object, IDataCallback<FieldType>
		{
			public ManualActivity that;
			public FilledAutofillField field;

			public void OnLoaded(FieldType fieldType)
			{
				that.OnFieldSelected(field, fieldType);
			}

			public void OnDataNotAvailable(string msg, params object[] pObjects)
			{
			}
		}

		private void UpdateHeuristics()
		{
		}

		private void SetResponseIntent(FillResponse fillResponse)
		{
			mReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, fillResponse);
		}

		private void SetDatasetIntent(Dataset dataset)
		{
			mReplyIntent.PutExtra(AutofillManager.ExtraAuthenticationResult, dataset);
		}

		/**
		* Adapter for the {@link RecyclerView} that holds a list of datasets.
		*/
		private class AutofillDatasetsAdapter : RecyclerView.Adapter
		{
			private List<string> mDatasetIds;
			private List<string> mDatasetNames;
			private List<List<string>> mFieldTypes;
			private Activity mActivity;

			public AutofillDatasetsAdapter(List<string> datasetIds, List<string> datasetNames,
				List<List<string>> fieldTypes, Activity activity)
			{
				mDatasetIds = datasetIds;
				mDatasetNames = datasetNames;
				mFieldTypes = fieldTypes;
				mActivity = activity;
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				((DatasetViewHolder)holder).Bind(mDatasetIds[position], mDatasetNames[position],
					mFieldTypes[position]);
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				return DatasetViewHolder.newInstance(parent, mActivity);
			}

			public override int ItemCount => mDatasetNames.Count;
		}

		/**
		* Contains views needed in each row of the list of datasets.
		*/
		private class DatasetViewHolder : RecyclerView.ViewHolder
		{

			private View mRootView;
			private TextView mDatasetNameText;
			private TextView mFieldTypesText;
			private Activity mActivity;

			public DatasetViewHolder(View itemView, Activity activity) : base(itemView)
			{

				mRootView = itemView;
				mDatasetNameText = itemView.FindViewById<TextView>(Resource.Id.datasetName);
				mFieldTypesText = itemView.FindViewById<TextView>(Resource.Id.fieldTypes);
				mActivity = activity;
			}

			public static DatasetViewHolder newInstance(ViewGroup parent, Activity activity)
			{
				return new DatasetViewHolder(LayoutInflater.From(parent.Context)
						.Inflate(Resource.Layout.dataset_suggestion, parent, false), activity);
			}

			public void Bind(String datasetId, String datasetName, List<String> fieldTypes)
			{
				mDatasetNameText.Text = datasetName;
				String firstFieldType = null;
				String secondFieldType = null;
				int numOfFieldTypes = 0;
				if (fieldTypes != null)
				{
					numOfFieldTypes = fieldTypes.Count;
					if (numOfFieldTypes > 0)
					{
						firstFieldType = fieldTypes[0];
					}
					if (numOfFieldTypes > 1)
					{
						secondFieldType = fieldTypes[1];
					}
				}
				string fieldTypesString;
				if (numOfFieldTypes == 1)
				{
					fieldTypesString = "Contains data for " + firstFieldType + ".";
				}
				else if (numOfFieldTypes == 2)
				{
					fieldTypesString = "Contains data for " + firstFieldType + " and " + secondFieldType + ".";
				}
				else if (numOfFieldTypes > 2)
				{
					fieldTypesString = "Contains data for " + firstFieldType + ", " + secondFieldType + ", and more.";
				}
				else
				{
					fieldTypesString = "Ignore: Contains no data.";
				}
				mFieldTypesText.Text = fieldTypesString;
				mRootView.Click += delegate
				{
					var intent = ManualFieldPickerActivity.GetIntent(mActivity, datasetId);
					mActivity.StartActivityForResult(intent, RC_SELECT_FIELD);
				};
			}
		}
	}
}