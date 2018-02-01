using AutofillService.Model;

namespace AutofillService
{
	public interface IFakeFieldGenerator
	{
		FilledAutofillField Generate(int seed);
	}

}