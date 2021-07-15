using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using tModPorter.Rewriters;
using tModPorter.Rewriters.IdentifierRewriters;
using tModPorter.Tests.RewriterTests.SimpleIdentifierTests;

namespace tModPorter.Tests.RewriterTests
{
    public sealed class ItemIdentifierRewriterTest : BaseIdentifierTest
    {
        protected override SimpleIdentifierRewriter CreateIdentifierRewriter(SemanticModel model,
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet) => new ItemIdentifierRewriter(model, null, nodeSet);

        /*
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
        */
    }
}