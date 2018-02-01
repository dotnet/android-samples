using System;
using AutofillService.Data.Source.Local;

namespace AutofillService.Model
{
	public class DalInfo
	{
		private string mWebDomain;
		private string mPackageName;

		public DalInfo(string webDomain, string packageName)
		{
			String canonicalDomain = DigitalAssetLinksRepository.GetCanonicalDomain(webDomain);
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
			mWebDomain = fullDomain;
			mPackageName = packageName;
		}

		public string WebDomain => mWebDomain;
		public string PackageName => mPackageName;

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if (obj == null || GetType() != obj.GetType()) return false;

			DalInfo dalInfo = (DalInfo)obj;

			if (mWebDomain != null ? !mWebDomain.Equals(dalInfo.mWebDomain) :
				dalInfo.mWebDomain != null)
				return false;
			return mPackageName != null ? mPackageName.Equals(dalInfo.mPackageName) :
				dalInfo.mPackageName == null;

		}

		public override int GetHashCode()
		{
			int result = mWebDomain != null ? mWebDomain.GetHashCode() : 0;
			result = 31 * result + (mPackageName != null ? mPackageName.GetHashCode(): 0);
			return result;
		}

	}
}