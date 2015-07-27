using SQLite;

namespace Topeka.Persistence
{
	[Table ("category")]
	public class CategoryTable
	{
		[PrimaryKey]
		public string Id { get; set; }

		[NotNull]
		public string Name { get; set; }

		[NotNull]
		public string Theme { get; set; }

		[NotNull]
		public string Solved { get; set; }

		public string Scores { get; set; }
	}
}

