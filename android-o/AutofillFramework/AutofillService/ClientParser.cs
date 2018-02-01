using System.Collections.Generic;
using System.Collections.Immutable;
using Android.App.Assist;
using Java.Lang;

namespace AutofillService
{
	public class ClientParser
	{
		private ImmutableList<AssistStructure> mStructures;

		public ClientParser(ImmutableList<AssistStructure> structures)
		{
			mStructures = structures ?? throw new NullPointerException();
		}

		public ClientParser(AssistStructure structure) : this(new List<AssistStructure>() { structure }.ToImmutableList())
		{
		}

		/**
		 * Traverses through the {@link AssistStructure} and does something at each {@link ViewNode}.
		 *
		 * @param processor contains action to be performed on each {@link ViewNode}.
		 */
		public void Parse(INodeProcessor processor)
		{
			foreach (AssistStructure structure in mStructures)
			{
				int nodes = structure.WindowNodeCount;
				for (int i = 0; i < nodes; i++)
				{
					AssistStructure.ViewNode viewNode = structure.GetWindowNodeAt(i).RootViewNode;
					TraverseRoot(viewNode, processor);
				}
			}
		}

		private void TraverseRoot(AssistStructure.ViewNode viewNode, INodeProcessor processor)
		{
			processor.ProcessNode(viewNode);
			int childrenSize = viewNode.ChildCount;
			if (childrenSize > 0)
			{
				for (int i = 0; i < childrenSize; i++)
				{
					TraverseRoot(viewNode.GetChildAt(i), processor);
				}
			}
		}

		public interface INodeProcessor
		{
			void ProcessNode(AssistStructure.ViewNode node);
		}
	}
}