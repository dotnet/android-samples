using System;
using System.Text;
using Android.App.Assist;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Autofill;
using Java.Util;

namespace AutofillService
{
	public class CommonUtil
	{
		public static string TAG = "AutofillSample";
		public static bool DEBUG = true;
		public static bool VERBOSE = false;
		public static string EXTRA_DATASET_NAME = "dataset_name";
		public static string EXTRA_FOR_RESPONSE = "for_response";

		private static void BundleToString(StringBuilder builder, Bundle data)
		{
			var keySet = data.KeySet();
			builder.Append("[Bundle with ").Append(keySet.Count).Append(" keys:");
			foreach (string key in keySet)
			{
				builder.Append(' ').Append(key).Append('=');
				var value = data.Get(key);
				if (value is Bundle)
				{
					BundleToString(builder, (Bundle) value);
				}
				else
				{
					builder.Append(value);
				}
			}

			builder.Append(']');
		}

		public static string BundleToString(Bundle data)
		{
			if (data == null)
			{
				return "N/A";
			}

			var builder = new StringBuilder();
			BundleToString(builder, data);
			return builder.ToString();
		}

		public static string GetTypeAsString(AutofillType type)
		{
			switch (type)
			{
				case AutofillType.Text:
					return "TYPE_TEXT";
				case AutofillType.List:
					return "TYPE_LIST";
				case AutofillType.None:
					return "TYPE_NONE";
				case AutofillType.Toggle:
					return "TYPE_TOGGLE";
				case AutofillType.Date:
					return "TYPE_DATE";
			}

			return "UNKNOWN_TYPE";
		}

		private static string GetAutofillValueAndTypeAsString(AutofillValue value)
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
			var nodeCount = structure.WindowNodeCount;
			Log.Verbose(TAG, "dumpStructure(): component=" + structure.ActivityComponent
			                                               + " numberNodes=" + nodeCount);
			for (var i = 0; i < nodeCount; i++)
			{
				Log.Verbose(TAG, "node #" + i);
				AssistStructure.WindowNode node = structure.GetWindowNodeAt(i);
				DumpNode("  ", node.RootViewNode);
			}
		}

		private static void DumpNode(String prefix, AssistStructure.ViewNode node)
		{
			var builder = new StringBuilder();
			builder.Append(prefix)
				.Append("autoFillId: ").Append(node.AutofillId)
				.Append("\tidEntry: ").Append(node.IdEntry)
				.Append("\tid: ").Append(node.Id)
				.Append("\tclassName: ").Append(node.ClassName)
				.Append('\n');

			builder.Append(prefix)
				.Append("focused: ").Append(node.IsFocused)
				.Append("\tvisibility").Append(node.Visibility)
				.Append("\tchecked: ").Append(node.IsChecked)
				.Append("\twebDomain: ").Append(node.WebDomain)
				.Append("\thint: ").Append(node.Hint)
				.Append('\n');

			var htmlInfo = node.HtmlInfo;

			if (htmlInfo != null)
			{
				builder.Append(prefix)
					.Append("HTML TAG: ").Append(htmlInfo.Tag)
					.Append(" attrs: ").Append(htmlInfo.Attributes)
					.Append('\n');
			}

			var afHints = string.Join(string.Empty, node.GetAutofillHints()).ToCharArray();
			var options = string.Join(string.Empty, node.GetAutofillOptions()).ToCharArray();
			builder.Append(prefix).Append("afType: ").Append(GetTypeAsString(node.AutofillType))
				.Append("\tafValue:")
				.Append(GetAutofillValueAndTypeAsString(node.AutofillValue))
				.Append("\tafOptions:").Append(options == null ? "N/A" : Arrays.ToString(options))
				.Append("\tafHints: ").Append(afHints == null ? "N/A" : Arrays.ToString(afHints))
				.Append("\tinputType:").Append(node.InputType)
				.Append('\n');

			var numberChildren = node.ChildCount;
			builder.Append(prefix).Append("# children: ").Append(numberChildren)
				.Append("\ttext: ").Append(node.Text)
				.Append('\n');

			Log.Verbose(TAG, builder.ToString());
			var prefix2 = prefix + "  ";
			for (int i = 0; i < numberChildren; i++)
			{
				Log.Verbose(TAG, prefix + "child #" + i);
				DumpNode(prefix2, node.GetChildAt(i));
			}
		}
	}
}