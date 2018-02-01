using System.Collections.Generic;
using AutofillService.Model;

namespace AutofillService.Data.Source
{
	public interface IDefaultFieldTypesSource
	{
		List<DefaultFieldTypeWithHints> GetDefaultFieldTypes();
	}
}