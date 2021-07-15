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

        [Theory]
        [InlineData("TestData/$1/None.cs", 0)]
        [InlineData("TestData/$1/Single.cs", 1)]
        [InlineData("TestData/$1/Multiple.cs", 3)]
        public virtual void VisitNode_CheckNodeCount(string filePath, int numNodes)
        {
            string fullFilePath = filePath.Replace("$1", GetType().Name);

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

        [Theory]
        [InlineData("TestData/$1/Single.cs")]
        [InlineData("TestData/$1/Multiple.cs")]
        public virtual void RewriteNode_CompareWithTarget(string filePath, string? targetFilePath = null)
        {
            string fullFilePath = filePath.Replace("$1", GetType().Name);
            string targetFullFilePath = targetFilePath == null
                ? Path.ChangeExtension(fullFilePath, ".Fix.cs")
                : targetFilePath.Replace("$1", GetType().Name);

            string source = File.ReadAllText(fullFilePath);
            string target = File.ReadAllText(targetFullFilePath);

            // Create a compilation for the file we are going to rewrite
            Utils.CreateCSharpCompilation(source, GetType().Name, out _, out CompilationUnitSyntax root, out SemanticModel model);

            // Create a rewriter
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            SimpleIdentifierRewriter rewriter = CreateIdentifierRewriter(model, nodeSet);
            
            IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes();

            foreach (SyntaxNode assigmentNode in assigmentNodes)
                rewriter.VisitNode(assigmentNode);

            root = root.RewriteMultipleNodes(nodeSet);
            
            Assert.Equal(target, root.ToFullString());
        }
    }
}