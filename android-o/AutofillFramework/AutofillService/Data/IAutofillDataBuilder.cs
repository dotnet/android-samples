using System.Collections.Generic;
using System.Collections.Immutable;
using AutofillService.Model;

namespace AutofillService.Data
{
	public interface IAutofillDataBuilder
	{
		ImmutableList<DatasetWithFilledAutofillFields> BuildDatasetsByPartition(int datasetNumber);
	}
}