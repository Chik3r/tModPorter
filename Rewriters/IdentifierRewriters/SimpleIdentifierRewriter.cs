using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	abstract class SimpleIdentifierRewriter : BaseRewriter
	{
		public abstract string OldIdentifier { get; }
		public abstract string NewIdentifier { get; }

		protected SimpleIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public sealed override RewriterType RewriterType => RewriterType.Identifier;

		public sealed override bool VisitNode(SyntaxNode node, out SyntaxNode finalNode)
		{
			finalNode = node;
			if (node.ToString() == OldIdentifier && !HasSymbol(node, out _))
				finalNode = IdentifierName(NewIdentifier).WithTrailingTrivia(node.GetTrailingTrivia()).WithLeadingTrivia(node.GetLeadingTrivia());

			return true;
		}
	}
}
