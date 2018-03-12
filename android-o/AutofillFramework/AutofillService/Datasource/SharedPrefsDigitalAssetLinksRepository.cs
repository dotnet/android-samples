using System;
using Android.Content;
using Android.Util;

namespace AutofillService.Datasource
{
    public class SharedPrefsDigitalAssetLinksRepository : IDigitalAssetLinksDataSource
    {
        private static SharedPrefsDigitalAssetLinksRepository sInstance;

        private SharedPrefsDigitalAssetLinksRepository()
        {
        }

        public static SharedPrefsDigitalAssetLinksRepository GetInstance()
        {
            return sInstance ?? (sInstance = new SharedPrefsDigitalAssetLinksRepository());
        }

        public bool IsValid(Context context, string webDomain, string packageName)
        {
            // TODO: implement caching. It could cache the whole domain -> (packagename, fingerprint),
            // but then either invalidate when the package change or when the DAL association times out
            // (the maxAge is part of the API response), or document that a real-life service
            // should do that.

            string fingerprint;
            try
            {
                fingerprint = SecurityHelper.GetFingerprint(context, packageName);
            }
            catch (Exception e)
            {
                Log.Warn(CommonUtil.TAG, "error getting fingerprint for " + packageName, e);
                return false;
            }

            return SecurityHelper.IsValid(webDomain, packageName, fingerprint);
        }

        public void Clear(Context context)
        {
            // TODO: implement once if caches results or remove from the interface
        }
    }
}