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

		CompilationUnitSyntax result = RewriteCodeOnce(model, rootNode);

		string fixedFilePath = Path.ChangeExtension(tree.FilePath, ".Fix.cs");

		Assert.True(File.Exists(fixedFilePath), $"File '{fixedFilePath}' doesn't exist.");
		string fixedContent = File.ReadAllText(fixedFilePath);

		Assert.AreEqual(fixedContent, result.ToFullString());
	}
	
	[TestCaseSource(nameof(GetTestCases))]
	public async Task RewriteCodeTwice(SyntaxTree tree) {
		Document newDoc = _project!.Documents.FirstOrDefault(x => x.FilePath == tree.FilePath)!;
		tree = await newDoc.GetSyntaxTreeAsync() ?? throw new NullReferenceException("Node has no syntax tree");
		SemanticModel model = _compilation!.GetSemanticModel(tree);
		SyntaxNode rootNode = await tree.GetRootAsync();

		CompilationUnitSyntax result = RewriteCodeOnce(model, rootNode);
		
		// Write the rewritten file to disk, so that we can then load it again with the .csproj
		await File.WriteAllTextAsync(tree.FilePath, result.ToString());
		await LoadProject(true);

		newDoc = _project.Documents.FirstOrDefault(x => x.FilePath == tree.FilePath)!;
		Assert.NotNull(newDoc, "Couldn't load the rewritten file from disk.");

		tree = await newDoc.GetSyntaxTreeAsync() ?? throw new NullReferenceException("Node has no syntax tree");
		
		model = _compilation.GetSemanticModel(tree);
		rootNode = await tree!.GetRootAsync();

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

	public static async Task LoadProject(bool force = false) {
		if (!force && _workspace is not null && _project is not null && _compilation is not null) return;
		
		if (!MSBuildLocator.IsRegistered)
			MSBuildLocator.RegisterDefaults();

		using MSBuildWorkspace workspace = MSBuildWorkspace.Create();
		_workspace = workspace;
		
		if (!File.Exists("TestData/TestData.csproj")) {
			throw new FileNotFoundException("TestData.csproj not found.");
		}
		
		_project = await workspace.OpenProjectAsync("TestData/TestData.csproj");
		
		_compilation = await _project.GetCompilationAsync();

		if (_workspace is null) throw new NullReferenceException(nameof(_workspace));
		if (_project is null) throw new NullReferenceException(nameof(_project));
		if (_compilation is null) throw new NullReferenceException(nameof(_compilation));
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