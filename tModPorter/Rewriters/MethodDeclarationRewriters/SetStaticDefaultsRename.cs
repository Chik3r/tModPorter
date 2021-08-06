using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MethodDeclarationRewriters
{
	public class SetStaticDefaultsRename : BaseRewriter
	{
		private static readonly string[] BaseTypesToModify = {
			"GlobalTile",
			"GlobalWall",
			"InfoDisplay",
			"ModTile",
			"ModWall",
			"ModBuff",
			"ModDust",
			"ModMount",
			"ModPrefix",
		};
		
		public SetStaticDefaultsRename(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		public override RewriterType RewriterType => RewriterType.Method;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not MethodDeclarationSyntax declaration) return;

			if (declaration.Identifier.ToString() != "SetDefaults") return;

			// Get class declaration and make sure the base types matches one of the singleton base types
			if (!TryGetAncestorNode(node, out ClassDeclarationSyntax classDeclaration) || classDeclaration.BaseList == null ||
			    classDeclaration.BaseList.Types.Count == 0 ||
			    classDeclaration.BaseList.Types.All(x => !BaseTypesToModify.Contains(x.Type.ToString()))) return;

			AddTokenToRewrite(declaration.Identifier);
		}

		public override SyntaxToken RewriteToken(SyntaxToken token)
		{
			if (token.Text != "SetDefaults")
				return token;

			return Identifier("SetStaticDefaults").WithTriviaFrom(token);
		}
	}
}