using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.AssignmentRewriters
{
	class DamageClassRewriter : BaseRewriter
	{
		private Dictionary<string, string> _fieldToDamageClass = new()
		{
			{"magic", "DamageClass.Magic"},
			{"melee", "DamageClass.Melee"},
			{"ranged", "DamageClass.Ranged"},
			{"summon", "DamageClass.Summon"},
			{"thrown", "DamageClass.Throwing"},
		};
		
		public DamageClassRewriter(SemanticModel model, List<string> usingList, 
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		public override RewriterType RewriterType => RewriterType.Assignment;

		public override void VisitNode(SyntaxNode node)
		{
			if (node is not AssignmentExpressionSyntax nodeAssignment)
				return;

			if (nodeAssignment.Left is not MemberAccessExpressionSyntax leftMember)
				return;

			if (_fieldToDamageClass.Any(f => f.Key == leftMember.Name.ToString()))
				AddNodeToRewrite(nodeAssignment);
		}

		public override SyntaxNode RewriteNode(SyntaxNode node)
		{
			var nodeAssigment = (AssignmentExpressionSyntax) node;
			var leftMember = (MemberAccessExpressionSyntax) nodeAssigment.Left;

			var newDamage = _fieldToDamageClass.First(f => f.Key == leftMember.Name.ToString());

			var modifiedLeft = leftMember.WithName(IdentifierName("DamageType").WithExtraTrivia(leftMember.Name));
			var newRight = ParseExpression(newDamage.Value).WithExtraTrivia(nodeAssigment.Right);
			var modifiedAssignment = nodeAssigment.WithLeft(modifiedLeft).WithRight(newRight);

			return modifiedAssignment;
		}
	}
}
