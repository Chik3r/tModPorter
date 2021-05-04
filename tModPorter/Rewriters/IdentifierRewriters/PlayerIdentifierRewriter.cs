using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class PlayerIdentifierRewriter : SimpleIdentifierRewriter
	{
		public PlayerIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override string OldIdentifier => "player";
		public override string NewIdentifier => "Player";
	}
}
