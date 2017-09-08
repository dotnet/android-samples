using System;
using System.Collections.Generic;
using Android.Content;
using AutofillFramework.multidatasetservice.model;
using GoogleGson;

namespace AutofillFramework.multidatasetservice.datasource
{
	/// <summary>
	/// Singleton autofill data repository that stores autofill fields to SharedPreferences.
	/// Disclaimer: you should not store sensitive fields like user data unencrypted. This is done
	/// here only for simplicity and learning purposes.
	/// </summary>
	public class SharedPrefsAutofillRepository : IAutofillRepository
	{
		static string SHARED_PREF_KEY = "com.example.android.autofillframework.service";
    	static string CLIENT_FORM_DATA_KEY = "loginCredentialDatasets";
    	static string DATASET_NUMBER_KEY = "datasetNumber";

		static SharedPrefsAutofillRepository sInstance;

		ISharedPreferences Prefs;

    	SharedPrefsAutofillRepository(Context context)
		{
			Prefs = context.ApplicationContext.GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private);
		}

		public static SharedPrefsAutofillRepository GetInstance(Context context)
		{
			if (sInstance == null)
			{
				sInstance = new SharedPrefsAutofillRepository(context);
			}
			return sInstance;
		}

		public void Clear()
		{
			Prefs.Edit().Remove(CLIENT_FORM_DATA_KEY).Apply();
		}

		public Dictionary<string, FilledAutofillFieldCollection> GetFilledAutofillFieldCollection(List<string> focusedAutofillHints, List<string> allAutofillHints)
		{
			var hasDataForFocusedAutofillHints = false;
			var clientFormDataMap = new Dictionary<string, FilledAutofillFieldCollection>();
			var clientFormDataStringSet = GetAllAutofillDataStringSet();
			foreach (string clientFormDataString in clientFormDataStringSet)
			{
				Gson gson = new GsonBuilder().ExcludeFieldsWithoutExposeAnnotation().Create();
				var obj = gson.FromJson(clientFormDataString, Java.Lang.Class.FromType(typeof(FilledAutofillFieldCollection)));
				FilledAutofillFieldCollection filledAutofillFieldCollection = obj;
            	if (filledAutofillFieldCollection != null) {
	                if (filledAutofillFieldCollection.HelpsWithHints(focusedAutofillHints)) {
	                    // Saved data has data relevant to at least 1 of the hints associated with the
	                    // View in focus.
	                    hasDataForFocusedAutofillHints = true;
	                }
	                if (filledAutofillFieldCollection.HelpsWithHints(allAutofillHints)) {
	                    // Saved data has data relevant to at least 1 of these hints associated with any
	                    // of the Views in the hierarchy.
						clientFormDataMap.Add(filledAutofillFieldCollection.DatasetName, filledAutofillFieldCollection);
	                }
            	}
        	}
			if (hasDataForFocusedAutofillHints)
			{
				return clientFormDataMap;
			}
			return null;
		}

		public void SaveFilledAutofillFieldCollection(FilledAutofillFieldCollection filledAutofillFieldCollection)
		{
			var datasetName = "dataset-" + GetDatasetNumber();
			filledAutofillFieldCollection.DatasetName = datasetName;
			ICollection<string> allAutofillData = GetAllAutofillDataStringSet();
			Gson gson = new GsonBuilder().ExcludeFieldsWithoutExposeAnnotation().Create();
			//allAutofillData.Add(gson.ToJson(filledAutofillFieldCollection));
			SaveAllAutofillDataStringSet(allAutofillData);
			IncrementDatasetNumber();
		}

		ICollection<string> GetAllAutofillDataStringSet()
		{
			return Prefs.GetStringSet(CLIENT_FORM_DATA_KEY, new HashSet<String>());
		}

		void SaveAllAutofillDataStringSet(ICollection<string> allAutofillDataStringSet)
		{
			Prefs.Edit().PutStringSet(CLIENT_FORM_DATA_KEY, allAutofillDataStringSet).Apply();
		}

		/// <summary>
		/// For simplicity, datasets will be named in the form "dataset-X" where X means
		/// this was the Xth dataset saved.
		/// </summary>
		/// <returns>The dataset number.</returns>
		int GetDatasetNumber()
		{
			return Prefs.GetInt(DATASET_NUMBER_KEY, 0);
		}

		/// <summary>
		/// Every time a dataset is saved, this should be called to increment the dataset number.
		/// (only important for this service's dataset naming scheme).
		/// </summary>
		void IncrementDatasetNumber()
		{
			Prefs.Edit().PutInt(DATASET_NUMBER_KEY, GetDatasetNumber() + 1).Apply();
		}

	}
}
