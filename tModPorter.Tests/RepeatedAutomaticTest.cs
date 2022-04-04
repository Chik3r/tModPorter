using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using NUnit.Framework;

namespace tModPorter.Tests;

public class RepeatedAutomaticTest {
	private Compilation _compilation = null!;
	private static List<SyntaxTree>? _trees;
	private static MetadataReference[] _references = null!;

	[OneTimeSetUp]
	public void Setup() {
		GetSyntaxTrees();

		_references = new MetadataReference[] {MetadataReference.CreateFromFile(typeof(object).Assembly.Location)};

		_compilation = CSharpCompilation.Create("TestAssembly", _trees, _references);
	}

	[TestCaseSource(nameof(GetTestCases))]
	public void RewriteCode(SyntaxTree tree) {
		SemanticModel model = _compilation.GetSemanticModel(tree);
		SyntaxNode rootNode = tree.GetRoot();

		CompilationUnitSyntax result = RewriteCodeOnce(model, rootNode);
		
		List<SyntaxTree> tmpTrees = new(_trees!) {
			result.SyntaxTree,
		};
		Compilation tmpCompilation = CSharpCompilation.Create("TestAssembly", tmpTrees, _references);
		model = tmpCompilation.GetSemanticModel(result.SyntaxTree);
		rootNode = result.SyntaxTree.GetRoot();
		
		result = RewriteCodeOnce(model, rootNode);

		string fixedFilePath = Path.ChangeExtension(tree.FilePath, ".Fix.cs");

		Assert.True(File.Exists(fixedFilePath), $"File '{fixedFilePath}' doesn't exist.");
		string fixedContent = File.ReadAllText(fixedFilePath);

		Assert.AreEqual(fixedContent, result.ToFullString());
	}
	
	private CompilationUnitSyntax RewriteCodeOnce(SemanticModel model, SyntaxNode rootNode) {
		MainRewriter rewriter = new(model);
		rewriter.Visit(rootNode);
		CompilationUnitSyntax? result = rewriter.RewriteNodes(rootNode) as CompilationUnitSyntax;

		Assert.NotNull(result);
		return rewriter.AddUsingDirectives(result);
	}

	public static List<SyntaxTree> GetSyntaxTrees() {
		if (_trees is not null) return _trees;

		List<string> testFiles = new(Directory.GetFiles("TestData/", "*", SearchOption.AllDirectories).Where(x => !x.Contains(".Fix.cs")));
		Assert.IsNotEmpty(testFiles);

		_trees = new List<SyntaxTree>(testFiles.Count);
		foreach (string filePath in testFiles) {
			string text = File.ReadAllText(filePath);
			_trees.Add(CSharpSyntaxTree.ParseText(text, path: filePath));
		}

		return _trees;
	}

	public static IEnumerable<TestCaseData> GetTestCases() {
		IEnumerable<SyntaxTree> syntaxTrees =
			GetSyntaxTrees().Where(x => !Path.GetDirectoryName(x.FilePath)!.Replace('\\', '/').Contains("TestData/Common"));

		TestCaseData data;
		foreach (SyntaxTree tree in syntaxTrees) {
			data = new TestCaseData(tree).SetName(Path.GetFileName(tree.FilePath));
			yield return data;
		}
	}
}