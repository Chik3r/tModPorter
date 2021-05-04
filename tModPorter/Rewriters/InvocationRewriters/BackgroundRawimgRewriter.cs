using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters
{
	class BackgroundRawimgRewriter : BaseRewriter
	{
		public BackgroundRawimgRewriter(SemanticModel model, List<string> usingList, 
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override RewriterType RewriterType => RewriterType.Invocation;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not InvocationExpressionSyntax invocation)
				return;

			if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
				return;

			// Add a ArgumentListSyntax to rewrite, if the method is "GetBackgroundSlot", it will always have one argument
			if (memberAccess.Name.ToString() == "GetBackgroundSlot" && !invocation.ArgumentList.Arguments[0].ToString().Contains(".rawimg"))
				AddNodeToRewrite(invocation.ArgumentList);

			base.VisitNode(node);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			if (node is not ArgumentListSyntax argumentList)
				return node;

			var newArguments = new SeparatedSyntaxList<ArgumentSyntax>();
			foreach (ArgumentSyntax oldArgument in argumentList.Arguments)
			{
				// Append ' + ".rawimg"' at the end of the parameter
				newArguments = newArguments.Add(oldArgument.WithExpression(IdentifierName(oldArgument.Expression + " + \".rawimg\"")));
			}

			return argumentList.WithArguments(newArguments);
		}
	}
}
