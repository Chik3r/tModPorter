using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters;

public class TileFieldsRewriter : BaseRewriter {
	private static readonly Dictionary<string, string> IdentifierMap = new() {
		{"frameX", "TileFrameX"},
		{"frameY", "TileFrameY"},
		{"type", "TileType"},
		{"wall", "WallType"},
		{"wallFrameX", "WallFrameX"},
		{"wallFrameY", "WallFrameY"},
	};

	public TileFieldsRewriter(SemanticModel model, List<string> usingList,
		HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
		HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
		: base(model, usingList, nodesToRewrite, tokensToRewrite) { }

	public override RewriterType RewriterType => RewriterType.Identifier;

	public override void VisitNode(SyntaxNode node) {
		if (node is not IdentifierNameSyntax identifier) return;

		// Only modify member accesses 'a.b'
		if (node.Parent is not MemberAccessExpressionSyntax memberAccess) return;

		SyntaxNode typeNode;
		// Check if the parent node's expression is an array access, example: 'a.b[0].c'
		if (memberAccess.Expression is ElementAccessExpressionSyntax {Expression: MemberAccessExpressionSyntax nestedMember})
			typeNode = nestedMember;
		else
			typeNode = memberAccess.Expression;

		string typeName = GetTypeName(typeNode);
		if (typeName != "Tilemap" && typeName != "Tile") return;

		if (IdentifierMap.ContainsKey(identifier.ToString()) && !HasSymbol(node, out _))
			AddNodeToRewrite(identifier);
	}

	public override SyntaxNode RewriteNode(SyntaxNode node) {
		if (IdentifierMap.TryGetValue(node.ToString(), out string value))
			return IdentifierName(value).WithExtraTrivia(node);
		else
			return node;
	}
}