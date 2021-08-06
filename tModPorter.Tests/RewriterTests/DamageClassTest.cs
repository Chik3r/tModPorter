using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.AssignmentRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests {
	public class DamageClassTest {
		[Theory]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_None.cs", 0)]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_Single.cs", 2)]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_Multiple.cs", 8)]
		public void VisitNode_CheckNodeCount(string inputFile, int numNodesToFind) {
			string source = File.ReadAllText(inputFile);
			CreateSimpleRewriter(source, out DamageClassRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes().Where(x => x is AssignmentExpressionSyntax);

			foreach (SyntaxNode assigmentNode in assigmentNodes)
				rewriter.VisitNode(assigmentNode);

			Assert.Equal(numNodesToFind, nodeSet.Count);
		}

		[Theory]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_None.cs")]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_SimilarName.cs")]
		public void VisitNode_CheckNodeCount_WhenWrongStatementType(string inputFile) {
			string source = File.ReadAllText(inputFile);

			CreateSimpleRewriter(source, out DamageClassRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes().Where(x => x is not AssignmentExpressionSyntax);

			foreach (SyntaxNode assigmentNode in assigmentNodes)
				rewriter.VisitNode(assigmentNode);

			Assert.Empty(nodeSet);
		}

		[Theory]
		[InlineData("TestData/DamageClassRewriterData/VisitNode_SimilarName.cs")]
		public void VisitNode_CheckNodeCount_WhenSimilarFieldName(string inputFile) {
			string source = File.ReadAllText(inputFile);

			CreateSimpleRewriter(source, out DamageClassRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes().Where(x => x is AssignmentExpressionSyntax);

			foreach (SyntaxNode assigmentNode in assigmentNodes)
				rewriter.VisitNode(assigmentNode);

			Assert.Empty(nodeSet);
		}

		[Fact]
		public void RewriterTypeGet() {
			DamageClassRewriter rewriter = new(null, null, null, null);
			Assert.Equal(RewriterType.Assignment, rewriter.RewriterType);
		}

		[Theory]
		[InlineData("TestData/DamageClassRewriterData/RewriteNode_Single.cs", "TestData/DamageClassRewriterData/RewriteNode_Single.Fix.cs")]
		[InlineData("TestData/DamageClassRewriterData/RewriteNode_Multiple.cs",
			"TestData/DamageClassRewriterData/RewriteNode_Multiple.Fix.cs")]
		[InlineData("TestData/DamageClassRewriterData/RewriteNode_Advanced.cs",
			"TestData/DamageClassRewriterData/RewriteNode_Advanced.Fix.cs")]
		public void RewriteNodeTest(string inputFile, string targetFile) {
			string source = File.ReadAllText(inputFile);
			string target = File.ReadAllText(targetFile);
			CreateSimpleRewriter(source, out DamageClassRewriter damageClassRewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes();

			foreach (SyntaxNode assigmentNode in assigmentNodes)
				damageClassRewriter.VisitNode(assigmentNode);

			root = root.RewriteMultipleNodes(nodeSet);

			Assert.Equal(target, root.ToFullString());
		}

		private void CreateSimpleRewriter(string source, out DamageClassRewriter rewriter,
			out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root) {
			Utils.CreateCSharpCompilation(source, "DamageClassTest", out _, out root, out SemanticModel model);

			nodeSet = new HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)>();
			rewriter = new DamageClassRewriter(model, null, nodeSet, null);
		}
	}
}