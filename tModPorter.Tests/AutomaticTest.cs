using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using NUnit.Framework;

namespace tModPorter.Tests;

public class AutomaticTest
{
    [Test]
    public void RewriteCode()
    {
        List<string> testFiles = new(Directory.GetFiles("TestData/", "*", SearchOption.AllDirectories).Where(x => !x.Contains(".Fix.cs")));
        Assert.IsNotEmpty(testFiles);

        List<SyntaxTree> trees = new(testFiles.Count);
        foreach (string filePath in testFiles) {
            string text = File.ReadAllText(filePath);
            trees.Add(CSharpSyntaxTree.ParseText(text, path: filePath));
        }

        MetadataReference[] references = { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };

        CSharpCompilation compilation = CSharpCompilation.Create("TestAssembly", trees, references);

        IEnumerable<SyntaxTree> treesToFix =
            trees.Where(x => !Path.GetDirectoryName(x.FilePath)!.Replace('\\', '/').Contains("TestData/Common"));
        foreach (SyntaxTree tree in treesToFix)
        {
            SemanticModel model = compilation.GetSemanticModel(tree);
            SyntaxNode rootNode = tree.GetRoot();

            MainRewriter rewriter = new(model);
            rewriter.Visit(rootNode);
            CompilationUnitSyntax? result = rewriter.RewriteNodes(rootNode) as CompilationUnitSyntax;
            
            Assert.NotNull(result);
            result = rewriter.AddUsingDirectives(result);

            string fixedFilePath = Path.ChangeExtension(tree.FilePath, ".Fix.cs");
            
            Assert.True(File.Exists(fixedFilePath), $"File '{fixedFilePath}' doesn't exist.");
            string fixedContent = File.ReadAllText(fixedFilePath);
            
            Assert.AreEqual(fixedContent, result.ToFullString());
        }
    }
}