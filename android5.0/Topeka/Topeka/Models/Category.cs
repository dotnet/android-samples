using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Util;

using Java.Interop;

using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Helpers
{
	public class Category : Java.Lang.Object, IParcelable
	{
		const int GoodScore = 8;
		const int NoScore = 0;
		public const string TAG = "Category";

		public string Name { get; private set; }

		public string Id { get; private set; }

		public Theme Theme { get; private set; }

		public List<Quiz> Quizzes { get; private set; }

		public int[] Scores { get; private set; }

		public bool Solved { get; set; }

		public int Score {
			get {
				var categoryScore = 0;
				foreach (var quizScore in Scores)
					categoryScore += quizScore;

				return categoryScore;
			}
		}

		[ExportField ("CREATOR")]
		public static Creator<Category> InitializeCreator ()
		{
			var creator = new Creator<Category> ();
			creator.Created += (sender, e) => e.Result = new Category (e.Source);
			return creator;
		}

		public Category (string name, string id, Theme theme, List<Quiz> quizzes, bool solved)
		{
			Name = name;
			Id = id;
			Theme = theme;
			Quizzes = quizzes;
			Scores = new int[quizzes.Count];
			Solved = solved;
		}

		public Category (string name, string id, Theme theme, List<Quiz> quizzes, int[] scores, bool solved)
		{
			Name = name;
			Id = id;
			Theme = theme;

			if (quizzes.Count == scores.Length) {
				Quizzes = quizzes;
				Scores = scores;
			} else {
				throw new InvalidOperationException ("Quizzes and scores must have the same length");
			}

			Solved = solved;
		}

		protected Category (Parcel inObj)
		{
			Name = inObj.ReadString ();
			Id = inObj.ReadString ();
			//TODO
//			Theme = (Theme)System.Enum.GetValues ()inObj.ReadInt ();
			Quizzes = new List<Quiz> ();
			inObj.ReadTypedList (Quizzes, Quiz.InitializeCreator ());
			Scores = inObj.CreateIntArray ();
			Solved = ParcelableHelper.ReadBoolean (inObj);
		}

		public int GetScore (Quiz which)
		{
			try {
				return Scores [Quizzes.IndexOf (which)];
			} catch (IndexOutOfRangeException) {
				return 0;
			}
		}

		public void SetScore (Quiz which, bool correctlySolved)
		{
			var index = Quizzes.IndexOf (which);
			Log.Debug (TAG, string.Format ("Setting score for {0} with index {1}", which, index));
			if (index == -1)
				return;
			
			Scores [index] = correctlySolved ? GoodScore : NoScore;
		}

		public bool IsSolvedCorrectly (Quiz quiz)
		{
			return GetScore (quiz) == GoodScore;
		}

		public int GetFirstUnsolvedQuizPosition ()
		{
			if (Quizzes == null)
				return -1;
			
			for (int i = 0; i < Quizzes.Count; i++)
				if (!Quizzes [i].Solved)
					return i;
			
			return Quizzes.Count;
		}

		public override string ToString ()
		{
			return string.Format ("Category{name=\'{0}\', id=\'{1}\', theme={2}, quizzes={3}, scores={4}, solved={5}",
						Name, Id, Theme, Quizzes, string.Concat (Scores), Solved);
		}

		public int DescribeContents ()
		{
			return 0;
		}

		public void WriteToParcel (Parcel dest, ParcelableWriteFlags flags)
		{
			dest.WriteString (Name);
			dest.WriteString (Id);
			dest.WriteInt (Theme.Ordinal ());
			dest.WriteTypedList (Quizzes);
			dest.WriteIntArray (Scores);
			ParcelableHelper.WriteBoolean (dest, Solved);
		}

		public override bool Equals (object obj)
		{
			if (this == obj)
				return true;

			if (obj == null || GetType () != obj.GetType ())
				return false;

			var category = (Category)obj;

			if (Id != category.Id || Name != category.Name || !Quizzes.SequenceEqual (category.Quizzes))
				return false;
			return Theme == category.Theme;
		}

		public override int GetHashCode ()
		{
			int result = Name.GetHashCode ();
			result = 31 * result + Id.GetHashCode ();
			result = 31 * result + Theme.GetHashCode ();
			result = 31 * result + Quizzes.GetHashCode ();
			return result;
		}
	}
}

