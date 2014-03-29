using System;
using System.Linq;
using System.Collections.Generic;

namespace SwipeToRefresh
{
	public enum BodyType {
		Markdown
	}

	public class Forum
	{
		public string CategoryID { get; set; }
		public int CountDiscussions { get; set; }
		public Discussion[] Discussions { get; set; }
	}

	public class Discussion
	{
		public string DiscussionID { get; set; }
		public string Name { get; set; }
		public string Body { get; set; }
		public string Url { get; set; }
		public BodyType Format { get; set; }
		public DateTime DateInserted { get; set; }
		public DateTime? DateUpdated { get; set; }
		public DateTime DateLastComment { get; set; }

		public int CountComments { get; set; }
		public int CountViews { get; set; }

		public bool Read { get; set; }

		public string FirstName { get; set; }
		public string FirstPhoto { get; set; }
		public string LastName { get; set; }
		public string LastPhoto { get; set; }
	}
}

