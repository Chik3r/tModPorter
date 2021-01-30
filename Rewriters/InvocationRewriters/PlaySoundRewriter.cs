using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters
{
	class PlaySoundRewriter : BaseRewriter
	{
		public PlaySoundRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override RewriterType RewriterType => RewriterType.Invocation;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess})
				if (memberAccess.ToString() == "Main.PlaySound")
					AddNodeToRewrite(memberAccess);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			AddUsing("Terraria.Audio");
			return IdentifierName("SoundEngine.PlaySound").WithExtraTrivia(node);
		}
	}
}
