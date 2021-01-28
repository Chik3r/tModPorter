using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	abstract class SimpleModifierRewriter : BaseRewriter
	{
		public abstract string NewModifier { get; }
		public abstract string OldModifier { get; }
		public abstract ModifierType ModifierType { get; }
		
		public SimpleModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public sealed override RewriterType RewriterType => RewriterType.MemberAccess;

		public sealed override SyntaxNode VisitNode(SyntaxNode node)
		{
			var nodeSyntax = (MemberAccessExpressionSyntax) node;
			if (nodeSyntax.Name.ToString() == OldModifier && !HasSymbol(nodeSyntax, out _))
			{
				if (ModifierType == ModifierType.Damage)
					nodeSyntax = nodeSyntax.WithName(IdentifierName($"GetDamage({NewModifier})"));
				else
					nodeSyntax = nodeSyntax.WithName(IdentifierName($"GetCritChance({NewModifier})"));

				nodeSyntax = nodeSyntax.WithLeadingTrivia(node.GetLeadingTrivia()).WithTrailingTrivia(node.GetTrailingTrivia());
			}

			return nodeSyntax;
		}
	}

	enum ModifierType
	{
		Damage,
		CritChance,
	}
}
