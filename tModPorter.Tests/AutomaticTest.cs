using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using tModPorter.Rewriters;
using NUnit.Framework;

namespace tModPorter.Tests;

// TODO: Make test using tModPorter class
public class AutomaticTest {
	private static Compilation? _compilation;
	private static Project? _project;
	private static Workspace? _workspace;

	[OneTimeSetUp]
	public async Task Setup() {
		await LoadProject();
	}

	[TestCaseSource(nameof(GetTestCases))]
	public void RewriteCode(SyntaxTree tree) {
		Document newDoc = _project!.Documents.First(x => x.FilePath == tree.FilePath);
		SemanticModel model = _compilation!.GetSemanticModel(tree);
		SyntaxNode rootNode = tree.GetRoot();

		CompilationUnitSyntax result = RewriteCodeOnce(newDoc, model, rootNode);

		string fixedFilePath = Path.ChangeExtension(tree.FilePath, ".Fix.cs");

		Assert.True(File.Exists(fixedFilePath), $"File '{fixedFilePath}' doesn't exist.");
		string fixedContent = File.ReadAllText(fixedFilePath);

		Assert.AreEqual(fixedContent, result.ToFullString());
	}
	
	[TestCaseSource(nameof(GetTestCases))]
	public async Task RewriteCodeTwice(SyntaxTree tree) {
		Document newDoc = _project!.Documents.First(x => x.FilePath == tree.FilePath);
		tree = await newDoc.GetSyntaxTreeAsync() ?? throw new NullReferenceException("Node has no syntax tree");
		SemanticModel model = _compilation!.GetSemanticModel(tree);
		SyntaxNode rootNode = await tree.GetRootAsync();

		CompilationUnitSyntax result = RewriteCodeOnce(newDoc, model, rootNode);
		
		// Write the rewritten file to disk, so that we can then load it again with the .csproj
		newDoc = newDoc.WithSyntaxRoot(result);
		tree = await newDoc.GetSyntaxTreeAsync() ?? throw new NullReferenceException("Node has no syntax tree");
		Compilation? newCompilation = await newDoc.Project.GetCompilationAsync();
		Assert.NotNull(newCompilation);
		model = newCompilation!.GetSemanticModel(tree);
		rootNode = await tree.GetRootAsync();

		result = RewriteCodeOnce(newDoc, model, rootNode);

		string fixedFilePath = Path.ChangeExtension(tree.FilePath, ".Fix.cs");

		Assert.True(File.Exists(fixedFilePath), $"File '{fixedFilePath}' doesn't exist.");
		string fixedContent = await File.ReadAllTextAsync(fixedFilePath);

		Assert.AreEqual(fixedContent, result.ToFullString());
	}
	
	private static CompilationUnitSyntax RewriteCodeOnce(Document document, SemanticModel model, SyntaxNode rootNode) {
		MainRewriter rewriter = new(document, model);
		rewriter.Visit(rootNode);
		CompilationUnitSyntax? result = rewriter.RewriteNodes(rootNode) as CompilationUnitSyntax;

		Assert.NotNull(result);
		return rewriter.AddUsingDirectives(result);
	}

	private static async Task LoadProject(bool force = false) {
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

	private static IEnumerable<SyntaxTree> GetSyntaxTrees() {
		return GetSyntaxTreesAsync().ToEnumerable();
	}

	private static async IAsyncEnumerable<SyntaxTree> GetSyntaxTreesAsync() {
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
		foreach (SyntaxTree tree in GetSyntaxTrees()) {
			TestCaseData data = new TestCaseData(tree).SetName(Path.GetFileName(tree.FilePath));
			yield return data;
		}
	}
	
	private static void CopyFilesRecursively(string sourcePath, string targetPath)
	{
		//Now Create all of the directories
		foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
		{
			Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
		}

		//Copy all the files & Replaces any files with the same name
		foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
		{
			File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
		}
	}
}