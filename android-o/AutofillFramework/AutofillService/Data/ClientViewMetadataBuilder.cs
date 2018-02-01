using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Android.App.Assist;
using Android.Util;
using Android.Views.Autofill;
using AutofillService.Data.Source;
using AutofillService.Model;

namespace AutofillService.Data
{
	public class ClientViewMetadataBuilder
	{
		private ClientParser mClientParser;
		private Dictionary<string, FieldTypeWithHeuristics> mFieldTypesByAutofillHint;

		public List<string> allHints;
		public MutableInt saveType;
		public List<AutofillId> autofillIds;
		public StringBuilder webDomainBuilder;
		public List<AutofillId> focusedAutofillIds;

		public ClientViewMetadataBuilder(ClientParser parser,
			Dictionary<string, FieldTypeWithHeuristics> fieldTypesByAutofillHint)
		{
			mClientParser = parser;
			mFieldTypesByAutofillHint = fieldTypesByAutofillHint;
		}

		public ClientViewMetadata BuildClientViewMetadata()
		{
			List<string> allHints = new List<string>();
			MutableInt saveType = new MutableInt(0);
			List<AutofillId> autofillIds = new List<AutofillId>();
			StringBuilder webDomainBuilder = new StringBuilder();
			List<AutofillId> focusedAutofillIds = new List<AutofillId>();
			mClientParser.Parse(new ParseNodeProcessor() { that = this });
			mClientParser.Parse(new ParseWebDomainNodeProcessor() { that = this });
			String webDomain = webDomainBuilder.ToString();
			AutofillId[] autofillIdsArray = autofillIds.ToArray();
			AutofillId[] focusedIds = focusedAutofillIds.ToArray();
			return new ClientViewMetadata(allHints, saveType.Value, autofillIdsArray, focusedIds, webDomain);
		}

		private void ParseWebDomain(AssistStructure.ViewNode viewNode, StringBuilder validWebDomain)
		{
			String webDomain = viewNode.WebDomain;
			if (webDomain != null)
			{
				Util.Logd("child web domain: %s", webDomain);
				if (validWebDomain.Length > 0)
				{
					if (!webDomain.Equals(validWebDomain.ToString()))
					{
						throw new SecurityException("Found multiple web domains: valid= "
						                            + validWebDomain + ", child=" + webDomain);
					}
				}
				else
				{
					validWebDomain.Append(webDomain);
				}
			}
		}

		private void ParseNode(AssistStructure.ViewNode root, List<String> allHints,
			MutableInt autofillSaveType, List<AutofillId> autofillIds,
			List<AutofillId> focusedAutofillIds)
		{
			String[] hints = root.GetAutofillHints();
			if (hints != null)
			{
				foreach (string hint in hints)
				{
					FieldTypeWithHeuristics fieldTypeWithHints = mFieldTypesByAutofillHint[hint];
					if (fieldTypeWithHints != null && fieldTypeWithHints.fieldType != null)
					{
						allHints.Add(hint);
						autofillSaveType.Value |= fieldTypeWithHints.fieldType.GetSaveInfo();
					}
				}
			}
			if (root.IsFocused)
			{
				focusedAutofillIds.Add(root.AutofillId);
			}
			autofillIds.Add(root.AutofillId);
		}

		public class ParseNodeProcessor : Java.Lang.Object, ClientParser.INodeProcessor
		{
			public ClientViewMetadataBuilder that;
			public void ProcessNode(AssistStructure.ViewNode node)
			{
				that.ParseNode(node, that.allHints, that.saveType, that.autofillIds, that.focusedAutofillIds);
			}
		}

		public class ParseWebDomainNodeProcessor : Java.Lang.Object, ClientParser.INodeProcessor
		{
			public ClientViewMetadataBuilder that;
			public void ProcessNode(AssistStructure.ViewNode node)
			{
				that.ParseWebDomain(node, that.webDomainBuilder);
			}
		}
	}
}