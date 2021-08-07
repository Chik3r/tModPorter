using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	public class ProjectileIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ProjectileIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		public override string OldIdentifier => "projectile";
		public override string NewIdentifier => "Projectile";
	}
}