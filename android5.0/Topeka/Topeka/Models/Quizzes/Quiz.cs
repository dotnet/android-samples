using System;
using Android.OS;
using Java.Lang;
using Topeka.Helpers;
using Android.Util;
using Java.Lang.Reflect;
using Java.Interop;

namespace Topeka.Models.Quizzes
{
    public abstract class Quiz : Java.Lang.Object
    {
        protected const string Tag = "Quiz";

        [ExportField("CREATOR")]
        public static Creator<Quiz> InitializeCreator()
        {
            var creator = new Creator<Quiz>();
            creator.Created += (sender, e) =>
            {
                int ordinal = e.Source.ReadInt();
                QuizType type = QuizType.FromInt(ordinal);
                try
                {
                    var quizType = type.GetType();
                    e.Result = (Java.Lang.Object)Activator.CreateInstance(quizType);
                }
                catch (InstantiationException ex)
                {
                    Log.Error(Tag, "createFromParcel ", ex);
                }
                catch (IllegalAccessException ex)
                {
                    Log.Error(Tag, "createFromParcel ", ex);
                }
                catch (InvocationTargetException ex)
                {
                    Log.Error(Tag, "createFromParcel ", ex);
                }
                catch (NoSuchMethodException ex)
                {
                    Log.Error(Tag, "createFromParcel ", ex);
                }
                throw new UnsupportedOperationException("Could not create Quiz");

            };
            return creator;
        }

        public bool Solved { get; set; }

        public string Question { get; set; }

        public QuizType QuizType { get; set; }

        public int Id
        {
            get
            {
                return Question.GetHashCode();
            }
        }
        public abstract string GetStringAnswer();

        public override int GetHashCode()
        {
            int result = Question.GetHashCode();
            result = 31 * result + QuizType.GetHashCode();
            result = 31 * result + (Solved ? 1 : 0);
            return result;
        }

        public override string ToString()
        {
            return QuizType + ": \"" + Question + "\"";
        }
    }
    public abstract class Quiz<T> : Quiz, IParcelable
    {
        public T Answer { get; set; }

        protected Quiz(string question, T answer, bool solved)
        {
            Question = question;
            Answer = answer;
            Solved = solved;
        }

        protected Quiz(Parcel inObj)
        {
            Question = inObj.ReadString();
            Solved = ParcelableHelper.ReadBoolean(inObj);
        }

        public virtual bool IsAnswerCorrect(T answer)
        {
            return Answer.Equals(answer);
        }

        public virtual int DescribeContents()
        {
            return 0;
        }

        public virtual void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            ParcelableHelper.WriteEnumValue(dest, QuizType.ToEnum());
            dest.WriteString(Question);
            ParcelableHelper.WriteBoolean(dest, Solved);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            var quiz = obj as Quiz<T>;

            if (quiz == null)
            {
                return false;
            }

            if (Solved != quiz.Solved)
            {
                return false;
            }
            if (Answer.Equals(quiz.Answer))
            {
                return false;
            }
            if (Question != quiz.Question)
            {
                return false;
            }
            if (QuizType.JsonName != quiz.QuizType.JsonName)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 31 * base.GetHashCode() + Answer.GetHashCode();
        }
    }
}