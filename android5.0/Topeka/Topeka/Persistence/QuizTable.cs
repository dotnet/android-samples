using System;
using Android.Provider;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Topeka.Persistence
{
    [Table("quiz")]
	public class QuizTable
	{
        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }
        [ForeignKey(typeof(CategoryTable))]
        public string category { get; set; }
        [NotNull]
        public string type { get; set; }
        [NotNull]
        public string question { get; set; }
        [NotNull]
        public string answer { get; set; }
        public string options { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public string step { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string solved { get; set; }
	}
}

