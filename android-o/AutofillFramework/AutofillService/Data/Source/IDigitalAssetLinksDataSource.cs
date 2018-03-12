using AutofillService.Data;
using AutofillService.Model;

namespace AutofillService.datasource
{
	public interface IDigitalAssetLinksDataSource
	{
		/**
		* Checks if the association between a web domain and a package is valid.
		*/
		void CheckValid(Util.DalCheckRequirement dalCheckRequirement, DalInfo dalInfo, IDataCallback<DalCheck> dalCheckCallback);

		/**
		 * Clears all cached data.
		 */
		void Clear();
	}
}
