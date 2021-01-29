using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters
{
	class PlaySoundRewriter : BaseRewriter
	{
		public PlaySoundRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override RewriterType RewriterType => RewriterType.Invocation;

		public override bool VisitNode(SyntaxNode node, out SyntaxNode finalNode)
		{
			finalNode = node;

			if (node is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax memberAccess)
			{
				if (memberAccess.ToString() == "Main.PlaySound")
				{
					finalNode = invocation.WithExpression(IdentifierName("SoundEngine.PlaySound")
						.WithLeadingTrivia(invocation.GetLeadingTrivia()).WithTrailingTrivia(invocation.GetTrailingTrivia()));
					AddUsing("Terraria.Audio");
				}
			}

			return true;
		}
	}
}
