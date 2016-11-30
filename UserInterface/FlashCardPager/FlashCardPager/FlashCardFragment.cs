using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace FlashCardPager
{
    public class FlashCardFragment : Android.Support.V4.App.Fragment
    {
        // Define Bundle keys for the flash card question and answer
        private static string FLASH_CARD_QUESTION = "card_question";
        private static string FLASH_CARD_ANSWER = "card_answer";

        // Empty constructor: a factory method (below) is used instead.
        public FlashCardFragment() { }

        // Static factory method that creates and initializes a new flash card fragment:
        public static FlashCardFragment newInstance(String question, String answer)
        {
            // Instantiate the fragment class:
            FlashCardFragment fragment = new FlashCardFragment();

            // Pass the question and answer to the fragment:
            Bundle args = new Bundle();
            args.PutString(FLASH_CARD_QUESTION, question);
            args.PutString(FLASH_CARD_ANSWER, answer);
            fragment.Arguments = args;

            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Get the question (math problem) and answer for this flash card fragment:
            string question = Arguments.GetString(FLASH_CARD_QUESTION, "");
            string answer = Arguments.GetString(FLASH_CARD_ANSWER, "");

            // Inflate this fragment from the "flashcard_layout":
            View view = inflater.Inflate(Resource.Layout.flashcard_layout, container, false);

            // Locate the question box TextView within the fragment's container:
            TextView questionBox = (TextView)view.FindViewById(Resource.Id.flash_card_question);

            // Load the flash card with the math problem:
            questionBox.Text = question;

            // Create a handler to report the answer when the math problem is tapped:
            questionBox.Click += delegate
            {
                Toast.MakeText(Activity.ApplicationContext,
                        "Answer: " + answer, ToastLength.Short).Show();
            };
            return view;
        }
    }
}
