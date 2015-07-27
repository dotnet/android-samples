namespace Topeka.Helpers
{
	public static class JsonAttributes
	{
		public static class CategoryFields
		{
			public const string Name = "name";
			public const string Theme = "theme";
			public const string Solved = "solved";
			public const string Scores = "scores";
		}

		public static class QuizFields
		{
			public const string Type = "type";
			public const string Question = "question";
			public const string Answer = "answer";
			public const string Options = "options";
			public const string Min = "min";
			public const string Max = "max";
			public const string Start = "start";
			public const string End = "end";
			public const string Step = "step";
			public const string Solved = "solved";
		}

		public static class QuizTypes
		{
			public const string AlphaPicker = "alpha-picker";
			public const string FillBlank = "fill-blank";
			public const string FillTwoBlanks = "fill-two-blanks";
			public const string FourQuarter = "four-quarter";
			public const string MultiSelect = "multi-select";
			public const string Picker = "picker";
			public const string SingleSelect = "single-select";
			public const string SingleSelectItem = "single-select-item";
			public const string ToggleTranslate = "toggle-translate";
			public const string TrueFalse = "true-false";
		}
	}
}

