using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters
{
	class ModTypeRewriter : BaseRewriter
	{
		public ModTypeRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override RewriterType RewriterType => RewriterType.Invocation;

		// A dictionary to convert old 'Mod.XType("Name")' to the new 'Mod.Find<XType>("Name")'
		private Dictionary<string, string> _modTypes = new()
		{
			{"BuffType", "ModBuff"},
			{"DustType", "ModDust"},
			{"ItemType", "ModItem"},
			{"MountType", "ModMountData"},
			{"NPCType", "ModNPC"},
			{"PrefixType", "ModPrefix"},
			{"ProjectileType", "ModProjectile"},
			{"TileEntityType", "ModTileEntity"},
			{"TileType", "ModTile"},
			{"WallType", "ModWall"},
		};

		public override bool VisitNode(SyntaxNode node, out SyntaxNode finalNode)
		{
			var nodeSyntax = (InvocationExpressionSyntax) node;
			finalNode = node;

			if (nodeSyntax.Expression is not MemberAccessExpressionSyntax memberAccess)
				return true;

			if (HasSymbol(memberAccess.Name, out _))
				return true;

			foreach (var modType in _modTypes)
			{
				if (memberAccess.Name.ToString() != modType.Key) 
					continue;

				// Replace 'mod' with 'Mod'
				if (memberAccess.Expression.ToString() == "mod")
					memberAccess = memberAccess.WithExpression(IdentifierName("Mod"));

				// Replace the old 'XType' with the new 'Find<XType>'
				var newMemberExpression = memberAccess.WithName(IdentifierName($"Find<{modType.Value}>"));
				var newNode = nodeSyntax.WithExpression(newMemberExpression);

				// Add .Type at the end
				var newMember = MemberAccessExpression(memberAccess.Kind(), newNode, IdentifierName("Type"));
				finalNode = newMember;

				return false;
			}

			return true;
		}
	}
}
