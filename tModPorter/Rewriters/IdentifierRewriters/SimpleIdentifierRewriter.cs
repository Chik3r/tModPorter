using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	public abstract class SimpleIdentifierRewriter : BaseRewriter
	{
		protected SimpleIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		public abstract string OldIdentifier { get; }
		public abstract string NewIdentifier { get; }
		public virtual string NeededUsing => "Terraria.ModLoader";

		public sealed override RewriterType RewriterType => RewriterType.Identifier;

		public sealed override void VisitNode(SyntaxNode node)
		{
			if (node is not IdentifierNameSyntax identifier) return;

			if (node.Parent is AssignmentExpressionSyntax or VariableDeclaratorSyntax) return;

			if (identifier.ToString() == OldIdentifier && !HasSymbol(node, out _))
				AddNodeToRewrite(identifier);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			if (NeededUsing != null) AddUsing(NeededUsing);
			return IdentifierName(NewIdentifier).WithExtraTrivia(node);
		}
	}
}