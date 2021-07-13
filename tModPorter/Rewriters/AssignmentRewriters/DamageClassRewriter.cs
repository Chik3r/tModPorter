using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.AssignmentRewriters
{
	public class DamageClassRewriter : BaseRewriter
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
			AssignmentExpressionSyntax nodeAssigment = (AssignmentExpressionSyntax) node;
			MemberAccessExpressionSyntax leftMember = (MemberAccessExpressionSyntax) nodeAssigment.Left;

			// If the assigned value is false, comment out the line
			if (nodeAssigment.Right is LiteralExpressionSyntax literalExpression &&
			    literalExpression.Kind() == SyntaxKind.FalseLiteralExpression)
			{
				SyntaxTriviaList leftTriviaList = leftMember.GetLeadingTrivia().Add(Comment("// "));
				ExpressionSyntax leftMemberCommented = leftMember.WithLeadingTrivia(leftTriviaList); 
				return nodeAssigment.WithLeft(leftMemberCommented);
			}

			KeyValuePair<string, string> newDamage = _fieldToDamageClass.First(f => f.Key == leftMember.Name.ToString());

			MemberAccessExpressionSyntax modifiedLeft = leftMember.WithName(IdentifierName("DamageType").WithExtraTrivia(leftMember.Name));
			ExpressionSyntax newRight = ParseExpression(newDamage.Value).WithExtraTrivia(nodeAssigment.Right);
			AssignmentExpressionSyntax modifiedAssignment = nodeAssigment.WithLeft(modifiedLeft).WithRight(newRight);

			return modifiedAssignment;
		}
	}
}
