using System;
using Android.Provider;
using SQLite;

namespace Topeka.Persistence
{
    [Table("category")]
	public class CategoryTable
	{
        [PrimaryKey]
        public string _id { get; set; }
        [NotNull]
        public string name { get; set; }
        [NotNull]
        public string theme { get; set; }
        [NotNull]
        public string solved { get; set; }
        public string scores { get; set; }
	}
}

