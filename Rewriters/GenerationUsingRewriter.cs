using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters
{
	class GenerationUsingRewriter : BaseRewriter
	{
		public GenerationUsingRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override RewriterType RewriterType => RewriterType.UsingDirective;

		public override bool VisitNode(SyntaxNode node, out SyntaxNode finalNode)
		{
			UsingDirectiveSyntax nodeSyntax = node as UsingDirectiveSyntax;
			finalNode = node;
			if (nodeSyntax.Name.ToString() == "Terraria.World.Generation")
				finalNode = nodeSyntax.WithName(IdentifierName("Terraria.WorldBuilding"));

			return true;
		}
	}
}
