using Square.Retrofit2.Http;
using Square.Retrofit2;

namespace AutofillService.Data.Source
{
	public interface IDalService
	{
		[GET(Value = "/v1/assetlinks:check")]
		ICall Check(
			[Query(Value = "source.web.site")] string webDomain,
			[Query(Value = "relation")] string permission,
			[Query(Value = "target.android_app.package_name")] string packageName,
			[Query(Value = "target.android_app.certificate.sha256_fingerprint")] string fingerprint
		);
	}
}