using System;
using System.Collections.Generic;
using Android.OS;
using Java.Lang;
using Java.Util;
using Android.App.Assist;
using Android.Service.Autofill;
using Android.Support.Annotation;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Object = Java.Lang.Object;
using String = System.String;

namespace AutofillService
{
	public class Util
	{
		public static String EXTRA_DATASET_NAME = "dataset_name";
		public static String EXTRA_FOR_RESPONSE = "for_response";
		public static Func<AssistStructure.ViewNode, object, bool> AUTOFILL_ID_FILTER = (node, id) => id.Equals(node. AutofillId);
		private static String TAG = "AutofillSample";
		public static LogLevel sLoggingLevel = LogLevel.Off;

		private static void BundleToString(StringBuilder builder, Bundle data)
		{
			var keySet = data.KeySet();
			builder.Append("[Bundle with ").Append(Integer.ToString(keySet.Count)).Append(" keys:");
			foreach (string key in keySet)
			{
				builder.Append(' ').Append(key).Append('=');
				Object value = data.Get(key);
				if ((value is Bundle)) {
					BundleToString(builder, (Bundle)value);
				} else {
					builder.Append(value is Object[] ? Arrays.ToString((Object[])value) : value);
				}
			}
			builder.Append(']');
		}

		public static String BundleToString(Bundle data)
		{
			if (data == null)
			{
				return "N/A";
			}
			StringBuilder builder = new StringBuilder();
			BundleToString(builder, data);
			return builder.ToString();
		}

		public static String GetTypeAsString(int type)
		{
			switch (type)
			{
				case (int)AutofillType.Text:
					return "TYPE_TEXT";
				case (int)AutofillType.List:
					return "TYPE_LIST";
				case (int)AutofillType.None:
					return "TYPE_NONE";
				case (int)AutofillType.Toggle:
					return "TYPE_TOGGLE";
				case (int)AutofillType.Date:
					return "TYPE_DATE";
			}
			return "UNKNOWN_TYPE";
		}

		private static String getAutofillValueAndTypeAsString(AutofillValue value)
		{
			if (value == null) return "null";

			var builder = new StringBuilder(value.ToString()).Append('(');
			if (value.IsText)
			{
				builder.Append("isText");
			}
			else if (value.IsDate)
			{
				builder.Append("isDate");
			}
			else if (value.IsToggle)
			{
				builder.Append("isToggle");
			}
			else if (value.IsList)
			{
				builder.Append("isList");
			}
			return builder.Append(')').ToString();
		}


		public static void DumpStructure(AssistStructure structure)
		{
			if (LogVerboseEnabled())
			{
				int nodeCount = structure.WindowNodeCount;
				Logv("dumpStructure(): component=%s numberNodes=%d",
					structure.ActivityComponent, nodeCount);
				for (int i = 0; i < nodeCount; i++)
				{
					Logv("node #%d", i);
					var node = structure.GetWindowNodeAt(i);
					DumpNode(new StringBuilder(), "  ", node.RootViewNode, 0);
				}
			}
		}

		private static void DumpNode(StringBuilder builder, String prefix, AssistStructure.ViewNode node, int childNumber)
		{
			builder.Append(prefix)
					.Append("child #").Append(Integer.ToString(childNumber)).Append("\n");

			builder.Append(prefix)
					.Append("autoFillId: ").Append(node.AutofillId.ToString())
					.Append("\tidEntry: ").Append(node.IdEntry)
					.Append("\tid: ").Append(node.Id.ToString())
					.Append("\tclassName: ").Append(node.ClassName)
					.Append('\n');

			builder.Append(prefix)
					.Append("focused: ").Append(node.IsFocused.ToString())
					.Append("\tvisibility").Append(node.Visibility.ToString())
					.Append("\tchecked: ").Append(node.IsChecked.ToString())
					.Append("\twebDomain: ").Append(node.WebDomain)
					.Append("\thint: ").Append(node.Hint)
					.Append('\n');

			ViewStructure.HtmlInfo htmlInfo = node.HtmlInfo;

			if (htmlInfo != null)
			{
				builder.Append(prefix)
						.Append("HTML TAG: ").Append(htmlInfo.Tag)
						.Append(" attrs: ").Append(htmlInfo.Attributes.ToString())
						.Append('\n');
			}

			var afHints = string.Join(string.Empty, node.GetAutofillHints()).ToCharArray();
			var options = string.Join(string.Empty, node.GetAutofillOptions()).ToCharArray();
			builder.Append(prefix).Append("afType: ").Append(GetTypeAsString((int)node.AutofillType))
					.Append("\tafValue:")
					.Append(getAutofillValueAndTypeAsString(node.AutofillValue))
					.Append("\tafOptions:").Append(options == null ? "N/A" : Arrays.ToString(options))
					.Append("\tafHints: ").Append(afHints == null ? "N/A" : Arrays.ToString(afHints))
					.Append("\tinputType:").Append(node.InputType.ToString())
					.Append('\n');

			int numberChildren = node.ChildCount;
			builder.Append(prefix).Append("# children: ").Append(numberChildren.ToString())
					.Append("\ttext: ").Append(node.Text)
					.Append('\n');

			String prefix2 = prefix + "  ";
			for (int i = 0; i < numberChildren; i++)
			{
				DumpNode(builder, prefix2, node.GetChildAt(i), i);
			}
			Logv(builder.ToString());
		}

		public static String getSaveTypeAsString(int type)
		{
			var types = new List<string>();
			if ((type & (int)SaveDataType.Address) != 0)
			{
				types.Add("ADDRESS");
			}
			if ((type & (int)SaveDataType.CreditCard) != 0)
			{
				types.Add("CREDIT_CARD");
			}
			if ((type & (int)SaveDataType.EmailAddress) != 0)
			{
				types.Add("EMAIL_ADDRESS");
			}
			if ((type & (int)SaveDataType.Username) != 0)
			{
				types.Add("USERNAME");
			}
			if ((type & (int)SaveDataType.Password) != 0)
			{
				types.Add("PASSWORD");
			}
			if (types.Count == 0)
			{
				return "UNKNOWN(" + type + ")";
			}
			return string.Join("|", types.ToArray());
		}

		/**
		 * Gets a node if it matches the filter criteria for the given id.
		 */
		public static AssistStructure.ViewNode FindNodeByFilter([NonNull] List<FillContext> contexts, [NonNull] object id, [NonNull] NodeFilter filter)
		{
			foreach (FillContext context in contexts)
			{
				AssistStructure.ViewNode node = FindNodeByFilter(context.Structure, id, filter);
				if (node != null)
				{
					return node;
				}
			}
			return null;
		}

		/**
		 * Gets a node if it matches the filter criteria for the given id.
		 */
		public static AssistStructure.ViewNode FindNodeByFilter([NonNull] AssistStructure structure, [NonNull] object id, [NonNull] NodeFilter filter)
		{
			Logv("Parsing request for activity %s", structure.ActivityComponent);
			int nodes = structure.WindowNodeCount;
			for (int i = 0; i < nodes; i++)
			{
				AssistStructure.WindowNode windowNode = structure.GetWindowNodeAt(i);
				AssistStructure.ViewNode rootNode = windowNode.RootViewNode;
				AssistStructure.ViewNode node = FindNodeByFilter(rootNode, id, filter);
				if (node != null)
				{
					return node;
				}
			}
			return null;
		}


		/**
		 * Gets a node if it matches the filter criteria for the given id.
		 */
		public static AssistStructure.ViewNode FindNodeByFilter([NonNull] AssistStructure.ViewNode node, [NonNull] object id, [NonNull] NodeFilter filter)
		{
			if (filter.matches(node, id))
			{
				return node;
			}
			int childrenSize = node.ChildCount;
			if (childrenSize > 0)
			{
				for (int i = 0; i < childrenSize; i++)
				{
					AssistStructure.ViewNode found = FindNodeByFilter(node.GetChildAt(i), id, filter);
					if (found != null)
					{
						return found;
					}
				}
			}
			return null;
		}

		public static void Logd(String message, params object[] objects)
		{
			if (LogDebugEnabled())
			{
				Log.Debug(TAG, string.Format(message, objects));
			}
		}

		public static void Logv(String message, params object[] objects)
		{
			if (LogVerboseEnabled())
			{
				Log.Verbose(TAG, String.Format(message, objects));
			}
		}

		public static bool LogDebugEnabled()
		{
			return sLoggingLevel >= LogLevel.Debug;
		}

		public static bool LogVerboseEnabled()
		{
			return sLoggingLevel >= LogLevel.Verbose;
		}

		public static void Logw(String message, params object[] objects)
		{
			Log.Warn(TAG, String.Format(message, objects));
		}

		public static void Logw(Throwable throwable, String message, params object[] objects)
		{
			Log.Warn(TAG, String.Format(message, objects), throwable);
		}

		public static void Loge(String message, params Object[] objects)
		{
			Log.Error(TAG, String.Format(message, objects));
		}

		public static void Loge(Throwable throwable, String message, params object[] objects)
		{
			Log.Error(TAG, String.Format(message, objects), throwable);
		}

		public static void SetLoggingLevel(LogLevel level)
		{
			sLoggingLevel = level;
		}

		/**
		 * Helper method for getting the index of a CharSequence object in an array.
		 */
		public static int IndexOf([NonNull] string[] array, string charSequence)
		{
			int index = -1;
			if (charSequence == null)
			{
				return index;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (charSequence.Equals(array[i]))
				{
					index = i;
					break;
				}
			}
			return index;
		}

		public enum LogLevel { Off, Debug, Verbose }

		public enum DalCheckRequirement { Disabled, LoginOnly, AllUrls }

		/**
		 * Helper interface used to filter Assist nodes.
		 */
		public interface NodeFilter
		{
			/**
			 * Returns whether the node passes the filter for such given id.
			 */
			bool matches(AssistStructure.ViewNode node, object id);
		}

	}
}
