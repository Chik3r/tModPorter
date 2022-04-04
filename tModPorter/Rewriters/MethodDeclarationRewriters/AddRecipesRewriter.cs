using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.MethodDeclarationRewriters;

public class AddRecipesRewriter : BaseRewriter {
	public AddRecipesRewriter(SemanticModel model, List<string> usingList,
		HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
		HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
		: base(model, usingList, nodesToRewrite, tokensToRewrite) { }

	public override RewriterType RewriterType => RewriterType.Method;

	public override void VisitNode(SyntaxNode node) {
		if (node is not MethodDeclarationSyntax nodeMethod)
			return;

		// Make sure the body isn't null, that the method name is "AddRecipes", that it has statements, and that it hasn't already been ported
		if (nodeMethod.Body != null && nodeMethod.Identifier.Text == "AddRecipes"
		                            && nodeMethod.Body.Statements.Count != 0 && !nodeMethod.Body.ToString().Contains("CreateRecipe"))
			AddNodeToRewrite(node);
	}

	public override SyntaxNode RewriteNode(SyntaxNode node) {
		MethodDeclarationSyntax nodeMethod = (MethodDeclarationSyntax) node;
		SyntaxTriviaList leading = nodeMethod.Body.Statements.First().GetLeadingTrivia();
		SyntaxList<StatementSyntax> newStatements = new();

		string expression = "";
		int resultAmount = 1;
		string result = null;

		foreach (StatementSyntax statementSyntax in nodeMethod.Body.Statements) {
			if (statementSyntax is not ExpressionStatementSyntax {
					Expression: InvocationExpressionSyntax invocationExpressionSyntax,
				})
				continue;

			if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessSyntax)
				continue;

			// Parse the existing recipe
			switch (memberAccessSyntax.Name.ToString()) {
				case "AddIngredient":
				case "AddTile":
				case "AddRecipeGroup":
					string[] splitExpression = invocationExpressionSyntax.ToString().Split('.', 2);
					expression += "." + splitExpression[1];
					break;
				case "SetResult":
					string[] arguments = invocationExpressionSyntax.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();

					if (arguments[0] != "this")
						result = arguments[0];
					if (arguments.Length == 2)
						resultAmount = int.Parse(arguments[1]);
					break;
				case "AddRecipe":
					string parsedExpression;

					if (string.IsNullOrEmpty(result))
						parsedExpression = $"CreateRecipe({resultAmount})";
					else
						parsedExpression = $"Mod.CreateRecipe({result}, {resultAmount})";

					parsedExpression += expression + ".Register()";

					newStatements = newStatements.Add(ExpressionStatement(ParseExpression(parsedExpression))
						.WithLeadingTrivia(leading).WithTrailingTrivia(ElasticCarriageReturnLineFeed));

					expression = "";
					resultAmount = 1;
					result = "";
					break;
			}
		}

		BlockSyntax modifierBody = nodeMethod.Body.WithStatements(newStatements);
		return nodeMethod.WithBody(modifierBody);
	}
}