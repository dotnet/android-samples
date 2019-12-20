using System.Collections.Generic;
using Android.Content;
using Android.Service.Autofill;
using Android.Util;
using Android.Views.Autofill;
using Android.Widget;
using AndroidX.Annotations;
using AutofillFramework;
using AutofillService.Model;
using Java.Lang;

namespace AutofillService
{
    /**
     * This is a class containing helper methods for building Autofill Datasets and Responses.
     */

    public class AutofillHelper
    {
        private AutofillHelper()
        {
            throw new UnsupportedOperationException("provide static methods only");
        }

        /**
         * Wraps autofill data in a LoginCredential  Dataset object which can then be sent back to the
         * client View.
         */
        public static Dataset NewDataset(
            Context context,
            AutofillFieldMetadataCollection autofillFields,
            FilledAutofillFieldCollection filledAutofillFieldCollection,
            bool datasetAuth)
        {
            var datasetName = filledAutofillFieldCollection.GetDatasetName();
            if (datasetName != null)
            {
                Dataset.Builder datasetBuilder;
                if (datasetAuth)
                {
                    datasetBuilder = new Dataset.Builder
                        (NewRemoteViews(context.PackageName, datasetName, Resource.Drawable.ic_lock_black_24dp));
                    var sender = AuthActivity.GetAuthIntentSenderForDataset(context, datasetName);
                    datasetBuilder.SetAuthentication(sender);
                }
                else
                {
                    datasetBuilder = new Dataset.Builder
                        (NewRemoteViews(context.PackageName, datasetName, Resource.Drawable.ic_person_black_24dp));
                }

                var setValueAtLeastOnce = filledAutofillFieldCollection.ApplyToFields(autofillFields, datasetBuilder);
                if (setValueAtLeastOnce)
                {
                    return datasetBuilder.Build();
                }
            }

            return null;
        }

        public static RemoteViews NewRemoteViews(string packageName, string remoteViewsText,
            [DrawableRes] int drawableId)
        {
            var presentation = new RemoteViews(packageName, Resource.Layout.multidataset_service_list_item);
            presentation.SetTextViewText(Resource.Id.text, remoteViewsText);
            presentation.SetImageViewResource(Resource.Id.icon, drawableId);
            return presentation;
        }

        /**
         * Wraps autofill data in a Response object (essentially a series of Datasets) which can then
         * be sent back to the client View.
         */
        public static FillResponse NewResponse(Context context,
            bool datasetAuth, AutofillFieldMetadataCollection autofillFields,
            Dictionary<string, FilledAutofillFieldCollection> clientFormDataMap)
        {
            FillResponse.Builder responseBuilder = new FillResponse.Builder();
            if (clientFormDataMap != null)
            {
                var datasetNames = clientFormDataMap.Keys;
                foreach (var datasetName in datasetNames)
                {
                    var filledAutofillFieldCollection = clientFormDataMap[datasetName];
                    if (filledAutofillFieldCollection != null)
                    {
                        var dataset = NewDataset(context, autofillFields,
                            filledAutofillFieldCollection, datasetAuth);
                        if (dataset != null)
                        {
                            responseBuilder.AddDataset(dataset);
                        }
                    }
                }
            }

            if (autofillFields.GetSaveType() != 0)
            {
                AutofillId[] autofillIds = autofillFields.GetAutofillIds();
                responseBuilder.SetSaveInfo
                    (new SaveInfo.Builder((SaveDataType) autofillFields.GetSaveType(), autofillIds).Build());
                return responseBuilder.Build();
            }

            Log.Debug(CommonUtil.TAG, "These fields are not meant to be saved by autofill.");
            return null;
        }
    }
}