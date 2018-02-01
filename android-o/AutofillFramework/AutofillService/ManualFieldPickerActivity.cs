using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AutofillService.Data;
using AutofillService.Data.Source.Local;
using AutofillService.Data.Source.Local.Db;
using AutofillService.Model;
using GoogleGson;

namespace AutofillService
{
	public class ManualFieldPickerActivity : AppCompatActivity
	{

		private static string EXTRA_DATASET_ID = "extra_dataset_id";
		public static string EXTRA_SELECTED_FIELD_DATASET_ID = "selected_field_dataset_id";
		public static string EXTRA_SELECTED_FIELD_TYPE_NAME = "selected_field_type_name";

		private LocalAutofillDataSource mLocalAutofillDataSource;

		private RecyclerView mRecyclerView;
		private TextView mListTitle;
		private DatasetWithFilledAutofillFields mDataset;

		public static Intent GetIntent(Context originContext, String datasetId)
		{
			var intent = new Intent(originContext, typeof(ManualFieldPickerActivity));
			intent.PutExtra(EXTRA_DATASET_ID, datasetId);
			return intent;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_field_picker);
			var sharedPreferences = GetSharedPreferences(LocalAutofillDataSource.SHARED_PREF_KEY, FileCreationMode.Private);
			var defaultFieldTypesSource = DefaultFieldTypesLocalJsonSource.GetInstance(Resources, new GsonBuilder().Create());
			var autofillDao = AutofillDatabase.GetInstance(this, defaultFieldTypesSource, new AppExecutors()).AutofillDao();
			string datasetId = Intent.GetStringExtra(EXTRA_DATASET_ID);
			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.fieldsList);
			mRecyclerView.AddItemDecoration(new DividerItemDecoration(this, OrientationHelper.Vertical));
			mListTitle = FindViewById<TextView>(Resource.Id.listTitle);
			mLocalAutofillDataSource = LocalAutofillDataSource.GetInstance(sharedPreferences, autofillDao, new AppExecutors());
			mLocalAutofillDataSource.GetAutofillDatasetWithId(datasetId, new DataCallback
			{
				that = this
			});
		}

		public class DataCallback : Java.Lang.Object, IDataCallback<DatasetWithFilledAutofillFields>
		{
			public ManualFieldPickerActivity that;
			public void OnLoaded(DatasetWithFilledAutofillFields dataset)
			{
				that.mDataset = dataset;
				if (that.mDataset != null)
				{
					that.OnLoadedDataset();
				}
			}

			public void OnDataNotAvailable(string msg, params object[] pObjects)
			{
			}
		}

		public void OnSelectedDataset(FilledAutofillField field)
		{
			Intent data = new Intent()
				.PutExtra(EXTRA_SELECTED_FIELD_DATASET_ID, field.GetDatasetId())
				.PutExtra(EXTRA_SELECTED_FIELD_TYPE_NAME, field.GetFieldTypeName());
			SetResult(Result.Ok, data);
			Finish();
		}

		public void OnLoadedDataset()
		{
			var fieldsAdapter = new FieldsAdapter(this, mDataset.filledAutofillFields);
			mRecyclerView.SetAdapter(fieldsAdapter);
			mListTitle.Text = GetString(Resource.String.manual_data_picker_title, mDataset.autofillDataset.GetDatasetName());
		}

		private class FieldsAdapter : RecyclerView.Adapter
		{

			private Activity mActivity;
			private ImmutableList<FilledAutofillField> mFields;

			public FieldsAdapter(Activity activity, ImmutableList<FilledAutofillField> fields)
			{
				mActivity = activity;
				mFields = fields;
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				FilledAutofillField field = mFields[position];
				((FieldViewHolder)holder).Bind(field);
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				return new FieldViewHolder(LayoutInflater.From(parent.Context)
					.Inflate(Resource.Layout.dataset_field, parent, false), mActivity);
			}

			public override int ItemCount => mFields.Count;
		}

		private class FieldViewHolder : RecyclerView.ViewHolder
		{

			private View mRootView;
			private TextView mFieldTypeText;
			private Activity mActivity;

			public FieldViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
			{
			}

			public FieldViewHolder(View itemView, Activity activity) : base(itemView)
			{
				mRootView = itemView;
				mFieldTypeText = itemView.FindViewById<TextView>(Resource.Id.fieldType);
				mActivity = activity;
			}

			public void Bind(FilledAutofillField field)
			{
				mFieldTypeText.Text = field.GetFieldTypeName();
				mRootView.Click += delegate
				{
					((ManualFieldPickerActivity)mActivity).OnSelectedDataset(field);
				};
			}
		}
	}
}