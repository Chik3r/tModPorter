using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ItemIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ItemIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override string OldIdentifier => "item";
		public override string NewIdentifier => "Item";
	}
}
