using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.MethodDeclarationRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests {
	public class UseItemReturnTest {
		[Theory]
		[InlineData("TestData/UseItemReturnTest/None.cs", 0)]
		[InlineData("TestData/UseItemReturnTest/Single.cs", 1)]
		public void VisitNode_ShouldMatchNodeCount(string filePath, int numNodes) {
			string source = File.ReadAllText(filePath);
			CreateSimpleRewriter(source, out UseItemReturnRewriter rewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				rewriter.VisitNode(assigmentNode);

			Assert.Equal(numNodes, nodeSet.Count);
		}

		[Theory]
		[InlineData("TestData/UseItemReturnTest/Single.cs", "TestData/UseItemReturnTest/Single.Fix.cs")]
		public void RewriteNode_InputShouldMatchTarget(string inputFile, string targetFile) {
			string source = File.ReadAllText(inputFile);
			string target = File.ReadAllText(targetFile);
			CreateSimpleRewriter(source, out UseItemReturnRewriter damageClassRewriter,
				out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				damageClassRewriter.VisitNode(assigmentNode);

			root = root.RewriteMultipleNodes(nodeSet);

			Assert.Equal(target, root.ToFullString());
		}

		private static void CreateSimpleRewriter(string source, out UseItemReturnRewriter rewriter,
			out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root) {
			Utils.CreateCSharpCompilation(source, nameof(UseItemReturnTest), out _, out root, out SemanticModel model);

			nodeSet = new HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)>();
			rewriter = new UseItemReturnRewriter(model, null, nodeSet);
		}
	}
}