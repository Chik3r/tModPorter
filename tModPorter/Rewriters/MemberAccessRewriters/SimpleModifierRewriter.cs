using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MemberAccessRewriters {
	public abstract class SimpleModifierRewriter : BaseRewriter {
		protected SimpleModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected abstract string NewModifier { get; }
		protected abstract string OldModifier { get; }
		protected abstract ModifierType ModifierType { get; }

		public sealed override RewriterType RewriterType => RewriterType.MemberAccess;

		public sealed override void VisitNode(SyntaxNode node) {
			if (node is not MemberAccessExpressionSyntax nodeSyntax)
				return;

			if (nodeSyntax.Name.ToString() == OldModifier && !HasSymbol(nodeSyntax, out _))
				AddNodeToRewrite(nodeSyntax.Name);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			if (node is not IdentifierNameSyntax nodeSyntax) return node;

			SyntaxNode newNode;
			if (ModifierType == ModifierType.Damage)
				newNode = IdentifierName($"GetDamage({NewModifier})");
			else
				newNode = IdentifierName($"GetCritChance({NewModifier})");

			newNode = newNode.WithTriviaFrom(nodeSyntax);

			return newNode;
		}
	}

	public enum ModifierType {
		Damage,
		CritChance,
	}
}