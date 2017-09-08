using System;
using Android.App.Assist;
using Android.Util;
using AutofillFramework.multidatasetservice.model;
using static Android.App.Assist.AssistStructure;

namespace AutofillFramework
{
	/// <summary>
	///	Parser for an AssistStructure object. This is invoked when the Autofill Service receives an
	/// AssistStructure from the client Activity, representing its View hierarchy. In this sample, it
	/// parses the hierarchy and collects autofill metadata from {@link ViewNode}s along the way.
	/// </summary>
	public sealed class StructureParser
	{
		public AutofillFieldMetadataCollection AutofillFields { get; set; }
		AssistStructure Structure;
		public FilledAutofillFieldCollection ClientFormData { get; set; }

		public StructureParser(AssistStructure structure)
		{
			Structure = structure;
			AutofillFields = new AutofillFieldMetadataCollection();
		}

		public void ParseForFill()
		{
			Parse(true);
		}

		public void ParseForSave()
		{
			Parse(false);
		}

		/// <summary>
		/// Traverse AssistStructure and add ViewNode metadata to a flat list.
		/// </summary>
		/// <returns>The parse.</returns>
		/// <param name="forFill">If set to <c>true</c> for fill.</param>
		void Parse(bool forFill)
		{
			Log.Debug(CommonUtil.Tag, "Parsing structure for " + Structure.ActivityComponent);
			var nodes = Structure.WindowNodeCount;
			ClientFormData = new FilledAutofillFieldCollection();
			for (int i = 0; i < nodes; i++)
			{
				var node = Structure.GetWindowNodeAt(i);
				var view = node.RootViewNode;
				ParseLocked(forFill, view);
			}
		}

		void ParseLocked(bool forFill, ViewNode viewNode)
		{
			if (viewNode.GetAutofillHints() != null && viewNode.GetAutofillHints().Length > 0)
			{
				if (forFill)
				{
					AutofillFields.Add(new AutofillFieldMetadata(viewNode));
				}
				else
				{
					ClientFormData.Add(new FilledAutofillField(viewNode));
				}
			}
			var childrenSize = viewNode.ChildCount;
			if (childrenSize > 0)
			{
				for (int i = 0; i < childrenSize; i++)
				{
					ParseLocked(forFill, viewNode.GetChildAt(i));
				}
			}
		}

	}
}
