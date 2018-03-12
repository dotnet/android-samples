using Android.Content;

namespace AutofillService.Datasource
{
	public interface IDigitalAssetLinksDataSource
	{
		/**
		 * Checks if the association between a web domain and a package is valid.
		 */
		bool IsValid(Context context, string webDomain, string packageName);

		/**
		 * Clears all cached data.
		 */
		void Clear(Context context);
	}
}
