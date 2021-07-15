using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.IdentifierRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests
{
    public class ItemIdentifierRewriterTests
    {
        [Theory]
        [InlineData("TestData/ItemIdentifierRewriterData/VisitNode_None.cs", 0)]
        [InlineData("TestData/ItemIdentifierRewriterData/VisitNode_Single.cs", 1)]
        [InlineData("TestData/ItemIdentifierRewriterData/VisitNode_Multiple.cs", 2)]
        public void VisitNode_CheckNodeCount(string inputFile, int numberMatches)
        {
            string source = File.ReadAllText(inputFile);
            Utils.CreateCSharpCompilation(source, "ItemIdentifierTest", out _, out CompilationUnitSyntax root, out SemanticModel model);
            
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            SimpleIdentifierRewriter rewriter = new ItemIdentifierRewriter(model, null, nodeSet);
            
            IEnumerable<SyntaxNode> descendantNodes = root.DescendantNodes();

            foreach (SyntaxNode assigmentNode in descendantNodes)
                rewriter.VisitNode(assigmentNode);
            
            Assert.Equal(numberMatches, nodeSet.Count);
        }
        
        [Theory]
        [InlineData("TestData/ItemIdentifierRewriterData/RewriteNode_Single.cs", "TestData/ItemIdentifierRewriterData/RewriteNode_Single.Fix.cs")]
        [InlineData("TestData/ItemIdentifierRewriterData/RewriteNode_Multiple.cs", "TestData/ItemIdentifierRewriterData/RewriteNode_Multiple.Fix.cs")]
        public void RewriteNodeTest(string inputFile, string targetFile)
        {
            string source = File.ReadAllText(inputFile);
            string target = File.ReadAllText(targetFile);
            Utils.CreateCSharpCompilation(source, "ItemIdentifierTest", out _, out CompilationUnitSyntax root, out SemanticModel model);
            
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            SimpleIdentifierRewriter rewriter = new ItemIdentifierRewriter(model, null, nodeSet);

            IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes();

            foreach (SyntaxNode assigmentNode in assigmentNodes)
                rewriter.VisitNode(assigmentNode);

            root = root.RewriteMultipleNodes(nodeSet);

            Assert.Equal(target, root.ToFullString());
        }
    }
}