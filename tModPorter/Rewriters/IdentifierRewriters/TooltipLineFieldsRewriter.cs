using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.IdentifierRewriters; 

public class TooltipLineFieldsRewriter : BaseRewriter {
	private static readonly Dictionary<string, string> IdentifierMap = new() {
		{"text", "Text"},
		{"isModifier", "IsModifier"},
		{"isModifierBad", "IsModifierBad"},
		{"overrideColor", "OverrideColor"},
	};

	public TooltipLineFieldsRewriter(SemanticModel model, List<string> usingList,
		HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
		HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
		: base(model, usingList, nodesToRewrite, tokensToRewrite) { }

	public override RewriterType RewriterType => RewriterType.Identifier;

	public override void VisitNode(SyntaxNode node) {
		if (node.Parent is not MemberAccessExpressionSyntax memberAccessSyntax)
			return;

		if (!IdentifierMap.ContainsKey(memberAccessSyntax.Name.ToString()))
			return;
		
		// Check if a is of type TooltipLine (a.b)
		string typeName = GetTypeName(memberAccessSyntax.Expression);
		if (typeName is null || typeName != "TooltipLine") return;
		
		AddNodeToRewrite(memberAccessSyntax.Name);
	}

	public override SyntaxNode RewriteNode(SyntaxNode node) {
		string newName = IdentifierMap[node.ToString()];
		SyntaxNode newNode = IdentifierName(newName).WithTriviaFrom(node);

		return newNode;
	}
}