using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModPorter
{
	public class PropertyRewriter : CSharpSyntaxRewriter
	{
		public static readonly Dictionary<string, string> IdentifiersToRename = new Dictionary<string, string> {
			{"item", "Item"},
			{"mod", "Mod"},
			{"player", "Player"},
			{"npc", "NPC"},
			{"projectile", "Projectile"},
			{"DustType", "Find<ModDust>"},
			{"ProjectileType", "Find<ModProjectile>"},
			{"GetGoreSlot", "Find<ModGore>"},
			{"disableSmartCursor", "TileID.Sets.DisableSmartCursor[Type]"},
			{"sapling", "TileID.Sets.TreeSapling[Type]"},
			{"torch", "TileID.Sets.Torch[Type]"},
			{"ModWorld", "ModSystem"},
			{"rangedCrit", "GetCritChance(DamageClass.Ranged)"},
			{"magicCrit", "GetCritChance(DamageClass.Magic)"},
			{"thrownCrit", "GetCritChance(DamageClass.Throwing)"},
			{"meleeCrit", "GetCritChance(DamageClass.Melee)"},
			{"rangedDamage", "GetDamage(DamageClass.Ranged)"},
			{"magicDamage", "GetDamage(DamageClass.Magic)"},
			{"thrownDamage", "GetDamage(DamageClass.Throwing)"},
			{"meleeDamage", "GetDamage(DamageClass.Melee)"},
			{"minionDamage", "GetDamage(DamageClass.Summon)"}
		};

		public static readonly Dictionary<string, string> MethodsToRename = new Dictionary<string, string> {
			{"Initialize", "OnWorldLoad"},
			{"Save", "SaveWorldData"},
			{"Load", "LoadWorldData"},
			{"NewRightClick", "RightClick"}
		};

		public static readonly Dictionary<string, string> AnonymousMethodToAddParameter = new Dictionary<string, string> {
			{"GenerationProgress", " GameConfiguration configuration"}
		};

		public static readonly Dictionary<string, string> MemberAccessToRename = new Dictionary<string, string> {
			{"Main.PlaySound", "SoundEngine.PlaySound"},
			{"Main.npcTexture", "TextureAssets.Npc"},
			{"Main.projectileTexture", "TextureAssets.Projectile"},
			{"item.owner", "Item.playerIndexTheItemIsReservedFor"},
			{"Main.tileValue", "Main.tileOreFinderPriority"},
			{"TileObjectData.newTile.HookCheck", "TileObjectData.newTile.HookCheckIfCanPlace"},
			{"player.showItemIcon2", "player.cursorItemIconID"},
			{"player.showItemIconText", "player.cursorItemIconText"},
			{"player.showItemIcon", "player.cursorItemIconEnabled"}
		};

		public static readonly Dictionary<string, string> AssignmentsToModify = new Dictionary<string, string> {
			{"item.melee = true", "Item.DamageType = DamageClass.Melee"},
			{"item.summon = true", "Item.DamageType = DamageClass.Summon"},
			{"item.thrown = true", "Item.DamageType = DamageClass.Throwing"},
			{"item.magic = true", "Item.DamageType = DamageClass.Magic"},
			{"item.ranged = true", "Item.DamageType = DamageClass.Ranged"},
			{"projectile.melee = true", "Projectile.DamageType = DamageClass.Melee"},
			{"projectile.minion = true", "Projectile.DamageType = DamageClass.Summon"},
			{"projectile.thrown = true", "Projectile.DamageType = DamageClass.Throwing"},
			{"projectile.magic = true", "Projectile.DamageType = DamageClass.Magic"},
			{"projectile.ranged = true", "Projectile.DamageType = DamageClass.Ranged"}
		};

		// The using must contain a space before the directive
		public static readonly Dictionary<string, string> NameAddUsing = new Dictionary<string, string> {
			{"SoundEngine.PlaySound", " Terraria.Audio"},
			{" GameConfiguration configuration", " Terraria.IO"},
			{"TextureAssets.Npc", " Terraria.GameContent"},
			{"TextureAssets.Projectile", " Terraria.GameContent"},
			{"TileID.Sets.DisableSmartCursor[Type]", " Terraria.ID"},
			{"TileID.Sets.TreeSapling[Type]", " Terraria.ID"},
			{"TileID.Sets.Torch[Type]", " Terraria.ID"}
		};

		private readonly SemanticModel _semanticModel;

		private Regex _addXRegex = new Regex(@".+(\.Add\w+\(.+\))");
		private Regex _setResultRegex = new Regex(@".+\.SetResult\((.+?)(?:,\s(\d+))*\);");
		public List<string> UsingsToAdd = new List<string>();
		public PropertyRewriter(SemanticModel model) => _semanticModel = model;

		public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
		{
			if (IdentifiersToRename.TryGetValue(node.Identifier.Text, out string newIdentifier)) {
				// Only replace if the symbol wasn't found
				ISymbol? symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol == null) {
					SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();
					SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();
					SyntaxToken identifierSyntax = SyntaxFactory.Identifier(leadingTrivia, newIdentifier, trailingTrivia);

					return SyntaxFactory.IdentifierName(identifierSyntax);
				}
			}

			return base.VisitIdentifierName(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// Change return type of UseItem to bool?
			//if (node.Identifier.Text == "UseItem")
			//{
			//	return node.WithReturnType(SyntaxFactory.IdentifierName("bool? "));
			//}

			if (MethodsToRename.TryGetValue(node.Identifier.Text, out string newIdentifier)) {
				// Only replace if the symbol wasn't found
				ISymbol? symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol == null) {
					SyntaxTriviaList trailing = node.Identifier.TrailingTrivia;
					SyntaxTriviaList leading = node.Identifier.LeadingTrivia;
					SyntaxToken identifier = SyntaxFactory.Identifier(leading, newIdentifier, trailing);
					return node.WithIdentifier(identifier);
				}
			}

			if (node.Identifier.Text == "AddRecipes" && node.Body.Statements.Count != 0) {
				SyntaxTriviaList leading = node.Body.Statements.First().GetLeadingTrivia();
				SyntaxList<StatementSyntax> newStatements = new SyntaxList<StatementSyntax>();
				//ExpressionSyntax recipeExpression = null;

				string expression = "";
				int resultAmount = 1;
				string result = null;

				foreach (StatementSyntax statement in node.Body.Statements) {
					// If the statement is "recipe.AddX(a)", add it to the expression
					Match addMatch = _addXRegex.Match(statement.ToString());
					if (addMatch.Success) {
						expression += addMatch.Groups[1];
						continue;
					}

					// If the expression is "recipe.SetResult(a)", change the result and result amount
					Match setResultMatch = _setResultRegex.Match(statement.ToString());
					if (setResultMatch.Success) {
						if (setResultMatch.Groups[1].Value != "this")
							result = setResultMatch.Groups[1].Value;
						if (setResultMatch.Groups[2].Value != "")
							resultAmount = int.Parse(setResultMatch.Groups[2].Value);
						continue;
					}

					// Only do the below code if the statement contains "AddRecipe()"
					if (!statement.ToString().Contains("AddRecipe()"))
						continue;

					// Parse the expression
					string parsedExpression = $"CreateRecipe({resultAmount})";
					parsedExpression += expression;
					if (string.IsNullOrEmpty(result))
						parsedExpression += ".Register()";
					else
						parsedExpression += $".ReplaceResult({result})";

					// And add a new statement
					newStatements = newStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression(parsedExpression))
						.WithLeadingTrivia(leading).WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));

					expression = "";
					resultAmount = 1;
					result = "";
				}

				BlockSyntax modifierBody = node.Body.WithStatements(newStatements);
				return node.WithBody(modifierBody);
			}

			return base.VisitMethodDeclaration(node);
		}

		public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
		{
			if (node.ParameterList == null)
				return base.VisitAnonymousMethodExpression(node);

			// If the delegate contains any parameter in AnonymousMethodToAddParameter, add a new parameter
			foreach (ParameterSyntax parameter in node.ParameterList.Parameters) {
				if (AnonymousMethodToAddParameter.Any(p => parameter.ToString().Contains(p.Key))) {
					KeyValuePair<string, string> newParameter =
						AnonymousMethodToAddParameter.First(p => parameter.ToString().Contains(p.Key));
					SyntaxToken newIdentifier = SyntaxFactory.Identifier(newParameter.Value);

					if (node.ParameterList.Parameters.Any(p => p.ToString() == newParameter.Value.Trim()))
						return base.VisitAnonymousMethodExpression(node);

					if (NameAddUsing.TryGetValue(newIdentifier.ToString(), out string newUsing)) {
						if (!UsingsToAdd.Contains(newUsing))
							UsingsToAdd.Add(newUsing);
					}

					return node.AddParameterListParameters(SyntaxFactory.Parameter(newIdentifier));
				}
			}

			return base.VisitAnonymousMethodExpression(node);
		}

		public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			if (MemberAccessToRename.TryGetValue(node.ToString(), out string newIdentifier)) {
				// Only replace if the symbol wasn't found
				ISymbol? symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol != null)
					return base.VisitMemberAccessExpression(node);

				// Add a using if the modification needs a new using
				if (NameAddUsing.TryGetValue(newIdentifier, out string newUsing)) {
					if (!UsingsToAdd.Contains(newUsing))
						UsingsToAdd.Add(newUsing);
				}

				// Modify the node
				string[] splitIdentifier = newIdentifier.Split(new[] {"."}, 2, StringSplitOptions.None);
				node = node.WithExpression(SyntaxFactory.ParseExpression(splitIdentifier[0])
					.WithTrailingTrivia(node.Expression.GetTrailingTrivia()).WithLeadingTrivia(node.Expression.GetLeadingTrivia()));
				node = node.WithName(SyntaxFactory.IdentifierName(splitIdentifier[1]));
				return node;
			}

			return base.VisitMemberAccessExpression(node);
		}

		public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
		{
			if (AssignmentsToModify.TryGetValue(node.ToString().ToLower(), out string newAssignment)) {
				return SyntaxFactory.IdentifierName(newAssignment).WithLeadingTrivia(node.GetLeadingTrivia())
					.WithTrailingTrivia(node.GetTrailingTrivia());
			}

			return base.VisitAssignmentExpression(node);
		}

		public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
		{
			string nodeText = node.ToFullString();
			SyntaxTriviaList trailing = node.GetTrailingTrivia();
			SyntaxTriviaList leading = node.GetLeadingTrivia();

			// Replace "Terraria.World.Generation" with "Terraria.WorldBuilding"
			if (nodeText.Contains("Terraria.World.Generation"))
				return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(" Terraria.WorldBuilding")).WithTrailingTrivia(trailing)
					.WithLeadingTrivia(leading);

			return base.VisitUsingDirective(node);
		}
	}
}