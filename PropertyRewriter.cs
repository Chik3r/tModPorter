using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModPorter
{
	public class PropertyRewriter : CSharpSyntaxRewriter
	{
		public static readonly Dictionary<string, string> PropertiesToRename = new Dictionary<string, string>
		{
			{"item", "Item"},
			{"mod", "Mod"},
			{"player", "Player"},
			{"DustType", "Find<ModDust>"},
			{"ProjectileType", "Find<ModProjectile>"},
			{"disableSmartCursor", "TileID.Sets.DisableSmartCursor[Type]"}
		};

		private readonly SemanticModel _semanticModel;
		public PropertyRewriter(SemanticModel model) => _semanticModel = model;

		public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
		{
			if (PropertiesToRename.TryGetValue(node.Identifier.Text, out string newIdentifier))
			{
				var symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol == null)
				{
					//Console.WriteLine($"Renaming {node.Identifier.Text} to {newIdentifier}");

					var leadingTrivia = node.GetLeadingTrivia();
					var trailingTrivia = node.GetTrailingTrivia();
					var identifierSyntax = SyntaxFactory.Identifier(leadingTrivia, newIdentifier, trailingTrivia);

					return SyntaxFactory.IdentifierName(identifierSyntax);
				}
			}

			return base.VisitIdentifierName(node);
		}

		//public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		//{
		//	var childs = node.ChildNodes().ToList();
		//	//Console.WriteLine(string.Join("-", node.ChildNodes()));

		//	foreach (SyntaxNode child in childs)
		//	{
		//		if (child is IdentifierNameSyntax childName)
		//		{
		//			Console.WriteLine($"To modify: {childName.Identifier}");
		//			var newIdentifier = childName.Identifier.Text.ToUpper();
		//			var leadingTrivia = childName.GetLeadingTrivia();
		//			var trailingTrivia = childName.GetTrailingTrivia();
		//			childName = childName.WithIdentifier(SyntaxFactory.Identifier(leadingTrivia, newIdentifier, trailingTrivia));
		//			Console.WriteLine($"Modified to: {childName.Identifier}");
		//			//childs[i] = childName;
		//		}
		//	}

		//	return base.VisitMemberAccessExpression(node);
		//}
	}
}
