using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ItemIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ItemIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string OldIdentifier => "item";
		public override string NewIdentifier => "Item";
	}
}
