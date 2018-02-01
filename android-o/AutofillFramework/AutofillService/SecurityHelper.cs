using System.IO;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using Java.Net;
using Java.Security;
using Java.Security.Cert;
using Org.Json;
using StringBuilder = Java.Lang.StringBuilder;


namespace AutofillService
{
    public class SecurityHelper
    {
        private static string REST_TEMPLATE =
            "https://digitalassetlinks.googleapis.com/v1/assetlinks:check?"
            + "source.web.site=%s&relation=delegate_permission/%s"
            + "&target.android_app.package_name=%s"
            + "&target.android_app.certificate.sha256_fingerprint=%s";

        private static string PERMISSION_GET_LOGIN_CREDS = "common.get_login_creds";
        private static string PERMISSION_HANDLE_ALL_URLS = "common.handle_all_urls";

        private SecurityHelper()
        {
            throw new UnsupportedOperationException("provides static methods only");
        }

        private static bool IsValidSync(string webDomain, string permission, string packageName, string fingerprint)
        {
            if (CommonUtil.DEBUG)
                Log.Debug(CommonUtil.TAG, "validating domain " + webDomain + " for pkg " + packageName
                                          + " and fingerprint " + fingerprint + " for permission" + permission);
            if (!webDomain.StartsWith("http:") && !webDomain.StartsWith("https:"))
            {
                // Unfortunately AssistStructure.ViewNode does not tell what the domain is, so let's
                // assume it's https
                webDomain = "https://" + webDomain;
            }

            var restUrl = string.Format(REST_TEMPLATE, webDomain, permission, packageName, fingerprint);
            if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "DAL REST request: " + restUrl);

            HttpURLConnection urlConnection = null;
            var output = new StringBuilder();
            try
            {
                var url = new URL(restUrl);
                urlConnection = (HttpURLConnection) url.OpenConnection();
                using (BufferedReader reader = new BufferedReader(new InputStreamReader(urlConnection.InputStream)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        output.Append(line);
                    }
                }

                var response = output.ToString();
                if (CommonUtil.VERBOSE) Log.Verbose(CommonUtil.TAG, "DAL REST Response: " + response);

                var jsonObject = new JSONObject(response);
                bool valid = jsonObject.OptBoolean("linked", false);
                if (CommonUtil.DEBUG) Log.Debug(CommonUtil.TAG, "Valid: " + valid);

                return valid;
            }
            catch (Exception e)
            {
                throw new RuntimeException("Failed to validate", e);
            }
            finally
            {
                urlConnection?.Disconnect();
            }
        }

        private static bool IsValidSync(string webDomain, string packageName, string fingerprint)
        {
            var isValid = IsValidSync(webDomain, PERMISSION_GET_LOGIN_CREDS, packageName, fingerprint);
            if (!isValid)
            {
                // Ideally we should only check for the get_login_creds, but not all domains set
                // it yet, so validating for handle_all_urls gives a higher coverage.
                if (CommonUtil.DEBUG)
                {
                    Log.Debug(CommonUtil.TAG, PERMISSION_GET_LOGIN_CREDS + " validation failed; trying "
                                                                         + PERMISSION_HANDLE_ALL_URLS);
                }

                isValid = IsValidSync(webDomain, PERMISSION_HANDLE_ALL_URLS, packageName, fingerprint);
            }

            return isValid;
        }

        public static string GetCanonicalDomain(string domain)
        {
            //TODO: rewrite using C# classes
            //var idn = InternetDomainName.from(domain);
            //while (idn != null && !idn.isTopPrivateDomain())
            //{
            //    idn = idn.parent();
            //}
            //return idn == null ? null : idn.toString();
            return domain;
        }

        public static bool IsValid(string webDomain, string packageName, string fingerprint)
        {
            string canonicalDomain = GetCanonicalDomain(webDomain);
            if (CommonUtil.DEBUG)
                Log.Debug(CommonUtil.TAG, "validating domain " + canonicalDomain + " (" + webDomain
                                          + ") for pkg " + packageName + " and fingerprint " + fingerprint);
            string fullDomain;
            if (!webDomain.StartsWith("http:") && !webDomain.StartsWith("https:"))
            {
                // Unfortunately AssistStructure.ViewNode does not tell what the domain is, so let's
                // assume it's https
                fullDomain = "https://" + canonicalDomain;
            }
            else
            {
                fullDomain = canonicalDomain;
            }

            // TODO: use the DAL Java API or a better REST alternative like Volley
            // and/or document it should not block until it returns (for example, the server could
            // start parsing the structure while it waits for the result.
            var task = new AsyncTask1()
            {
                fullDomain = fullDomain,
                packageName = packageName,
                fingerprint = fingerprint
            };
            try
            {
                return (bool) task.Execute((string[]) null).Get();
            }
            catch (InterruptedException ex)
            {
                Thread.CurrentThread().Interrupt();
                Log.Warn(CommonUtil.TAG, "Thread interrupted");
            }
            catch (Exception e)
            {
                Log.Warn(CommonUtil.TAG, "Async task failed", e);
            }

            return false;
        }

        public class AsyncTask1 : AsyncTask<string, int, bool>
        {
            public string fullDomain;
            public string packageName;
            public string fingerprint;

            protected override bool RunInBackground(params string[] @params)
            {
                return IsValidSync(fullDomain, packageName, fingerprint);
            }
        }

        /**
         * Gets the fingerprint of the signed certificate of a package.
         */
        public static string GetFingerprint(Context context, string packageName)
        {
            var pm = context.PackageManager;
            var packageInfo = pm.GetPackageInfo(packageName, PackageInfoFlags.Signatures);
            var signatures = packageInfo.Signatures;
            if (signatures.Count != 1)
            {
                throw new SecurityException(packageName + " has " + signatures.Count + " signatures");
            }

           var cert = signatures[0].ToByteArray();
            using(Stream input = new MemoryStream(cert)) {
                var factory = CertificateFactory.GetInstance("X509");
                var x509 = (X509Certificate) factory.GenerateCertificate(input);
                var md = MessageDigest.GetInstance("SHA256");
                var publicKey = md.Digest(x509.GetEncoded());
                return ToHexFormat(publicKey);
            }
        }

        private static string ToHexFormat(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = Integer.ToHexString(bytes[i]);
                int length = hex.Length;
                if (length == 1)
                {
                    hex = "0" + hex;
                }
                if (length > 2)
                {
                    hex = hex.Substring(length - 2, length);
                }
                builder.Append(hex.ToUpper());
                if (i < (bytes.Length - 1))
                {
                    builder.Append(':');
                }
            }
            return builder.ToString();
        }
    }
}