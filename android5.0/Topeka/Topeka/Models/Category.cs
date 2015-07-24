using System;
using System.Linq;
using Android.OS;
using System.Collections.Generic;
using Android.Util;
using Topeka.Models.Quizzes;
using Topeka.Helpers;
using Java.Lang;
using Java.Interop;

namespace Topeka.Helpers
{
	public class Category : Java.Lang.Object, IParcelable
	{
		public const string TAG = "Category";
        
        [ExportField("CREATOR")]
        public static Creator<Category> InitializeCreator()
        {
            var creator = new Creator<Category>();
            creator.Created += (sender, e) => { e.Result = new Category(e.Source); };
            return creator;
        }

        const int GoodScore = 8;
		const int NoScore = 0;
		readonly string name;
		readonly string id;
		readonly Theme theme;
		readonly int[] scores;
		List<Quiz> quizzes;
		bool solved;
        
		public string Name {
			get {
				return name;
			}
		}

		public string Id {
			get {
				return id;
			}
		}

		public Theme Theme {
			get {
				return theme;
			}
		}

		public List<Quiz> Quizzes {
			get {
				return quizzes;
			}
		}

		public int Score {
			get {
				var categoryScore = 0;
				foreach (var quizScore in scores) {
					categoryScore += quizScore;
				}
				return categoryScore;
			}
		}

		public int[] Scores {
			get {
				return scores;
			}
		}

		public bool Solved {
			get {
				return solved;
			} set {
				solved = value;
			}
		}

        public Category(string name, string id, Theme theme, List<Quiz> quizzes, bool solved) {
			this.name = name;
			this.id = id;
			this.theme = theme;
			this.quizzes = quizzes;
			scores = new int[quizzes.Count];
			this.solved = solved;
		}

		public Category(string name, string id, Theme theme, List<Quiz> quizzes, int[] scores, bool solved) {
			this.name = name;
			this.id = id;
			this.theme = theme;
			if (quizzes.Count == scores.Length) {
				this.quizzes = quizzes;
				this.scores = scores;
			} else {
				throw new InvalidOperationException("Quizzes and scores must have the same length");
			}
			this.solved = solved;
		}

		protected Category(Parcel inObj) {
			name = inObj.ReadString();
			id = inObj.ReadString();
			theme = (Theme)(System.Enum.GetValues(typeof(Theme)).GetValue(inObj.ReadInt()));
			quizzes = new List<Quiz>();
			inObj.ReadTypedList(quizzes, Quiz.InitializeCreator());
			scores = inObj.CreateIntArray();
			solved = ParcelableHelper.ReadBoolean(inObj);
		}


		public int GetScore(Quiz which) {
			try {
				return scores[quizzes.IndexOf(which)];
			} catch (IndexOutOfRangeException) {
				return 0;
			}
		}

		public void SetScore(Quiz which, bool correctlySolved) {
			var index = quizzes.IndexOf(which);
			Log.Debug(TAG, "Setting score for " + which + " with index " + index);
			if (index == -1) {
				return;
			}
			scores[index] = correctlySolved ? GoodScore : NoScore;
		}

		public bool IsSolvedCorrectly(Quiz quiz) {
			return GetScore(quiz) == GoodScore;
		}
			
		public int GetFirstUnsolvedQuizPosition() {
			if (quizzes == null) {
				return -1;
			}
			for (int i = 0; i < quizzes.Count; i++) {
				if (!quizzes[i].Solved) {
					return i;
				}
			}
			return quizzes.Count;
		}

		public override string ToString ()
		{
				return "Category{" +
					"name='" + name + '\'' +
					", id='" + id + '\'' +
					", theme=" + theme +
					", quizzes=" + quizzes +
					", scores=" + string.Concat(scores) +
					", solved=" + solved +
					'}';
		}

		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			dest.WriteString(name);
			dest.WriteString(id);
			dest.WriteInt(Theme.Ordinal());
			dest.WriteTypedList(Quizzes);
			dest.WriteIntArray(scores);
			ParcelableHelper.WriteBoolean(dest, solved);
		}

		public override bool Equals (object obj)
		{
			if (this == obj) {
				return true;
			}
			if (obj == null || GetType () != obj.GetType()) {
				return false;
			}

			var category = (Category) obj;

			if (Id != category.id)
				return false;
			if (Name != category.name)
				return false;
			if (!quizzes.SequenceEqual (category.quizzes))
				return false;
			return theme == category.theme;
		}

		public override int GetHashCode ()
		{
			int result = name.GetHashCode();
			result = 31 * result + id.GetHashCode();
			result = 31 * result + theme.GetHashCode();
			result = 31 * result + quizzes.GetHashCode();
			return result;
		}
	}
}

