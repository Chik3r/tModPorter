using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters;

public class SimpleIdentifiersRewriter : BaseRewriter {
	private static readonly Dictionary<string, string> IdentifierMap = new() {
		{"mod", "Mod"},
		{"item", "Item"},
		{"npc", "NPC"},
		{"player", "Player"},
		{"projectile", "Projectile"},
	};

	public SimpleIdentifiersRewriter(SemanticModel model, List<string> usingList,
		HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
		HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
		: base(model, usingList, nodesToRewrite, tokensToRewrite) { }

	public override RewriterType RewriterType => RewriterType.Identifier;

	public override void VisitNode(SyntaxNode node) {
		if (node is not IdentifierNameSyntax identifier) return;

		// Ignore variable declarations ('var a = 5') or assignments ('a = 5')
		// This doesn't ignore something like 'mod.field = 5' since 'mod.field' is its own syntax.
		if (node.Parent is AssignmentExpressionSyntax or VariableDeclaratorSyntax) return;

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