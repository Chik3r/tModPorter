using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.IdentifierRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests.SimpleIdentifierTests
{
    public abstract class BaseIdentifierTest
    {
        protected abstract SimpleIdentifierRewriter CreateIdentifierRewriter(SemanticModel model,
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet);

        protected virtual bool ShouldRunTest(string filePath) => true;
        
        [Theory]
        [InlineData("TestData/$1/VisitNode_None.cs", 0)]
        [InlineData("TestData/$1/VisitNode_Single.cs", 1)]
        [InlineData("TestData/$1/VisitNode_Multiple.cs", 3)]
        public virtual void VisitNode_CheckNodeCount(string filePath, int numNodes)
        {
            string fullFilePath = filePath.Replace("$1", GetType().Name);
            if (!ShouldRunTest(fullFilePath)) return;

            string source = File.ReadAllText(fullFilePath);
            
            // Create a compilation for the file we are going to rewrite
            Utils.CreateCSharpCompilation(source, GetType().Name, out _, out CompilationUnitSyntax root, out SemanticModel model);
            
            // Create a rewriter
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            SimpleIdentifierRewriter rewriter = CreateIdentifierRewriter(model, nodeSet);
            
            // Get all nodes in the file
            IEnumerable<SyntaxNode> descendantNodes = root.DescendantNodes();

            // Visit the nodes and add them to the nodeSet
            foreach (SyntaxNode assigmentNode in descendantNodes)
                rewriter.VisitNode(assigmentNode);
            
            Assert.Equal(numNodes, nodeSet.Count);
        }
    }
}