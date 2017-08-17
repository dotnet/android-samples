using System;
using Android.Content;

namespace AutofillFramework.multidatasetservice.settings
{
	public class MyPreferences
	{
		static String TAG = "MyPreferences";

	    static String RESPONSE_AUTH_KEY = "response_auth";
	    static String DATASET_AUTH_KEY = "dataset_auth";
	    static String MASTER_PASSWORD_KEY = "master_password";

		static MyPreferences sInstance;
		ISharedPreferences mPrefs;

		MyPreferences(Context context)
		{
			mPrefs = context.ApplicationContext.GetSharedPreferences("my-settings", FileCreationMode.Private);
		}

		public static MyPreferences GetInstance(Context context)
		{
			if (sInstance == null)
			{
				sInstance = new MyPreferences(context);
			}
			return sInstance;
		}

		/**
     	 * Gets whether {@link FillResponse}s should require authentication.
     	 */
		public bool IsResponseAuth()
		{
			return mPrefs.GetBoolean(RESPONSE_AUTH_KEY, false);
		}

		/**
     	 * Enables/disables authentication for the entire autofill {@link FillResponse}.
     	 */
		public void SetResponseAuth(bool responseAuth)
		{
			mPrefs.Edit().PutBoolean(RESPONSE_AUTH_KEY, responseAuth).Apply();
		}

		/**
     	 * Gets whether {@link Dataset}s should require authentication.
     	 */
		public bool IsDatasetAuth()
		{
			return mPrefs.GetBoolean(DATASET_AUTH_KEY, false);
		}

		/**
     	 * Enables/disables authentication for individual autofill {@link Dataset}s.
     	 */
		public void SetDatasetAuth(bool datasetAuth)
		{
			mPrefs.Edit().PutBoolean(DATASET_AUTH_KEY, datasetAuth).Apply();
		}

		/**
     	 * Gets autofill master username.
     	 */
		public string GetMasterPassword()
		{
			return mPrefs.GetString(MASTER_PASSWORD_KEY, null);
		}

		/**
     	 * Sets autofill master password.
     	 */
		public void SetMasterPassword(string masterPassword)
		{
			mPrefs.Edit().PutString(MASTER_PASSWORD_KEY, masterPassword).Apply();
		}

		public void ClearCredentials()
		{
			mPrefs.Edit().Remove(MASTER_PASSWORD_KEY).Apply();
		}
	}
}
