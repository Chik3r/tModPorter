using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters
{
	class ModTypeRewriter : BaseRewriter
	{
		public ModTypeRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

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

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not InvocationExpressionSyntax nodeSyntax)
				return;

			// Support nullable invocations
			var memberAccess = nodeSyntax.Expression as MemberAccessExpressionSyntax;
			var memberBinding = nodeSyntax.Expression as MemberBindingExpressionSyntax;
			if (memberAccess == null && memberBinding == null)
				return;

			if ((memberAccess != null && HasSymbol(memberAccess.Name, out _)) ||
			    (memberBinding != null && HasSymbol(memberBinding.Name, out _)))
				return;

			if (_modTypes.Any(m => m.Key == memberAccess?.Name.ToString()) ||
			    _modTypes.Any(m => m.Key == memberBinding?.Name.ToString()))
				AddNodeToRewrite(nodeSyntax);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			var nodeSyntax = (InvocationExpressionSyntax) node;
			var memberAccess = nodeSyntax.Expression as MemberAccessExpressionSyntax;
			var memberBinding = nodeSyntax.Expression as MemberBindingExpressionSyntax;

			var modType = _modTypes.First(m => 
				(m.Key == memberAccess?.Name.ToString()) || (m.Key == memberBinding?.Name.ToString()));

			// Replace 'mod' with 'Mod'
			if (memberAccess?.Expression.ToString() == "mod")
				memberAccess = memberAccess.WithExpression(IdentifierName("Mod"));

			// Replace depending on the type of the expression
			if (memberAccess != null)
			{
				// Replace the old 'XType' with the new 'Find<XType>'
				var newMemberExpression = memberAccess.WithName(IdentifierName($"Find<{modType.Value}>"));
				var newNode = nodeSyntax.WithExpression(newMemberExpression);

				// Add .Type at the end
				var newMember = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, newNode, IdentifierName("Type"));
				return newMember;
			}
			else
			{
				// Replace the old 'XType' with the new 'Find<XType>'
				var newMemberExpression = memberBinding.WithName(IdentifierName($"Find<{modType.Value}>"));
				var newNode = nodeSyntax.WithExpression(newMemberExpression);

				// Add .Type at the end
				var newMember = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, newNode, IdentifierName("Type"));
				return newMember;
			}
		}
	}
}
