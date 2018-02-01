using System;
using AutofillService.Data.Source.Local.Db;

namespace AutofillService.Model
{
	public class FakeData
	{
		public Converters.StringList strictExampleSet;
		public String textTemplate;
		public String dateTemplate;

		public FakeData(Converters.StringList strictExampleSet, String textTemplate, String dateTemplate)
		{
			this.strictExampleSet = strictExampleSet;
			this.textTemplate = textTemplate;
			this.dateTemplate = dateTemplate;
		}
	}
}