using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace tModPorter.Rewriters
{
	class TestUsingRewriter : BaseRewriter
	{
		public TestUsingRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override RewriterType RewriterType => RewriterType.UsingDirective;

		public override SyntaxNode VisitNode(SyntaxNode node)
		{
			UsingDirectiveSyntax nodeSyntax = node as UsingDirectiveSyntax;
			if (nodeSyntax.Name.ToString() == "Terraria.World.Generation")
			{
				Console.WriteLine("Found a thingy! - " + nodeSyntax.Name);
				return nodeSyntax.WithName(IdentifierName("Terraria.WorldBuilding"));
			}

			//Console.WriteLine($"Processing: {node}");
			return base.VisitNode(node);
		}
	}
}
