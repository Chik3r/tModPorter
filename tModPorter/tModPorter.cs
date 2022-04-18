#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using tModPorter.Rewriters;
using UtfUnknown;
using static System.Console;

namespace tModPorter;

public class tModPorter {
	private int _totalDocuments;
	private int _processedDocuments;

	public async Task<Result> ProcessProject(string projectPath, int maxThreads = 2, IProgress<double>? progressReporter = null) {
		try {
			MSBuildLocator.RegisterDefaults();

			using MSBuildWorkspace workspace = MSBuildWorkspace.Create();

			// Print message for WorkspaceFailed event to help diagnosing project load failures.
			workspace.WorkspaceFailed += (o, e) => {
				// TODO: Don't ignore this
			};

			WriteLine($"Loading project: {projectPath}");
			// Attach progress reporter so we print projects as they are loaded.
			Project project = await workspace.OpenProjectAsync(projectPath, new ConsoleProgressReporter());
			_totalDocuments = project.Documents.Count();

			int numChunks = Math.Min(maxThreads, _totalDocuments);
			int i = 0;
			IEnumerable<IEnumerable<Document>> chunks =
				from document in project.Documents
				group document by i++ % numChunks
				into part
				select part.AsEnumerable();

			List<Task> tasks = chunks.Select(chunk => Task.Run(() => ProcessChunk(chunk, progressReporter))).ToList();

			await Task.WhenAll(tasks);

			return new Result(true, null);
		}
		catch (Exception e) {
			return new Result(false, e);
		}
	}

	private double UpdateProgress() {
		int processedDocs = Interlocked.Add(ref _processedDocuments, 1);
		return (double) processedDocs / _totalDocuments;
	}

	private async Task ProcessChunk(IEnumerable<Document> chunk, IProgress<double>? progress) {
		foreach (Document document in chunk)
			await ProcessFile(document, progress);
	}

	private async Task ProcessFile(Document document, IProgress<double>? progress) {
		SyntaxTree root = await document.GetSyntaxTreeAsync() ??
		                  throw new Exception("No syntax root - " + document.FilePath);

		SyntaxNode rootNode = await root.GetRootAsync();

		MainRewriter rewriter = new(document, await document.GetSemanticModelAsync());
		// Visit all the nodes to know what to change
		rewriter.Visit(rootNode);
		// Modify all nodes
		SyntaxNode result = rewriter.RewriteNodes(rootNode);
		if (result is not CompilationUnitSyntax unitSyntax) {
			throw new InvalidOperationException($"Rewritten node was not of type {nameof(CompilationUnitSyntax)}, source node's type is {rootNode.GetType()}");
		}
		result = rewriter.AddUsingDirectives(unitSyntax);

		if (!result.IsEquivalentTo(rootNode) && document.FilePath != null) {
			Encoding encoding;
			await using (Stream fs = new FileStream(document.FilePath, FileMode.Open, FileAccess.Read)) {
				DetectionResult detectionResult = CharsetDetector.DetectFromStream(fs);
				encoding = detectionResult.Detected.Encoding;
				if (detectionResult.Detected.Confidence < .95f)
					WriteLine($"Less than 95% confidence about the file encoding of: {document.FilePath}");
			}

			await File.WriteAllTextAsync(document.FilePath, result.ToFullString(), encoding);
		}

		progress?.Report(UpdateProgress());
	}
	
	private class ConsoleProgressReporter : IProgress<ProjectLoadProgress> {
		public void Report(ProjectLoadProgress loadProgress) {
			string? projectDisplay = Path.GetFileName(loadProgress.FilePath);
			if (loadProgress.TargetFramework != null) projectDisplay += $" ({loadProgress.TargetFramework})";

			WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
		}
	}
}