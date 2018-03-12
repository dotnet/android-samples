using System.Collections.Generic;
using Android.Content.PM;
using AutofillService.datasource;
using AutofillService.Model;
using Java.Lang;
using Square.Retrofit2;
using Exception = System.Exception;
using String = System.String;

namespace AutofillService.Data.Source.Local
{
	public class DigitalAssetLinksRepository : IDigitalAssetLinksDataSource
	{

		private static String DAL_BASE_URL = "https://digitalassetlinks.googleapis.com";
		private static String PERMISSION_GET_LOGIN_CREDS = "common.get_login_creds";
		private static String PERMISSION_HANDLE_ALL_URLS = "common.handle_all_urls";
		private static DigitalAssetLinksRepository sInstance;

		private IDalService mDalService;
		private PackageManager mPackageManager;

		private DalInfo dalInfo;
		private DalCheck dalCheck;
		private Dictionary<DalInfo, DalCheck> mCache;
		private IDataCallback<DalCheck> dalCheckCallback;
		private Util.DalCheckRequirement dalCheckRequirement;

		string webDomain;
		string fingerprint;
		string packageName;

		private DigitalAssetLinksRepository(PackageManager packageManager)
		{
			mPackageManager = packageManager;
			mCache = new Dictionary<DalInfo, DalCheck>();
			mDalService = (IDalService)new Retrofit.Builder()
				.BaseUrl(DAL_BASE_URL)
				.Build()
				.Create(Java.Lang.Class.FromType(typeof(IDalService)));
		}

		public static DigitalAssetLinksRepository GetInstance(PackageManager packageManager)
		{
			if (sInstance == null)
			{
				sInstance = new DigitalAssetLinksRepository(packageManager);
			}
			return sInstance;
		}

		public static string GetCanonicalDomain(string domain)
		{
			string idn = domain;
			//InternetDomainName idn = InternetDomainName.from(domain);
			//while (idn != null && !idn.isTopPrivateDomain())
			//{
			//	idn = idn.parent();
			//}
			return idn == null ? null : idn.ToString();
		}
		public void Clear()
		{
			mCache.Clear();
		}



		public void CheckValid(Util.DalCheckRequirement dalCheckRequirement, DalInfo dalInfo, IDataCallback<DalCheck> dalCheckCallback)
		{

			this.dalCheckCallback = dalCheckCallback;
			this.dalCheckRequirement = dalCheckRequirement;

			if (dalCheckRequirement.Equals(Util.DalCheckRequirement.Disabled))
			{
				dalCheck = new DalCheck();
				dalCheck.linked = true;
				dalCheckCallback.OnLoaded(dalCheck);
				return;
			}

			dalCheck = mCache[dalInfo];
			if (dalCheck != null)
			{
				dalCheckCallback.OnLoaded(dalCheck);
				return;
			}
			packageName = dalInfo.PackageName;
			webDomain = dalInfo.WebDomain;

			try
			{
				var packageInfo = mPackageManager.GetPackageInfo(packageName, PackageInfoFlags.Signatures);
				fingerprint = SecurityHelper.GetFingerprint(packageInfo, packageName);
			}
			catch (Exception e)
			{
				dalCheckCallback.OnDataNotAvailable("Error getting fingerprint for %s",
					packageName);
				return;
			}
			Util.Logd("validating domain %s for pkg %s and fingerprint %s.", webDomain,
				packageName, fingerprint);
			mDalService.Check(webDomain, PERMISSION_GET_LOGIN_CREDS, packageName, fingerprint).Enqueue(new MyCallBack { repository = this });
		}


		public class MyCallBack : Object, ICallback
		{
			public DigitalAssetLinksRepository repository;

			public void OnFailure(ICall call, Throwable response)
			{
				repository.mDalService.Check(repository.webDomain, PERMISSION_HANDLE_ALL_URLS, repository.packageName, repository.fingerprint);
			}

			public void OnResponse(ICall call, Response response)
			{
				var dalCheck = (DalCheck)response.Body();
				if (dalCheck == null || !dalCheck.linked)
				{
					// get_login_creds check failed, so try handle_all_urls check
					if (repository.dalCheckRequirement.Equals(Util.DalCheckRequirement.LoginOnly))
					{
						repository.dalCheckCallback.OnDataNotAvailable(
							"DAL: Login creds check failed.");
					}
					else if (repository.dalCheckRequirement.Equals(Util.DalCheckRequirement.AllUrls))
					{
						repository.mDalService.Check(repository.webDomain, PERMISSION_HANDLE_ALL_URLS, repository.packageName,
							repository.fingerprint).Enqueue(new DalServiceCallBack { repository = repository });
					}
				}
				else
				{
					// get_login_creds check succeeded, so we're finished.
					repository.mCache.Add(repository.dalInfo, dalCheck);
					repository.dalCheckCallback.OnLoaded(dalCheck);
				}
			}
		}

		public class DalServiceCallBack : Object, ICallback
		{
			public DigitalAssetLinksRepository repository;
			public void OnFailure(ICall p0, Throwable p1)
			{
				repository.dalCheckCallback.OnDataNotAvailable(p1.Message);
			}

			public void OnResponse(ICall p0, Response response)
			{
				var dalCheck = (DalCheck)response.Body();
				repository.mCache.Add(repository.dalInfo, dalCheck);
				repository.dalCheckCallback.OnLoaded(dalCheck);
			}
		}
	}
}