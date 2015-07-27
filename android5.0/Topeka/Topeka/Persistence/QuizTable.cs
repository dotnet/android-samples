using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Topeka.Persistence
{
	[Table ("quiz")]
	public class QuizTable
	{
		[PrimaryKey, AutoIncrement]
		public int _id { get; set; }

		[ForeignKey (typeof(CategoryTable))]
		public string Category { get; set; }

		[NotNull]
		public string Type { get; set; }

		[NotNull]
		public string Question { get; set; }

		[NotNull]
		public string Answer { get; set; }

		public string Options { get; set; }

		public string Min { get; set; }

		public string Max { get; set; }

		public string Step { get; set; }

		public string Start { get; set; }

		public string End { get; set; }

		public string Solved { get; set; }
	}
}

