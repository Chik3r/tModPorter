using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using tModPorter.Rewriters;
using NUnit.Framework;

namespace tModPorter.Tests;

public class AutomaticTest {
	private static Compilation? _compilation;
	private static Project? _project;
	private static Workspace? _workspace;

	[OneTimeSetUp]
	public async Task Setup() {
		await LoadProject();

		MetadataReference[] references = {MetadataReference.CreateFromFile(typeof(object).Assembly.Location)};
	}

	[TestCaseSource(nameof(GetTestCases))]
	public void RewriteCode(SyntaxTree tree) {
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

	public static async Task LoadProject() {
		if (_workspace is not null && _project is not null && _compilation is not null) return;
		
		MSBuildLocator.RegisterDefaults();

		using MSBuildWorkspace workspace = MSBuildWorkspace.Create();
		_workspace = workspace;

		var a = Directory.EnumerateFiles("TestData/", "*");
		
		if (!File.Exists("TestData/TestData.csproj")) {
			throw new FileNotFoundException("TestData.csproj not found.");
		}
		
		_project = await workspace.OpenProjectAsync("TestData/TestData.csproj");
		
		_compilation = await _project.GetCompilationAsync();
	}

	public static IEnumerable<SyntaxTree> GetSyntaxTrees() {
		return GetSyntaxTreesAsync().ToEnumerable();
	}

	public static async IAsyncEnumerable<SyntaxTree> GetSyntaxTreesAsync() {
		await LoadProject();
		if (_project is null) {
			throw new NullReferenceException(nameof(_project));
		}
		
		foreach (Document document in _project.Documents) {
			SyntaxTree tree = await document.GetSyntaxTreeAsync() ?? throw new Exception("No syntax tree found for: " + document.FilePath);
			if (tree.FilePath.Replace('\\', '/').Contains("TestData/Common")) continue;
			yield return tree;
		}
	}

	public static IEnumerable<TestCaseData> GetTestCases() {
		var a = GetSyntaxTrees().ToList();
		foreach (SyntaxTree tree in GetSyntaxTrees()) {
			TestCaseData data = new TestCaseData(tree).SetName(Path.GetFileName(tree.FilePath));
			yield return data;
		}
	}
}