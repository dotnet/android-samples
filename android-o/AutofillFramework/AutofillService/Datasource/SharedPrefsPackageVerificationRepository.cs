using System;
using Android.Content;
using Android.Util;
using AutofillService.datasource;

namespace AutofillService.Datasource
{
    public class SharedPrefsPackageVerificationRepository : IPackageVerificationDataSource
    {
        private static string SHARED_PREF_KEY = "com.example.android.autofill.service"
                                                + ".datasource.PackageVerificationDataSource";

        private static IPackageVerificationDataSource sInstance;

        private SharedPrefsPackageVerificationRepository()
        {
        }

        public static IPackageVerificationDataSource GetInstance()
        {
            return sInstance ?? (sInstance = new SharedPrefsPackageVerificationRepository());
        }

        public bool PutPackageSignatures(Context context, string packageName)
        {
            string hash;
            try
            {
                hash = SecurityHelper.GetFingerprint(context, packageName);
                Log.Debug(CommonUtil.TAG, "Hash for " + packageName + ": " + hash);
            }
            catch (Exception e)
            {
                Log.Warn(CommonUtil.TAG, "Error getting hash for " + packageName + ": " + e);
                return false;
            }

            if (!СontainsSignatureForPackage(context, packageName))
            {
                // Storage does not yet contain signature for this package name.
                context.ApplicationContext
                    .GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                    .Edit()
                    .PutString(packageName, hash)
                    .Apply();
                return true;
            }

            return СontainsMatchingSignatureForPackage(context, packageName, hash);
        }

        public void Clear(Context context)
        {
            context.ApplicationContext.GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private)
                .Edit()
                .Clear()
                .Apply();
        }

        private bool СontainsSignatureForPackage(Context context, string packageName)
        {
            var prefs = context.ApplicationContext.GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private);
            return prefs.Contains(packageName);
        }

        private bool СontainsMatchingSignatureForPackage(Context context, string packageName, string hash)
        {
            var prefs = context.ApplicationContext.GetSharedPreferences(SHARED_PREF_KEY, FileCreationMode.Private);
            return hash.Equals(prefs.GetString(packageName, null));
        }
    }
}