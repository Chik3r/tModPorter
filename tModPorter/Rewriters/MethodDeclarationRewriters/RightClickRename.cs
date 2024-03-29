﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MethodDeclarationRewriters;

public class RightClickRename : BaseRewriter {
	public RightClickRename(SemanticModel model, List<string> usingList,
		HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
		HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
		: base(model, usingList, nodesToRewrite, tokensToRewrite) { }

	public override RewriterType RewriterType => RewriterType.Method;

	public override void VisitNode(SyntaxNode node) {
		if (node is not MethodDeclarationSyntax declaration) return;

		if (declaration.Identifier.ToString() != "NewRightClick") return;

		// Get class declaration and match base class to ModItem
		if (!TryGetAncestorNode(node, out ClassDeclarationSyntax classDeclaration) || classDeclaration.BaseList == null ||
		    classDeclaration.BaseList.Types.Count == 0 ||
		    classDeclaration.BaseList.Types.All(x => x.Type.ToString() != "ModTile")) return;

		AddTokenToRewrite(declaration.Identifier);
	}

	public override SyntaxToken RewriteToken(SyntaxToken token) {
		if (token.Text != "NewRightClick")
			return token;

		return Identifier("RightClick").WithTriviaFrom(token);
	}
}