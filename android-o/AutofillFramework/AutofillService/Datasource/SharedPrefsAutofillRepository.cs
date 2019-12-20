using System.Collections.Generic;
using Android.Content;
using AutofillService.Model;
//mc++ using GoogleGson;
using Newtonsoft.Json;

namespace AutofillService.Datasource
{
    public class SharedPrefsAutofillRepository : IAutofillDataSource
    {
        //TODO: change
        private static string SHARED_PREF_KEY = "com.example.android.autofill.service.datasource.AutofillDataSource";
        private static string CLIENT_FORM_DATA_KEY = "loginCredentialDatasets";
        private static string DATASET_NUMBER_KEY = "datasetNumber";
        private static SharedPrefsAutofillRepository sInstance;

        private SharedPrefsAutofillRepository()
        {
        }

        public static SharedPrefsAutofillRepository GetInstance()
        {
            return sInstance ?? (sInstance = new SharedPrefsAutofillRepository());
        }


        public Dictionary<string, FilledAutofillFieldCollection> GetFilledAutofillFieldCollection(Context context,
            List<string> focusedAutofillHints, List<string> allAutofillHints)
        {
            bool hasDataForFocusedAutofillHints = false;
            var clientFormDataMap = new Dictionary<string, FilledAutofillFieldCollection>();
            var clientFormDataStringSet = GetAllAutofillDataStringSet(context);
            foreach (var clientFormDataString in clientFormDataStringSet)
            {
                var filledAutofillFieldCollection = JsonConvert.DeserializeObject<FilledAutofillFieldCollection>(clientFormDataString);
                if (filledAutofillFieldCollection != null)
                {
                    if (filledAutofillFieldCollection.HelpsWithHints(focusedAutofillHints))
                    {
                        // Saved data has data relevant to at least 1 of the hints associated with the
                        // View in focus.
                        hasDataForFocusedAutofillHints = true;
                    }

                    if (filledAutofillFieldCollection.HelpsWithHints(allAutofillHints))
                    {
                        // Saved data has data relevant to at least 1 of these hints associated with any
                        // of the Views in the hierarchy.
                        clientFormDataMap.Add(filledAutofillFieldCollection.GetDatasetName(),
                            filledAutofillFieldCollection);
                    }
                }
            }

            if (hasDataForFocusedAutofillHints)
            {
                return clientFormDataMap;
            }

            return null;
        }

        public void SaveFilledAutofillFieldCollection(Context context,
            FilledAutofillFieldCollection filledAutofillFieldCollection)
        {
            var datasetName = "dataset-" + GetDatasetNumber(context);
            filledAutofillFieldCollection.SetDatasetName(datasetName);
            var allAutofillData = GetAllAutofillDataStringSet(context);
            allAutofillData.Add(JsonConvert.SerializeObject(filledAutofillFieldCollection));
            SaveAllAutofillDataStringSet(context, allAutofillData);
            IncrementDatasetNumber(context);
        }

        public void Clear(Context context)
        {
            context.ApplicationContext
                .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .Edit()
                .Remove(CLIENT_FORM_DATA_KEY)
                .Remove(DATASET_NUMBER_KEY)
                .Apply();
        }

        private ICollection<string> GetAllAutofillDataStringSet(Context context)
        {
            return context.ApplicationContext
                .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .GetStringSet(CLIENT_FORM_DATA_KEY, new HashSet<string>());
        }

        private void SaveAllAutofillDataStringSet(Context context, ICollection<string> allAutofillDataStringSet)
        {
            context.ApplicationContext
                .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .Edit()
                .PutStringSet(CLIENT_FORM_DATA_KEY, allAutofillDataStringSet)
                .Apply();
        }

        /**
         * For simplicity, datasets will be named in the form "dataset-X" where X means
         * this was the Xth dataset saved.
         */
        private int GetDatasetNumber(Context context)
        {
            return context.ApplicationContext
                .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .GetInt(DATASET_NUMBER_KEY, 0);
        }

        /**
         * Every time a dataset is saved, this should be called to increment the dataset number.
         * (only important for this service's dataset naming scheme).
         */
        private void IncrementDatasetNumber(Context context)
        {
            context.ApplicationContext
                .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .Edit()
                .PutInt(DATASET_NUMBER_KEY, GetDatasetNumber(context) + 1)
                .Apply();
        }
    }
}