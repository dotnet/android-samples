using System;
using System.Collections.Generic;
using Android.Content;
using Android.Service.Autofill;
using Android.Views.Autofill;
using Android.Widget;
using AutofillFramework;
using AutofillService.Data.Source;
using AutofillService.Model;

namespace AutofillService.Data.Adapter
{
	public class ResponseAdapter
	{
		private Context mContext;
		private DatasetAdapter mDatasetAdapter;
		private String mPackageName;
		private ClientViewMetadata mClientViewMetadata;

		public ResponseAdapter(Context context, ClientViewMetadata clientViewMetadata,
			String packageName, DatasetAdapter datasetAdapter)
		{
			mContext = context;
			mClientViewMetadata = clientViewMetadata;
			mDatasetAdapter = datasetAdapter;
			mPackageName = packageName;
		}

		public FillResponse BuildResponseForFocusedNode(string datasetName, FilledAutofillField field, FieldType fieldType)
		{
			FillResponse.Builder responseBuilder = new FillResponse.Builder();
			RemoteViews remoteViews = RemoteViewsHelper.ViewsWithNoAuth(
				mPackageName, datasetName);
			Dataset dataset = mDatasetAdapter.BuildDatasetForFocusedNode(field, fieldType, remoteViews);
			if (dataset != null)
			{
				responseBuilder.AddDataset(dataset);
				return responseBuilder.Build();
			}
			return null;
		}

		/**
		 * Wraps autofill data in a Response object (essentially a series of Datasets) which can then
		 * be sent back to the client View.
		 */
		public FillResponse BuildResponse(Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint,
				List<DatasetWithFilledAutofillFields> datasets, bool datasetAuth)
		{
			FillResponse.Builder responseBuilder = new FillResponse.Builder();
			if (datasets != null)
			{
				foreach (var datasetWithFilledAutofillFields in datasets)
				{
					if (datasetWithFilledAutofillFields != null)
					{
						Dataset dataset;
						String datasetName = datasetWithFilledAutofillFields.autofillDataset.GetDatasetName();
						if (datasetAuth)
						{
							IntentSender intentSender = AuthActivity.GetAuthIntentSenderForDataset(
									mContext, datasetName);
							RemoteViews remoteViews = RemoteViewsHelper.ViewsWithAuth(
									mPackageName, datasetName);
							dataset = mDatasetAdapter.BuildDataset(fieldTypesByAutofillHint,
									datasetWithFilledAutofillFields, remoteViews, intentSender);
						}
						else
						{
							RemoteViews remoteViews = RemoteViewsHelper.ViewsWithNoAuth(
									mPackageName, datasetName);
							dataset = mDatasetAdapter.BuildDataset(fieldTypesByAutofillHint,
									datasetWithFilledAutofillFields, remoteViews);
						}
						if (dataset != null)
						{
							responseBuilder.AddDataset(dataset);
						}
					}
				}
			}
			int saveType = 0;
			AutofillId[] autofillIds = mClientViewMetadata.mFocusedIds;
			if (autofillIds != null && autofillIds.Length> 0)
			{
				SaveInfo saveInfo = new SaveInfo.Builder((SaveDataType)saveType, autofillIds).Build();
				responseBuilder.SetSaveInfo(saveInfo);
				return responseBuilder.Build();
			}
			return null;
		}

		public FillResponse BuildResponse(IntentSender sender, RemoteViews remoteViews)
		{
			FillResponse.Builder responseBuilder = new FillResponse.Builder();
			int saveType = mClientViewMetadata.mSaveType;
			AutofillId[] autofillIds = mClientViewMetadata.mFocusedIds;
			if (autofillIds != null && autofillIds.Length > 0)
			{
				SaveInfo saveInfo = new SaveInfo.Builder((SaveDataType)saveType, autofillIds).Build();
				responseBuilder.SetSaveInfo(saveInfo);
				responseBuilder.SetAuthentication(autofillIds, sender, remoteViews);
				return responseBuilder.Build();
			}
			return null;
		}

		public FillResponse BuildManualResponse(IntentSender sender, RemoteViews remoteViews)
		{
			FillResponse.Builder responseBuilder = new FillResponse.Builder();
			int saveType = mClientViewMetadata.mSaveType;
			AutofillId[] focusedIds = mClientViewMetadata.mFocusedIds;
			if (focusedIds != null && focusedIds.Length > 0)
			{
				SaveInfo saveInfo = new SaveInfo.Builder((SaveDataType)saveType, focusedIds).Build();
				return responseBuilder.SetSaveInfo(saveInfo)
					.SetAuthentication(focusedIds, sender, remoteViews)
					.Build();
			}
			return null;
		}
	}
}