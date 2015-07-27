using System;

using Topeka.Helpers;

namespace Topeka.Models.Quizzes
{
	public class QuizType : Java.Lang.Object
	{
		public enum QuizTypeFlags
		{
			AlphaPicker,
			FillBlank,
			FillTwoBlanks,
			FourQuarter,
			MultiSelect,
			Picker,
			SingleSelect,
			SingleSelectItem,
			ToggleTranslate,
			TrueFalse
		}

		public static readonly QuizType AlphaPicker = new QuizType (JsonAttributes.QuizTypes.AlphaPicker, typeof(AlphaPickerQuiz));
		public static readonly QuizType FillBlank = new QuizType (JsonAttributes.QuizTypes.FillBlank, typeof(FillBlankQuiz));
		public static readonly QuizType FillTwoBlanks = new QuizType (JsonAttributes.QuizTypes.FillTwoBlanks, typeof(FillTwoBlanksQuiz));
		public static readonly QuizType FourQuarter = new QuizType (JsonAttributes.QuizTypes.FourQuarter, typeof(FourQuarterQuiz));
		public static readonly QuizType MultiSelect = new QuizType (JsonAttributes.QuizTypes.MultiSelect, typeof(MultiSelectQuiz));
		public static readonly QuizType Picker = new QuizType (JsonAttributes.QuizTypes.Picker, typeof(PickerQuiz));
		public static readonly QuizType SingleSelect = new QuizType (JsonAttributes.QuizTypes.SingleSelect, typeof(SelectItemQuiz));
		public static readonly QuizType SingleSelectItem = new QuizType (JsonAttributes.QuizTypes.SingleSelectItem, typeof(SelectItemQuiz));
		public static readonly QuizType ToggleTranslate = new QuizType (JsonAttributes.QuizTypes.ToggleTranslate, typeof(ToggleTranslateQuiz));
		public static readonly QuizType TrueFalse = new QuizType (JsonAttributes.QuizTypes.TrueFalse, typeof(TrueFalseQuiz));

		public string JsonName { get; private set; }

		public Type Type { get; private set; }

		QuizType (string jsonName, Type type)
		{
			JsonName = jsonName;
			Type = type;
		}

		public static QuizType FromInt (int index)
		{
			var flag = (QuizTypeFlags)Enum.GetValues (typeof(QuizTypeFlags)).GetValue (index);
			switch (flag) {
			case QuizTypeFlags.AlphaPicker:
				return AlphaPicker;
			case QuizTypeFlags.FillBlank:
				return FillBlank;
			case QuizTypeFlags.FillTwoBlanks:
				return FillTwoBlanks;
			case QuizTypeFlags.FourQuarter:
				return FourQuarter;
			case QuizTypeFlags.MultiSelect:
				return MultiSelect;
			case QuizTypeFlags.Picker:
				return Picker;
			case QuizTypeFlags.SingleSelect:
				return SingleSelect;
			case QuizTypeFlags.SingleSelectItem:
				return SingleSelectItem;
			case QuizTypeFlags.ToggleTranslate:
				return ToggleTranslate;
			case QuizTypeFlags.TrueFalse:
				return TrueFalse;
			}
			throw new IndexOutOfRangeException ("invalid index for quiztype");
		}

		public QuizTypeFlags ToEnum ()
		{
			switch (JsonName) {
			case "alpha-picker":
				return QuizTypeFlags.AlphaPicker;
			case "fill-blank":
				return QuizTypeFlags.FillBlank;
			case "fill-two-blanks":
				return QuizTypeFlags.FillTwoBlanks;
			case "four-quarter":
				return QuizTypeFlags.FourQuarter;
			case "multi-select":
				return QuizTypeFlags.MultiSelect;
			case "picker":
				return QuizTypeFlags.Picker;
			case "single-select":
				return QuizTypeFlags.SingleSelect;
			case "single-select-item":
				return QuizTypeFlags.SingleSelectItem;
			case "toggle-translate":
				return QuizTypeFlags.ToggleTranslate;
			case "true-false":
				return QuizTypeFlags.TrueFalse;
			}
			throw new InvalidOperationException ("invalid quiztype (shouldn't be possible)");
		}
	}
}

