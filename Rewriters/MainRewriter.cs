using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModPorter.Rewriters
{
	class MainRewriter : CSharpSyntaxRewriter
	{
		private SemanticModel _model;
		private readonly List<string> _usingList = new();
		private readonly ILookup<RewriterType, BaseRewriter> _rewriterLookup;

		public MainRewriter(SemanticModel model)
		{
			_model = model;

			Type baseType = typeof(BaseRewriter);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);

			var rewriters = types.Select(type => (BaseRewriter) Activator.CreateInstance(type, new object[] {_model, _usingList})).ToList();
			_rewriterLookup = rewriters.ToLookup(r => r.RewriterType);
		}

		private bool CallRewriters(RewriterType type, SyntaxNode node, out SyntaxNode finalNode)
		{
			bool callBaseVisit = true;
			foreach (var rewriter in _rewriterLookup[type])
			{
				callBaseVisit &= rewriter.VisitNode(node, out node);
			}
			finalNode = node;

			return callBaseVisit;
		}

		internal CompilationUnitSyntax AddUsings(CompilationUnitSyntax syntax)
		{
			List<UsingDirectiveSyntax> usingDirectives = new();
			foreach (string usingName in _usingList)
				usingDirectives.Add(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(" " + usingName))
					.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
			return syntax.AddUsings(usingDirectives.ToArray());
		}

		#region Visit Nodes

		public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) =>
			CallRewriters(RewriterType.AnonymousMethod, node, out var finalNode)
				? base.VisitAnonymousMethodExpression((AnonymousMethodExpressionSyntax) finalNode)
				: finalNode;

		public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) =>
			CallRewriters(RewriterType.Assignment, node, out var finalNode)
				? base.VisitAssignmentExpression((AssignmentExpressionSyntax)finalNode)
				: finalNode;

		public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node) =>
			CallRewriters(RewriterType.Identifier, node, out var finalNode)
				? base.VisitIdentifierName((IdentifierNameSyntax)finalNode)
				: finalNode;

		public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) =>
			CallRewriters(RewriterType.Invocation, node, out var finalNode)
				? base.VisitInvocationExpression((InvocationExpressionSyntax)finalNode)
				: finalNode;

		public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) =>
			CallRewriters(RewriterType.MemberAccess, node, out var finalNode)
				? base.VisitMemberAccessExpression((MemberAccessExpressionSyntax)finalNode)
				: finalNode;

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) =>
			CallRewriters(RewriterType.Method, node, out var finalNode)
				? base.VisitMethodDeclaration((MethodDeclarationSyntax)finalNode)
				: finalNode;

		public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node) =>
			CallRewriters(RewriterType.UsingDirective, node, out var finalNode)
				? base.VisitUsingDirective((UsingDirectiveSyntax)finalNode)
				: finalNode;

		#endregion
	}
}
