using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters {
	public abstract class SimpleIdentifierRewriter : BaseRewriter {
		protected SimpleIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public abstract string OldIdentifier { get; }
		public abstract string NewIdentifier { get; }

		public sealed override RewriterType RewriterType => RewriterType.Identifier;

		public sealed override void VisitNode(SyntaxNode node) {
			if (node is not IdentifierNameSyntax {Parent: MemberAccessExpressionSyntax} identifier) return;

			if (identifier.ToString() == OldIdentifier && !HasSymbol(node, out _))
				AddNodeToRewrite(identifier);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node) {
			return IdentifierName(NewIdentifier).WithExtraTrivia(node);
		}
	}
}