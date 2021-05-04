using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class NPCIdentifierRewriter : SimpleIdentifierRewriter
	{
		public NPCIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override string OldIdentifier => "npc";
		public override string NewIdentifier => "NPC";
	}
}
