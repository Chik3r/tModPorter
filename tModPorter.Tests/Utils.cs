using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;

namespace tModPorter.Tests
{
    public static class Utils
    {
        public static CSharpCompilation CreateCSharpCompilation(string source, string assemblyName, out SyntaxTree tree,
            out CompilationUnitSyntax root, out SemanticModel model)
        {
            tree = CSharpSyntaxTree.ParseText(source);

            root = tree.GetCompilationUnitRoot();

            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName)
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(MainRewriter).Assembly.Location))
                .AddSyntaxTrees(tree);

            model = compilation.GetSemanticModel(tree);

            return compilation;
        }

        internal static CompilationUnitSyntax RewriteMultipleNodes(this CompilationUnitSyntax root,
            IEnumerable<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet)
        {
            Dictionary<SyntaxNode, SyntaxNode> nodeDictionary = new();
            foreach ((BaseRewriter rewriter, SyntaxNode originalNode) in nodeSet)
            {
                SyntaxNode newNode = rewriter.RewriteNode(originalNode);
                nodeDictionary.Add(originalNode, newNode);
            }

            return root.ReplaceNodes(nodeDictionary.Keys.AsEnumerable(), (n1, n2) => nodeDictionary[n1]);
        }
    }
}