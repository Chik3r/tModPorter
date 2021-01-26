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

		private SyntaxNode CallRewriters(RewriterType type, SyntaxNode node)
		{
			foreach (var rewriter in _rewriterLookup[type])
			{
				node = rewriter.VisitNode(node);
			}

			return node;
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
			CallRewriters(RewriterType.AnonymousMethod, node);

		public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) =>
			CallRewriters(RewriterType.Assignment, node);

		public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node) => 
			CallRewriters(RewriterType.Identifier, node);

		public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node) =>
			CallRewriters(RewriterType.Invocation, node);

		public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node) =>
			CallRewriters(RewriterType.MemberAccess, node);

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) => 
			CallRewriters(RewriterType.Method, node);

		public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node) =>
			CallRewriters(RewriterType.UsingDirective, node);

		#endregion
	}
}
