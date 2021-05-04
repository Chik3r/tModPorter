using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MethodDeclarationRewriters
{
	class AddRecipesRewriter : BaseRewriter
	{
		public AddRecipesRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override RewriterType RewriterType => RewriterType.Method;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not MethodDeclarationSyntax nodeMethod)
				return;

			// Make sure the body isn't null, that the method name is "AddRecipes", that it has statements, and that it hasn't already been ported
			if (nodeMethod.Body != null && nodeMethod.Identifier.Text == "AddRecipes" 
			                            && nodeMethod.Body.Statements.Count != 0 && !nodeMethod.Body.ToString().Contains("CreateRecipe"))
				AddNodeToRewrite(node);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			var nodeMethod = (MethodDeclarationSyntax) node;
			var leading = nodeMethod.Body.Statements.First().GetLeadingTrivia();
			var newStatements = new SyntaxList<StatementSyntax>();

			string expression = "";
			int resultAmount = 1;
			string result = null;

			foreach (StatementSyntax statementSyntax in nodeMethod.Body.Statements)
			{
				if (statementSyntax is not ExpressionStatementSyntax
					{Expression: InvocationExpressionSyntax invocationExpressionSyntax})
					continue;

				if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessSyntax)
					continue;

				// Parse the existing recipe
				switch (memberAccessSyntax.Name.ToString())
				{
					case "AddIngredient":
					case "AddTile":
					case "AddRecipeGroup":
						var splitExpression = invocationExpressionSyntax.ToString().Split('.', 2);
						expression += "." + splitExpression[1];
						break;
					case "SetResult":
						var arguments = invocationExpressionSyntax.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();

						if (arguments[0] != "this")
							result = arguments[0];
						if (arguments.Length == 2)
							resultAmount = int.Parse(arguments[1]);
						break;
					case "AddRecipe":
						var parsedExpression = $"CreateRecipe({resultAmount})" + expression;

						if (string.IsNullOrEmpty(result))
							parsedExpression += ".Register()";
						else
							parsedExpression += $".ReplaceResult({result})";

						newStatements = newStatements.Add(ExpressionStatement(ParseExpression(parsedExpression))
							.WithLeadingTrivia(leading).WithTrailingTrivia(ElasticCarriageReturnLineFeed));

						expression = "";
						resultAmount = 1;
						result = "";
						break;
				}
			}

			var modifierBody = nodeMethod.Body.WithStatements(newStatements);
			return nodeMethod.WithBody(modifierBody);
		}
	}
}
