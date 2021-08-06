using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.MemberAccessRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests
{
	public class TextureExistsTest
	{
		[Theory]
		[InlineData("TextureExists(\"string\")", 0)]
		[InlineData("ModContent.TextureExists(\"string\")", 1)]
		[InlineData("if (ModContent.TextureExists(field)) { Write(ModContent.TextureExists(field)) }", 2)]
		public void VisitNode_ShouldMatchNodeCount(string statement, int nodeCount)
		{
			CreateSimpleRewriter(statement, out TextureExistsRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);
			
			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				rewriter.VisitNode(assigmentNode);

			Assert.Equal(nodeCount, nodeSet.Count);
		}

		[Fact]
		public void RewriteNode_ShouldMatchTarget()
		{
			const string statement = "ModContent.TextureExists(\"string\")";
			const string target = "ModContent.HasAsset(\"string\")";
			
			CreateSimpleRewriter(statement, out TextureExistsRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);
			
			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				rewriter.VisitNode(assigmentNode);
			
			root = root.RewriteMultipleNodes(nodeSet);

			Assert.Equal(target, root.ToFullString());
		}
		
		private static void CreateSimpleRewriter(string source, out TextureExistsRewriter rewriter,
			out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root) {
			Utils.CreateCSharpCompilation(source, nameof(FindTypeRewriterTest), out _, out root, out SemanticModel model);

			nodeSet = new HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)>();
			rewriter = new TextureExistsRewriter(model, null, nodeSet, null);
		}
	}
}