using System;
using System.Collections.Generic;

namespace AutofillService.Model
{
	public class DefaultFieldTypeWithHints
	{
		public DefaultFieldType fieldType;
		public List<String> autofillHints;

		public class DefaultFieldType
		{
			public String typeName;
			public List<int> autofillTypes;
			public int saveInfo;
			public int partition;
			public DefaultFakeData fakeData;
		}

		public class DefaultFakeData
		{
			public List<String> strictExampleSet;
			public String textTemplate;
			public String dateTemplate;

			public DefaultFakeData(List<String> strictExampleSet, String textTemplate,
				String dateTemplate)
			{
				this.strictExampleSet = strictExampleSet;
				this.textTemplate = textTemplate;
				this.dateTemplate = dateTemplate;
			}
		}
	}
}