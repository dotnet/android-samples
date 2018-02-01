using Android.Content;
using Android.Content.PM;
using Java.Lang;

namespace AutofillService.datasource
{
	public class SharedPrefsPackageVerificationRepository : IPackageVerificationDataSource
	{

		private static string SHARED_PREF_KEY = "com.example.android.autofill.service"
		+ ".datasource.PackageVerificationDataSource";
		private static IPackageVerificationDataSource sInstance;

		private ISharedPreferences mSharedPrefs;
		private Context mContext;

		private SharedPrefsPackageVerificationRepository(Context context)
		{
			mSharedPrefs = context.ApplicationContext
				.GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private);
			mContext = context.ApplicationContext;
		}

		public static IPackageVerificationDataSource GetInstance(Context context)
		{
			if (sInstance == null)
			{
				sInstance = new SharedPrefsPackageVerificationRepository(
					context.ApplicationContext);
			}
			return sInstance;
		}

		public bool PutPackageSignatures(Context context, string packageName)
		{
			string hash;
			try
			{
				PackageManager pm = mContext.PackageManager;
				PackageInfo packageInfo = pm.GetPackageInfo(packageName, PackageInfoFlags.Signatures);
				hash = SecurityHelper.GetFingerprint(packageInfo, packageName);
				Util.Logd("Hash for %s: %s", packageName, hash);
			}
			catch (Exception ex)
			{
				Util.Logw(ex, "Error getting hash for %s.", packageName);
				return false;
			}

			if (!ContainsSignatureForPackage(packageName))
			{
				// Storage does not yet contain signature for this package name.
				mSharedPrefs.Edit().PutString(packageName, hash).Apply();
				return true;
			}
			return ContainsMatchingSignatureForPackage(packageName, hash);
		}

		public void Clear(Context context)
		{
			mSharedPrefs.Edit().Clear().Apply();
		}
		private bool ContainsSignatureForPackage(string packageName)
		{
			return mSharedPrefs.Contains(packageName);
		}

		private bool ContainsMatchingSignatureForPackage(string packageName, string hash)
		{
			return hash.Equals(mSharedPrefs.GetString(packageName, null));
		}
	}
}
