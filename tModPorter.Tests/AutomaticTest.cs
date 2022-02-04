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
    private Compilation _compilation;
    private static List<SyntaxTree>? Trees;
    
    [OneTimeSetUp]
    public void Setup()
    {
        GetSyntaxTrees();

        MetadataReference[] references = { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };

        _compilation = CSharpCompilation.Create("TestAssembly", Trees, references);
    }
    
    [TestCaseSource(nameof(GetTestCases))]
    public void RewriteCode(SyntaxTree tree)
    {
        SemanticModel model = _compilation.GetSemanticModel(tree);
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

    public static List<SyntaxTree> GetSyntaxTrees()
    {
        if (Trees is not null) return Trees;
        
        List<string> testFiles = new(Directory.GetFiles("TestData/", "*", SearchOption.AllDirectories).Where(x => !x.Contains(".Fix.cs")));
        Assert.IsNotEmpty(testFiles);

        Trees = new(testFiles.Count);
        foreach (string filePath in testFiles) {
            string text = File.ReadAllText(filePath);
            Trees.Add(CSharpSyntaxTree.ParseText(text, path: filePath));
        }

        return Trees;
    }

    public static IEnumerable<TestCaseData> GetTestCases()
    {
        IEnumerable<SyntaxTree> syntaxTrees =
            GetSyntaxTrees().Where(x => !Path.GetDirectoryName(x.FilePath)!.Replace('\\', '/').Contains("TestData/Common"));

        TestCaseData data;
        foreach (SyntaxTree tree in syntaxTrees)
        {
            data = new TestCaseData(tree).SetName(Path.GetFileName(tree.FilePath));
            yield return data;
        }
    }
}