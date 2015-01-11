using System;
using System.Text;

namespace ElizaChat
{
	public class ElizaResponder
	{
		private static string[] CONVERSATION_KEYWORDS = {
			"CAN YOU", "CAN I", "YOU ARE", "YOURE", "I DONT", "I FEEL", "WHY DONT YOU", "WHY CANT I",
			"ARE YOU", "I CANT", "I AM", " IM ", "YOU", "I WANT", "WHAT", "HOW", "WHO", "WHERE",
			"WHEN", "WHY", "NAME", "CAUSE", "SORRY", "DREAM", "HELLO", "HI", "MAYBE", "NO", "YOUR",
			"ALWAYS", "THINK", "ALIKE", "YES", "FRIEND", "COMPUTER", "NOKEYFOUND"
		};

		private static string[] WORDS_TO_REPLACE = {"ARE", "AM", "WERE", "WAS", "YOU", "I", "YOUR", "MY",
			"IVE", "YOUVE", "IM", "YOURE", "YOU", "ME"
		};

		private static string[] QUESTIONS = {
			"DON'T YOU BELIEVE THAT I CAN.", "PERHAPS YOU WOULD LIKE TO BE ABLE TO.",
			"YOU WANT ME TO BE ABLE TO*", "PERHAPS YOU DON'T WANT TO*",
			"DO YOU WANT TO BE ABLE TO*", "WHAT MAKES YOU THINK I AM*",
			"DOES IT PLEASE YOU TO BELIEVE I AM*", "PERHAPS YOU WOULD LIKE TO BE*",
			"DO YOU SOMETIMES WISH YOU WERE*", "DON'T YOU REALLY*", "WHY DON'T YOU*",
			"DO YOU WISH TO BE ABLE TO*", "DOES THAT TROUBLE YOU?",
			"TELL ME MORE ABOUT SUCH FEELINGS*", "DO YOU OFTEN FEEL*",
			"DO YOU ENJOY FEELING*", "DO YOU REALLY BELIEVE I DON'T*",
			"PERHAPS IN TIME I WILL*", "DO YOU WANT ME TO*",
			"DO YOU THINK YOU SHOULD BE ABLE TO*", "WHY CAN'T YOU*",
			"WHAT MAKES YOU WONDER WHETHER OR NOT I AM*",
			"WOULD YOU PREFER IF I WERE NOT*", "PERHAPS IN YOUR FANTASIES I AM*",
			"HOW DO YOU KNOW YOU CAN'T*", "HAVE YOU TRIED?", "PERHAPS YOU CAN NOW*",
			"DID YOU COME TO ME BECAUSE YOU ARE*", "HOW LONG HAVE YOU BEEN*",
			"DO YOU BELIEVE IT IS NORMAL TO BE*", "DO YOU ENJOY BEING*",
			"I AM MORE INTERESTED IN TALKING ABOUT YOU.", "OH, I*",
			"ARE YOU REALLY SO INTERESTED IN SOMEONE LIKE ME?",
			"WHAT WOULD IT MEAN TO YOU IF YOU GOT*", "WHY DO YOU WANT*",
			"SUPPOSE YOU SOON GOT*", "WHAT IF YOU NEVER GOT*", "I SOMETIMES ALSO WANT*",
			"WHY DO YOU ASK?", "DOES THAT QUESTION INTEREST YOU?",
			"WHAT ANSWER WOULD PLEASE YOU THE MOST?", "WHAT DO YOU THINK?",
			"ARE SUCH QUESTIONS ON YOUR MIND OFTEN?",
			"WHAT IS IT THAT YOU REALLY WANT TO KNOW?", "HAVE YOU ASKED ANYONE ELSE?",
			"HAVE YOU ASKED SUCH QUESTIONS BEFORE?",
			"WHAT ELSE COMES TO MIND WHEN YOU ASK THAT?", "WE CAN KEEP THIS ANONYMOUS.",
			"NO NEED TO SPECIFY ANY NAMES-- PLEASE GO ON.", "IS THAT THE REAL REASON?",
			"DON'T ANY OTHER REASONS COME TO MIND?",
			"DOES THAT REASON EXPLAIN ANYTHING ELSE?", "WHAT OTHER REASONS MIGHT THERE BE?",
			"PLEASE DON'T APOLOGIZE.", "APOLOGIES ARE NOT NECESSARY.",
			"WHAT FEELINGS DO YOU HAVE WHEN YOU APOLOGIZE?", "NO NEED TO BE DEFENSIVE!",
			"WHAT DOES THAT DREAM SUGGEST TO YOU?", "DO YOU DREAM OFTEN?",
			"WHAT PERSONS APPEAR IN YOUR DREAMS?", "DO YOU HAVE PLEASANT DREAMS?",
			"HOW DO YOU DO ... PLEASE STATE YOUR PROBLEM.", "YOU DON'T SEEM QUITE CERTAIN.",
			"WHY THE UNCERTAIN TONE?", "LET'S TRY TO KEEP THIS POSITIVE.", "YOU AREN'T SURE?",
			"DON'T YOU KNOW?", "IS THAT A DEFINITE NO OR MIGHT YOU CHANGE YOUR MIND?",
			"I AM SENSING SOME NEGATIVITY.", "WHY NOT?", "ARE YOU SURE?", "WHY NO?",
			"WHY ARE YOU CONCERNED ABOUT MY*", "WHAT ABOUT YOUR OWN*",
			"CAN'T YOU THINK OF A SPECIFIC EXAMPLE?", "WHEN?", "WHAT ARE YOU THINKING OF?",
			"REALLY. ALWAYS?", "DO YOU REALLY THINK SO?", "BUT YOU ARE NOT SURE YOU.",
			"BELIEVE IN YOURSELF.", "IN WHAT WAY?", "WHAT RESEMBLANCE DO YOU SEE?",
			"WHAT DOES THE SIMILARITY SUGGEST TO YOU?",
			"WHAT OTHER CONNECTIONS DO YOU SEE?", "COULD THERE REALLY BE SOME CONNECTION?",
			"HOW?", "YOU SEEM QUITE POSITIVE.", "ARE YOU SURE?", "I SEE.", "I UNDERSTAND.",
			"TELL ME ABOUT YOUR FRIENDS.", "ARE YOU WORRIED ABOUT YOUR FRIENDS?",
			"DO YOUR FRIENDS EVER GIVE YOU A HARD TIME?", "WHAT DO YOU LIKE ABOUT YOUR FRIENDS?",
			"DO YOU LOVE YOUR FRIENDS?", "PERHAPS YOUR LOVE FOR FRIENDS WORRIES YOU.",
			"DO COMPUTERS EXCITE YOU?", "ARE YOU TALKING ABOUT ME IN PARTICULAR?",
			"HOW DO YOU LIKE YOUR WATCH?", "WHY DO YOU MENTION COMPUTERS?",
			"DO YOU FIND MACHINES AS FASCINATING AS I DO?",
			"DON'T YOU THINK COMPUTERS CAN HELP PEOPLE?",
			"WHAT ABOUT MACHINES EXCITES YOU THE MOST?",
			"HEY THERE, HOW CAN I HELP YOU?",
			"WHAT DOES THAT SUGGEST TO YOU?", "I SEE.",
			"I'M NOT SURE I UNDERSTAND YOU FULLY.", "COME COME ELUCIDATE YOUR THOUGHTS.",
			"CAN YOU ELABORATE ON THAT?", "THAT IS QUITE INTERESTING."
		};


		private static char[] CONVERSATION_TO_RESPONSES_MAP = {
			(char)1, (char)3, (char)4, (char)2, (char)6, (char)4, (char)6, (char)4, (char)10, (char)4, (char)14, (char)3, (char)17, (char)3, (char)20, (char)2, (char)22, (char)3, (char)25, (char)3,
			(char)28, (char)4, (char)28, (char)4, (char)32, (char)3, (char)35, (char)5, (char)40, (char)9, (char)40, (char)9, (char)40, (char)9, (char)40, (char)9, (char)40, (char)9, (char)40, (char)9,
			(char)49, (char)2, (char)51, (char)4, (char)55, (char)4, (char)59, (char)4, (char)63, (char)1, (char)63, (char)1, (char)64, (char)5, (char)69, (char)5, (char)74, (char)2, (char)76, (char)4,
			(char)80, (char)3, (char)83, (char)7, (char)90, (char)3, (char)93, (char)6, (char)99, (char)7, (char)106, (char)6
		};

		private int[] responseStarts = new int[36];
		private int[] responseCurrentIndices = new int[36];
		private int[] responseEnds = new int[36];
		private string previousInput = null;

		public ElizaResponder ()
		{
			for (int i = 0; i < CONVERSATION_TO_RESPONSES_MAP.Length / 2; i++) {
				responseStarts [i] = CONVERSATION_TO_RESPONSES_MAP [2 * i];
				responseCurrentIndices [i] = CONVERSATION_TO_RESPONSES_MAP [2 * i];
				responseEnds [i] = responseStarts [i] + CONVERSATION_TO_RESPONSES_MAP [2 * i + 1];
			}
		}

		public string ElzTalk (string input)
		{
			if (null == input) {
				input = "";
			}
			var result = "";

			input = " " + input.ToUpper ().Replace ("\'", "") + " ";

			if (previousInput != null && input.Equals (previousInput)) {
				return "DIDN'T YOU JUST SAY THAT?\n";
			}
			previousInput = input;

			int keywordIndex = 0;
			for (; keywordIndex < CONVERSATION_KEYWORDS.Length; ++keywordIndex) {
				int index = input.IndexOf (CONVERSATION_KEYWORDS [keywordIndex]);
				if (index != -1) {
					break;
				}
			}

			var afterKeyword = " ";
			if (keywordIndex == CONVERSATION_KEYWORDS.Length) {
				keywordIndex = 35;
			} else {
				int index = input.IndexOf (CONVERSATION_KEYWORDS [keywordIndex]);
				afterKeyword = input.Substring (index + CONVERSATION_KEYWORDS [keywordIndex].Length);
				string[] parts = afterKeyword.Split ("\\s+".ToCharArray ());
				for (int i = 0; i < WORDS_TO_REPLACE.Length / 2; i++) {
					string first = WORDS_TO_REPLACE [i * 2];
					string second = WORDS_TO_REPLACE [i * 2 + 1];
					for (int j = 0; j < parts.Length; ++j) {
						if (parts [j].Equals (first)) {
							parts [j] = second;
						} else if (parts [j].Equals (second)) {
							parts [j] = first;
						}
					}
				}
				var builder = new StringBuilder ();
				foreach (var current in parts)
					builder.Append (current);
				afterKeyword = " " + builder.ToString ();
			}

			var question = QUESTIONS [responseCurrentIndices [keywordIndex] - 1];
			responseCurrentIndices [keywordIndex] = responseCurrentIndices [keywordIndex] + 1;
			if (responseCurrentIndices [keywordIndex] > responseEnds [keywordIndex]) {
				responseCurrentIndices [keywordIndex] = responseStarts [keywordIndex];
			}
			result += question;
			if (result.EndsWith ("*")) {
				result = result.Substring (0, result.Length - 1);
				result += " " + afterKeyword;
			}
			return result;
		}
	}
}

