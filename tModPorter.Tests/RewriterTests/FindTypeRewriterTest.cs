using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.InvocationRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests
{
    public class FindTypeRewriterTest
    {
        [Theory]
        [InlineData("TestData/FindTypeRewriterTest/None.cs", 0)]
        [InlineData("TestData/FindTypeRewriterTest/All.cs", 10)]
        public void VisitNode_ShouldMatchNodeCount(string filePath, int numNodes)
        {
            string source = File.ReadAllText(filePath);
            CreateSimpleRewriter(source, out FindTypeRewriter rewriter,
                out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

            foreach (SyntaxNode assigmentNode in root.DescendantNodes())
                rewriter.VisitNode(assigmentNode);

            Assert.Equal(numNodes, nodeSet.Count);
        }

        [Theory]
        [InlineData("TestData/FindTypeRewriterTest/All.cs", "TestData/FindTypeRewriterTest/All.Fix.cs")]
        public void RewriteNode_InputShouldMatchTarget(string inputFile, string targetFile)
        {
            string source = File.ReadAllText(inputFile);
            string target = File.ReadAllText(targetFile);
            CreateSimpleRewriter(source, out FindTypeRewriter damageClassRewriter,
                out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root);

            foreach (SyntaxNode assigmentNode in root.DescendantNodes())
                damageClassRewriter.VisitNode(assigmentNode);

            root = root.RewriteMultipleNodes(nodeSet);

            Assert.Equal(target, root.ToFullString());
        }

        private void CreateSimpleRewriter(string source, out FindTypeRewriter rewriter,
            out HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet, out CompilationUnitSyntax root)
        {
            Utils.CreateCSharpCompilation(source, nameof(FindTypeRewriterTest), out _, out root, out SemanticModel model);

            nodeSet = new HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)>();
            rewriter = new FindTypeRewriter(model, null, nodeSet);
        }
    }
}