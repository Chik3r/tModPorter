using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            model = compilation.GetSemanticModel(tree);

            return compilation;
        }
    }
}