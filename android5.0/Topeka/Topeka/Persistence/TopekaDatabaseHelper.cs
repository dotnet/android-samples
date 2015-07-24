using Android.Content;
using Android.Content.Res;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Topeka.Helpers;
using Topeka.Models.Quizzes;

namespace Topeka.Persistence
{
    public class TopekaDatabaseHelper
    {
		const string Tag = "TopekaDatabaseHelper";
		const string DbName = "topeka";
		const string DbSuffix = ".db";
		const int DbVersion = 1;
        SQLiteConnection database;
		static List<Category> categories;
		static TopekaDatabaseHelper instance;
		Resources resources;

        static TopekaDatabaseHelper GetInstance(Context context)
        {
            if (instance == null)
                instance = new TopekaDatabaseHelper(context);
            return instance;
        }
        TopekaDatabaseHelper(Context context)
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName + DbSuffix)))
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName + DbSuffix));
            database = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName + DbSuffix));
            resources = context.Resources;
            OnCreate(database);
        }

        public static List<Category> GetCategories(Context context, bool fromDatabase)
        {
            if (null == categories || fromDatabase)
            {
                categories = LoadCategories(context).ToList();
            }
            return categories;
        }

        static IEnumerable<Category> LoadCategories(Context context)
        {
            var readableDatabase = GetDatabase(context);
            var query = readableDatabase.Table<CategoryTable>().Select(c => c);
            foreach (var category in query)
            {
                yield return new Category(
                    category.name,
                    category._id,
                    Theme.FromString(category.theme),
                    GetQuizzes(category._id, readableDatabase),
                    GetScoresFromString(category.scores),
                    GetBooleanFromDatabase(category.solved)
                    );
            }
        }

        static bool GetBooleanFromDatabase(string isSolved)
        {
			return !string.IsNullOrEmpty (isSolved) && Convert.ToBoolean (isSolved);
        }

        static int[] GetScoresFromString(string scores)
        {
            return scores.Trim(new[] { '[',']', '{', '}'}).Split(',').Select(x => int.Parse(x)).ToArray();
        }

        public static Category GetCategoryWith(Context context, string categoryId)
        {
            var readableDatabase = GetDatabase(context);
            var category = readableDatabase.Get<CategoryTable>(categoryId);
            return new Category(
                    category.name,
                    category._id,
                    Theme.FromString(category.theme),
                    GetQuizzes(category._id, readableDatabase),
                    GetScoresFromString(category.scores),
                    GetBooleanFromDatabase(category.solved));
        }

        public static int GetScore(Context context)
        {
			var tempCategories = GetCategories(context, false);
            int score = 0;
            tempCategories.ForEach(category => score += category.Score);
            return score;
        }

        public static void UpdateCategory(Context context, Category category)
        {
            var writableDatabase = GetDatabase(context);
            var categoryQuery = writableDatabase.Get<CategoryTable>(category.Id);
            categoryQuery.solved = category.Solved.ToString();
            categoryQuery.scores = string.Join(",", category.Scores);
            writableDatabase.Update(categoryQuery);
            var quizzes = category.Quizzes;
            UpdateQuizzes(writableDatabase, quizzes);
        }

        static void UpdateQuizzes(SQLiteConnection writableDatabase, List<Quiz> quizzes)
        {
            foreach (var quiz in quizzes)
            {
                var quizQuery = writableDatabase.Get<QuizTable>(r => r.question == quiz.Question);
                quizQuery.solved = quiz.Solved.ToString();
                quizQuery.question = quiz.Question;
                writableDatabase.Update(quizQuery);
            }
        }

        public static void Reset(Context context)
        {
            var writableDatabase = GetDatabase(context);
            writableDatabase.DeleteAll<CategoryTable>();
            writableDatabase.DeleteAll<QuizTable>();
            GetInstance(context).PreFillDatabase(writableDatabase);
        }

        static List<Quiz> GetQuizzes(string categoryId, SQLiteConnection database)
        {
            var quizzes = new List<Quiz>();
            var queryResults = database.Table<QuizTable>().Where(q => q.category == categoryId);
            foreach (var query in queryResults)
            {
                quizzes.Add(CreateQuizDueToType(query));
            }
            return quizzes;
        }

        static Quiz CreateQuizDueToType(QuizTable table)
        {
            string type = table.type;
            string question = table.question;
            string answer = table.answer;
            string options = table.options;
            int min = Convert.ToInt32(table.min);
            int max = Convert.ToInt32(table.max);
            int step = Convert.ToInt32(table.step);
            bool solved = GetBooleanFromDatabase(table.solved);

            switch (type)
            {
                case JsonAttributes.QuizTypes.AlphaPicker:
                    {
                        return new AlphaPickerQuiz(question, answer, solved);
                    }
                case JsonAttributes.QuizTypes.FillBlank:
                    {
                        return CreateFillBlankQuiz(table, question, answer, solved);
                    }
                case JsonAttributes.QuizTypes.FillTwoBlanks:
                    {
                        return CreateFillTwoBlanksQuiz(question, answer, solved);
                    }
                case JsonAttributes.QuizTypes.FourQuarter:
                    {
                        return CreateFourQuarterQuiz(question, answer, options, solved);
                    }
                case JsonAttributes.QuizTypes.MultiSelect:
                    {
                        return CreateMultiSelectQuiz(question, answer, options, solved);
                    }
                case JsonAttributes.QuizTypes.Picker:
                    {
                        return new PickerQuiz(question, Convert.ToInt32(answer), min, max, step, solved);
                    }
                case JsonAttributes.QuizTypes.SingleSelect:
                case JsonAttributes.QuizTypes.SingleSelectItem:
                    {
                        return CreateSelectItemQuiz(question, answer, options, solved);
                    }
                case JsonAttributes.QuizTypes.ToggleTranslate:
                    {
                        return CreateToggleTranslateQuiz(question, answer, options, solved);
                    }
                case JsonAttributes.QuizTypes.TrueFalse:
                    {
                        return CreateTrueFalseQuiz(question, answer, solved);

                    }
                default:
                    {
                        throw new InvalidOperationException("Quiz type " + type + " is not supported");
                    }
            }
        }

        static Quiz CreateFillBlankQuiz(QuizTable table, string question, string answer,
            bool solved)
        {
            string start = table.start;
            string end = table.end;
            return new FillBlankQuiz(question, answer, start, end, solved);
        }

        static Quiz CreateFillTwoBlanksQuiz(string question, string answer, bool solved)
        {
            String[] answerArray = JsonConvert.DeserializeObject<string[]>(answer);
            return new FillTwoBlanksQuiz(question, answerArray, solved);
        }

        static Quiz CreateFourQuarterQuiz(string question, string answer, string options,
            bool solved)
        {
            int[] answerArray = JsonConvert.DeserializeObject<int[]>(answer);
            String[] optionsArray = JsonConvert.DeserializeObject<string[]>(options);
            return new FourQuarterQuiz(question, answerArray, optionsArray, solved);
        }

        static Quiz CreateMultiSelectQuiz(string question, string answer, string options,
            bool solved)
        {
            int[] answerArray = JsonConvert.DeserializeObject<int[]>(answer);
            String[] optionsArray = JsonConvert.DeserializeObject<string[]>(options);
            return new MultiSelectQuiz(question, answerArray, optionsArray, solved);
        }

        static Quiz CreateSelectItemQuiz(string question, string answer, string options,
            bool solved)
        {
            int[] answerArray = JsonConvert.DeserializeObject<int[]>(answer);
            String[] optionsArray = JsonConvert.DeserializeObject<string[]>(options);
            return new SelectItemQuiz(question, answerArray, optionsArray, solved);
        }

        static Quiz CreateToggleTranslateQuiz(string question, string answer, string options,
            bool solved)
        {
            var answerArray = JsonConvert.DeserializeObject<int[]>(answer);
            var optionsArrays = ExtractOptionsArrays(options);
            return new ToggleTranslateQuiz(question, answerArray, optionsArrays, solved);
        }

        static Quiz CreateTrueFalseQuiz(string question, string answer, bool solved)
        {
            bool answerValue = "true" == answer;
            return new TrueFalseQuiz(question, answerValue, solved);
        }

        static string[] ExtractOptionsArrays(string options)
        {
            var flatList = new List<string>();
            var deserial = JsonConvert.DeserializeObject<string[][]>(options);
            for (int row = 0; row < deserial.Length; row++)
            {
                for (int col = 0; col < deserial[row].Length; col++)
                {
                    flatList.Add(deserial[row][col]);
                }
            }
            return flatList.ToArray();
        }

        static SQLiteConnection GetDatabase(Context context)
        {
            return GetInstance(context).database;
        }

        public void OnCreate(SQLiteConnection db)
        {
            db.CreateTable<CategoryTable>();
            db.CreateTable<QuizTable>();
            PreFillDatabase(db);
        }

        void PreFillDatabase(SQLiteConnection db)
        {
            try
            {
                FillCategoriesAndQuizzes(db);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        void FillCategoriesAndQuizzes(SQLiteConnection db)
        {
            var jsonArray = JArray.Parse(ReadCategoriesFromResources());
            foreach (var category in jsonArray)
            {
                var categoryId = category["id"].ToObject<string>();
                FillCategory(db, category, categoryId);
                var quizzes = (JArray)category["quizzes"];
                FillQuizzesForCategory(db, quizzes, categoryId);
            }
        }

        string ReadCategoriesFromResources()
        {
            var categoriesJson = new StringBuilder();
            using (var stream = resources.OpenRawResource(Resource.Raw.categories))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        categoriesJson.Append(line);
                    }
                }
            }
            return categoriesJson.ToString();
        }

        static void FillCategory(SQLiteConnection db, JToken categoryJson, string categoryId)
        {
            var category = new CategoryTable 
            {
                _id = categoryId,
                name = categoryJson[JsonAttributes.CategoryFields.Name].ToString(),
                theme = categoryJson[JsonAttributes.CategoryFields.Theme].ToString(),
                solved = categoryJson[JsonAttributes.CategoryFields.Solved].ToString(),
                scores = categoryJson[JsonAttributes.CategoryFields.Scores].ToString()
            };
            db.Insert(category);
        }

        static void FillQuizzesForCategory(SQLiteConnection db, JToken quizzes, string categoryId)
        {
            foreach (var quizJson in quizzes)
            {
                var quiz = new QuizTable 
                {
                    category = categoryId,
                    type = quizJson[JsonAttributes.QuizFields.Type].ToString(),
                    question = quizJson[JsonAttributes.QuizFields.Question].ToString(),
                    answer = quizJson[JsonAttributes.QuizFields.Answer].ToString(),
                    options = quizJson[JsonAttributes.QuizFields.Options]?.ToString(),
                    min = quizJson[JsonAttributes.QuizFields.Min]?.ToString(),
                    max = quizJson[JsonAttributes.QuizFields.Max]?.ToString(),
                    start = quizJson[JsonAttributes.QuizFields.Start]?.ToString(),
                    end = quizJson[JsonAttributes.QuizFields.End]?.ToString(),
                    step = quizJson[JsonAttributes.QuizFields.Step]?.ToString(),
                };
                db.Insert(quiz);
            }
        }
    }
}