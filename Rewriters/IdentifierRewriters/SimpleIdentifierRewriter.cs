using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	abstract class SimpleIdentifierRewriter : BaseRewriter
	{
		public abstract string OldIdentifier { get; }
		public abstract string NewIdentifier { get; }

		protected SimpleIdentifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public sealed override RewriterType RewriterType => RewriterType.Identifier;

		public sealed override void VisitNode(SyntaxNode node)
		{
			if (node.ToString() == OldIdentifier && !HasSymbol(node, out _))
				AddNodeToRewrite(node);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			return IdentifierName(NewIdentifier).WithExtraTrivia(node);
		}
	}
}
