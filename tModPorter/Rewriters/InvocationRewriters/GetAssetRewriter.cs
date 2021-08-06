using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters.InvocationRewriters {
	public class GetAssetRewriter : BaseRewriter {
		private Dictionary<string, (string type, string addUsing)> _methodToType = new() {
			{"GetTexture", ("Texture2D", "Microsoft.Xna.Framework.Graphics")},
			{"GetEffect", ("Effect", "Microsoft.Xna.Framework.Graphics")}
		};

		public GetAssetRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		public override RewriterType RewriterType => RewriterType.Invocation;

		public override void VisitNode(SyntaxNode node) {
			if (node is not InvocationExpressionSyntax invocation) return;

			if (invocation.ArgumentList.Arguments.Count != 1) return;
			if (invocation.Expression is not MemberAccessExpressionSyntax member) return;
			if (!_methodToType.ContainsKey(member.Name.ToString())) return;

			AddNodeToRewrite(member);
			
			// TODO: Maybe add .Value after Request<> ?
		}

		public override SyntaxNode RewriteNode(SyntaxNode node) {
			if (node is not MemberAccessExpressionSyntax member) return node;

			(string type, string addUsing) = _methodToType[member.Name.ToString()];
			AddUsing(addUsing);

			TypeArgumentListSyntax typeArgList = TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(type)));
			GenericNameSyntax requestSyntax = GenericName("Request").WithTypeArgumentList(typeArgList);

			if (HasSymbol(member.Expression, out ISymbol symbol) && symbol.Name == "ModContent") {
				// do stuff for ModLoader.Request<>()
				requestSyntax = requestSyntax.WithTriviaFrom(member.Name);

				MemberAccessExpressionSyntax newMember =
					MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, member.Expression, requestSyntax)
						.WithTriviaFrom(member);
				return newMember;
			}

			// turn 'mod' to 'Mod'
			if (!HasSymbol(member.Expression, out _) && member.Expression.ToString() == "mod") {
				member = member.WithExpression(IdentifierName("Mod").WithTriviaFrom(member.Expression));
			}

			
			// do stuff for Mod.Assets.Request<>()
			// Create Mod.Assets
			MemberAccessExpressionSyntax modAssetsSyntax =
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, member.Expression, IdentifierName("Assets"));
			
			// Create Mod.Assets.Request<>()
			MemberAccessExpressionSyntax assetsRequestSyntax =
				MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, modAssetsSyntax, requestSyntax);
			
			return assetsRequestSyntax.WithTriviaFrom(node);
		}
	}
}