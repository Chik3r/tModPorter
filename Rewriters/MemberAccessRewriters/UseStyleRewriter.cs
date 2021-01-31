using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class UseStyleRewriter : BaseRewriter
	{
		private readonly Dictionary<string, string> _useStyleToPort = new()
		{
			{"HoldingUp", "HoldUp"},
			{"HoldingOut", "Shoot"},
			{"SwingThrow", "Swing"},
			{"EatingUsing", "EatFood"},
			{"Stabbing", "Thrust"},
		};

		public UseStyleRewriter(SemanticModel model, List<string> usingList, 
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override RewriterType RewriterType => RewriterType.MemberAccess;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not MemberAccessExpressionSyntax memberAccess)
				return;

			if (HasSymbol(memberAccess.Name, out _))
				return;

			if (memberAccess.Expression.ToString() != "ItemUseStyleID")
				return;

			if (_useStyleToPort.ContainsKey(memberAccess.Name.ToString()))
				AddNodeToRewrite(memberAccess.Name);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			if (node is not SimpleNameSyntax nameSyntax)
				return node;

			var newUseStyle = _useStyleToPort.First(u => u.Key == nameSyntax.ToString());
			return IdentifierName(newUseStyle.Value).WithExtraTrivia(nameSyntax);
		}
	}
}
