using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace MediaBrowserService
{
	public class PackageValidator
	{
		static readonly string Tag = LogHelper.MakeLogTag(typeof(PackageValidator));

		readonly Dictionary<string, List<CallerInfo>> mValidCertificates;

		public PackageValidator (Context ctx)
		{
			using (var reader = ctx.Resources.GetXml (Resource.Xml.allowed_media_browser_callers)) {
				mValidCertificates = ReadValidCertificates (reader);
			}
		}

		Dictionary<string, List<CallerInfo>> ReadValidCertificates (XmlReader parser)
		{
			var validCertificates = new Dictionary<string, List<CallerInfo>>();
			try {
				while (parser.Read()) {
					if (parser.IsStartElement() && parser.Name == "signing_certificate") {
						string name = parser[0];
						string packageName = parser[2];
						var isRelease = Convert.ToBoolean (parser[1]);
						string certificate = parser.ReadString ();
						if (certificate != null)
							certificate = certificate.Replace ("\\s|\\n", "");

						var info = new CallerInfo (name, packageName, isRelease, certificate);

						List<CallerInfo> infos; 
						validCertificates.TryGetValue (certificate, out infos);
						if (infos == null) {
							infos = new List<CallerInfo>();
							validCertificates.Add (certificate, infos);
						}
						LogHelper.Verbose (Tag, "Adding allowed caller: ", info.Name,
							" package=", info.PackageName, " release=", info.Release,
							" certificate=", certificate);
						infos.Add (info);
					}
				}
			} catch (XmlException e) {
				LogHelper.Error (Tag, e, "Could not read allowed callers from XML.");
			} catch (IOException e) {
				LogHelper.Error (Tag, e, "Could not read allowed callers from XML.");
			}
			return validCertificates;
		}

		public bool IsCallerAllowed (Context context, string callingPackage, int callingUid)
		{
			if (Process.SystemUid == callingUid || Process.MyUid() == callingUid) {
				return true;
			}

			PackageManager packageManager = context.PackageManager;
			PackageInfo packageInfo;
			try {
				packageInfo = packageManager.GetPackageInfo (callingPackage, PackageInfoFlags.Signatures);
			} catch (PackageManager.NameNotFoundException e) {
				LogHelper.Warn (Tag, e, "Package manager can't find package: ", callingPackage);
				return false;
			}
			if (packageInfo.Signatures.Count != 1) {
				LogHelper.Warn (Tag, "Caller has more than one signature certificate!");
				return false;
			}
			var signature = Base64.EncodeToString (packageInfo.Signatures[0].ToByteArray (), Base64Flags.NoWrap);

			List<CallerInfo> validCallers = mValidCertificates[signature];
			if (validCallers == null) {
				LogHelper.Verbose (Tag, "Signature for caller ", callingPackage, " is not valid: \n", signature);
				if (mValidCertificates.Count == 0) {
					LogHelper.Warn (Tag, "The list of valid certificates is empty. Either your file ",
						"res/xml/allowed_media_browser_callers.xml is empty or there was an error ",
						"while reading it. Check previous log messages.");
				}
				return false;
			}

			// Check if the package name is valid for the certificate:
			var expectedPackages = new StringBuilder ();
			foreach (var info in validCallers) {
				if (callingPackage == info.PackageName) {
					LogHelper.Verbose(Tag, "Valid caller: ", info.Name, " package=", 
						info.PackageName, " release=", info.Release);
					return true;
				}
				expectedPackages.Append (info.PackageName).Append (' ');
			}

			LogHelper.Info (Tag, "Caller has a valid certificate, but its package doesn't match any ",
				"expected package for the given certificate. Caller's package is ", callingPackage,
				". Expected packages as defined in res/xml/allowed_media_browser_callers.xml are (",
				expectedPackages, "). This caller's certificate is: \n", signature);
			return false;
		}

		class CallerInfo
		{
			public string Name { get; set; }
			public string PackageName { get; set; }
			public bool Release { get; set; }
			public string SigningCertificate { get; set; }

			public CallerInfo (string name, string packageName, bool release, string signingCertificate)
			{
				Name = name;
				PackageName = packageName;
				Release = release;
				SigningCertificate = signingCertificate;
			}
		}
	}
}

