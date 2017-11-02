using System;
using Android.Content;

namespace AutofillFramework.multidatasetservice.settings
{
	public class MyPreferences
	{
	    static String RESPONSE_AUTH_KEY = "response_auth";
	    static String DATASET_AUTH_KEY = "dataset_auth";
	    static String MASTER_PASSWORD_KEY = "master_password";

		static MyPreferences Instance;
		ISharedPreferences Prefs;

		MyPreferences(Context context)
		{
			Prefs = context.ApplicationContext.GetSharedPreferences("my-settings", FileCreationMode.Private);
		}

		public static MyPreferences GetInstance(Context context)
		{
			if (Instance == null)
			{
				Instance = new MyPreferences(context);
			}
			return Instance;
		}

		/// <summary>
		/// Gets whether FillResponses should require authentication.
		/// </summary>
		/// <returns><c>true</c>, if response auth was ised, <c>false</c> otherwise.</returns>
		public bool IsResponseAuth()
		{
			return Prefs.GetBoolean(RESPONSE_AUTH_KEY, false);
		}

		/// <summary>
		/// Enables/disables authentication for the entire autofill {@link FillResponse}.
		/// </summary>
		/// <param name="responseAuth">If set to <c>true</c> response auth.</param>
		public void SetResponseAuth(bool responseAuth)
		{
			Prefs.Edit().PutBoolean(RESPONSE_AUTH_KEY, responseAuth).Apply();
		}

		/// <summary>
		/// Gets whether Datasets should require authentication.
		/// </summary>
		/// <returns><c>true</c>, if dataset auth was ised, <c>false</c> otherwise.</returns>
		public bool IsDatasetAuth()
		{
			return Prefs.GetBoolean(DATASET_AUTH_KEY, false);
		}

		/// <summary>
		/// Enables/disables authentication for individual autofill Datasets.
		/// </summary>
		/// <param name="datasetAuth">If set to <c>true</c> dataset auth.</param>
		public void SetDatasetAuth(bool datasetAuth)
		{
			Prefs.Edit().PutBoolean(DATASET_AUTH_KEY, datasetAuth).Apply();
		}

		/// <summary>
		/// Gets autofill master username.
		/// </summary>
		/// <returns>The master password.</returns>
		public string GetMasterPassword()
		{
			return Prefs.GetString(MASTER_PASSWORD_KEY, null);
		}

		/// <summary>
		/// Sets autofill master password.
		/// </summary>
		/// <param name="masterPassword">Master password.</param>
		public void SetMasterPassword(string masterPassword)
		{
			Prefs.Edit().PutString(MASTER_PASSWORD_KEY, masterPassword).Apply();
		}

		public void ClearCredentials()
		{
			Prefs.Edit().Remove(MASTER_PASSWORD_KEY).Apply();
		}
	}
}
