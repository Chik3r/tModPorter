using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ModIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ModIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override string OldIdentifier => "mod";
		public override string NewIdentifier => "Mod";
	}
}
