using System.IO;
using Android.Content.PM;
using Java.Lang;
using Java.Security;
using Java.Security.Cert;
using String = System.String;
using StringBuilder = Java.Lang.StringBuilder;


namespace AutofillService
{
	public static class SecurityHelper
	{

		/**
		 * Gets the fingerprint of the signed certificate of a package.
		 */
		public static String GetFingerprint(PackageInfo packageInfo, String packageName)
		{
			var signatures = packageInfo.Signatures;
			if (signatures.Count != 1)
			{
				throw new SecurityException(packageName + " has " + signatures.Count + " signatures");
			}
			byte[] cert = signatures[0].ToByteArray();
			using (Stream input = new MemoryStream(cert))
			{
				CertificateFactory factory = CertificateFactory.GetInstance("X509");
				X509Certificate x509 = (X509Certificate)factory.GenerateCertificate(input);
				MessageDigest md = MessageDigest.GetInstance("SHA256");
				byte[] publicKey = md.Digest(x509.GetEncoded());
				return ToHexFormat(publicKey);
			}
		}

		private static String ToHexFormat(byte[] bytes)
		{
			var builder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				String hex = Integer.ToHexString(bytes[i]);
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
