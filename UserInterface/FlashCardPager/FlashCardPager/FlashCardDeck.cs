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

namespace FlashCardPager
{
    public class FlashCard
    {
        // Math problem for this flash card:
        public string problem;

        // Answer for this flash card:
        public string answer;

        // Return the math problem string:
        public string Problem { get { return problem; } }

        // Return the answer string:
        public string Answer { get { return answer; } }
    }

    public class FlashCardDeck
    {
        // Math symbols:
        private static string multiply = "\u00D7";
        private static string divide = "\u00F7";
        private static string plus = "\u002B";
        private static string minus = "\u2212";

        // Built-in flash card problems/answers (could be replaced with a database)
        static FlashCard[] builtInFlashCards = {
            new FlashCard { problem = "42 " + divide + " 7",
                            answer = "6" },
            new FlashCard { problem = "9 " + plus + " 3",
                            answer = "12" },
            new FlashCard { problem = "4 " + multiply + " 7",
                            answer = "28" },
            new FlashCard { problem = "85 " + minus + " 9",
                            answer = "76" },
            new FlashCard { problem = "17 " + plus + " -1",
                            answer = "16" },
            new FlashCard { problem = "64 " + multiply + " 2",
                            answer = "128" } 
        };

        // Array of flash cards that make up the flash card deck:
        private FlashCard[] flashCards;

        // Create an instance copy using the built-in flash cards:
        public FlashCardDeck() { flashCards = builtInFlashCards; }

        // Indexer (read only) for accessing a flash card:
        public FlashCard this[int i] { get { return flashCards[i]; } }

        // Returns the number of flash cards in the deck:
        public int NumCards { get { return flashCards.Length; } }
    }
}
