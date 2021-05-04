using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ProjectileIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ProjectileIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override string OldIdentifier => "projectile";
		public override string NewIdentifier => "Projectile";
	}
}
