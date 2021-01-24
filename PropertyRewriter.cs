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
		public static readonly Dictionary<string, string> IdentifiersToRename = new Dictionary<string, string>
		{
			{"item", "Item"},
			{"mod", "Mod"},
			{"player", "Player"},
			{"DustType", "Find<ModDust>"},
			{"ProjectileType", "Find<ModProjectile>"},
			{"disableSmartCursor", "TileID.Sets.DisableSmartCursor[Type]"},
			{"ModWorld", "ModSystem"},
			{"rangedCrit", "GetCrit(DamageClass.Ranged)"},
			{"magicCrit", "GetCrit(DamageClass.Magic)"},
			{"thrownCrit", "GetCrit(DamageClass.Throwing)"},
			{"meleeCrit", "GetCrit(DamageClass.Melee)"},
			{"rangedDamage", "GetDamage(DamageClass.Ranged)"},
			{"magicDamage", "GetDamage(DamageClass.Magic)"},
			{"thrownDamage", "GetDamage(DamageClass.Throwing)"},
			{"meleeDamage", "GetDamage(DamageClass.Melee)"},
			{"minionDamage", "GetDamage(DamageClass.Summon)"},
		};

		public static readonly Dictionary<string, string> MethodsToRename = new Dictionary<string, string>
		{
			{"Initialize", "OnWorldLoad"},
			{"Save", "SaveWorldData"},
			{"Load", "LoadWorldData"}
		};

		public static readonly Dictionary<string, string> AnonymousMethodToAddParameter = new Dictionary<string, string>
		{
			{"GenerationProgress", " GameConfiguration configuration"}
		};

		public static readonly Dictionary<string, string> MemberAccessToRename = new Dictionary<string, string>
		{
			{"Main.PlaySound", "SoundEngine.PlaySound"},
		};

		// The using must contain a space before the directive
		public static readonly Dictionary<string, string> NameAddUsing = new Dictionary<string, string>
		{
			{"SoundEngine.PlaySound", " Terraria.Audio"},
			{" GameConfiguration configuration", " Terraria.IO"}
		};
		public List<string> UsingsToAdd = new List<string>();

		private readonly SemanticModel _semanticModel;
		public PropertyRewriter(SemanticModel model) => _semanticModel = model;

		public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
		{
			if (IdentifiersToRename.TryGetValue(node.Identifier.Text, out string newIdentifier))
			{
				// Only replace if the symbol wasn't found
				var symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol == null)
				{
					var leadingTrivia = node.GetLeadingTrivia();
					var trailingTrivia = node.GetTrailingTrivia();
					var identifierSyntax = SyntaxFactory.Identifier(leadingTrivia, newIdentifier, trailingTrivia);

					return SyntaxFactory.IdentifierName(identifierSyntax);
				}
			}

			return base.VisitIdentifierName(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// Change return type of UseItem to bool?
			if (node.Identifier.Text == "UseItem")
			{
				return node.WithReturnType(SyntaxFactory.IdentifierName("bool? "));
			}

			if (MethodsToRename.TryGetValue(node.Identifier.Text, out string newIdentifier))
			{
				// Only replace if the symbol wasn't found
				var symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol == null)
				{
					var trailing = node.Identifier.TrailingTrivia;
					var leading = node.Identifier.LeadingTrivia;
					var identifier = SyntaxFactory.Identifier(leading, newIdentifier, trailing);
					return node.WithIdentifier(identifier);
				}
			}

			return base.VisitMethodDeclaration(node);
		}

		public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
		{
			// If the delegate contains any parameter in AnonymousMethodToAddParameter, add a new parameter
			foreach (ParameterSyntax parameter in node.ParameterList.Parameters)
			{
				if (AnonymousMethodToAddParameter.Any(p => parameter.ToString().Contains(p.Key)))
				{
					var newIdentifier = SyntaxFactory.Identifier(AnonymousMethodToAddParameter.First(p => parameter.ToString().Contains(p.Key)).Value);

					if (NameAddUsing.TryGetValue(newIdentifier.ToString(), out string newUsing))
					{
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
			if (MemberAccessToRename.TryGetValue(node.ToString(), out string newIdentifier))
			{
				// Only replace if the symbol wasn't found
				var symbol = _semanticModel.GetSymbolInfo(node).Symbol;
				if (symbol != null)
					return base.VisitMemberAccessExpression(node);

				// Add a using if the modification needs a new using
				if (NameAddUsing.TryGetValue(newIdentifier, out string newUsing))
				{
					if (!UsingsToAdd.Contains(newUsing))
						UsingsToAdd.Add(newUsing);
				}

				// Modify the node
				var splitIdentifier = newIdentifier.Split(new[] { "." }, 2, StringSplitOptions.None);
				node = node.WithExpression(SyntaxFactory.ParseExpression(splitIdentifier[0])
					.WithTrailingTrivia(node.Expression.GetTrailingTrivia()).WithLeadingTrivia(node.Expression.GetLeadingTrivia()));
				node = node.WithName(SyntaxFactory.IdentifierName(splitIdentifier[1]));
				return node;
			}

			return base.VisitMemberAccessExpression(node);
		}

		public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
		{
			var nodeText = node.ToFullString();
			var trailing = node.GetTrailingTrivia();
			var leading = node.GetLeadingTrivia();

			// Replace "Terraria.World.Generation" with "Terraria.WorldBuilding"
			if (nodeText.Contains("Terraria.World.Generation")) 
				return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(" Terraria.WorldBuilding")).WithTrailingTrivia(trailing).WithLeadingTrivia(leading);

			return base.VisitUsingDirective(node);
		}
	}
}
