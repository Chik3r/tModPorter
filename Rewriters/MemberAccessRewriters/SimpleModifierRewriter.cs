using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	abstract class SimpleModifierRewriter : BaseRewriter
	{
		protected abstract string NewModifier { get; }
		protected abstract string OldModifier { get; }
		protected abstract ModifierType ModifierType { get; }

		protected SimpleModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public sealed override RewriterType RewriterType => RewriterType.MemberAccess;

		public sealed override void VisitNode(SyntaxNode node)
		{
			if (node is not MemberAccessExpressionSyntax nodeSyntax)
				return;

			if (nodeSyntax.Name.ToString() == OldModifier && !HasSymbol(nodeSyntax, out _))
				AddNodeToRewrite(nodeSyntax);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			var nodeSyntax = (MemberAccessExpressionSyntax) node;
			if (ModifierType == ModifierType.Damage)
				nodeSyntax = nodeSyntax.WithName(IdentifierName($"GetDamage({NewModifier})"));
			else
				nodeSyntax = nodeSyntax.WithName(IdentifierName($"GetCritChance({NewModifier})"));

			nodeSyntax = nodeSyntax.WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(node.GetTrailingTrivia());

			return nodeSyntax;
		}
	}

	enum ModifierType
	{
		Damage,
		CritChance,
	}
}
