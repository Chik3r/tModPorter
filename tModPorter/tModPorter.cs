using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using tModPorter.Rewriters;
using UtfUnknown;
using static System.Console;

namespace tModPorter
{
	class tModPorter
	{
		static async Task Main(string[] args)
		{
			MSBuildLocator.RegisterDefaults();

			using MSBuildWorkspace workspace = MSBuildWorkspace.Create();

			// Print message for WorkspaceFailed event to help diagnosing project load failures.
			workspace.WorkspaceFailed += (o, e) => {
				if (e.Diagnostic.Message.Contains(@"Steamworks.NET.dll"", ""AMD64"""))
					return; 

				if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Warning)
					ForegroundColor = ConsoleColor.Yellow;
				else				
					ForegroundColor = ConsoleColor.Red;

				WriteLine(e.Diagnostic.Message);
				ForegroundColor = ConsoleColor.Gray;

				if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
					ReadKey();
			};

			string projectPath = GetProjectPath(args);
			WriteLine($"Loading solution '{projectPath}'");

			// Attach progress reporter so we print projects as they are loaded.
			Project project = await workspace.OpenProjectAsync(projectPath, new ConsoleProgressReporter());
			int documentCount = project.Documents.Count();
			WriteLine($"Finished loading solution '{projectPath}'");

			ProgressBar bar = ProgressBar.StartNew(documentCount);

			byte chunkSize = (byte) Math.Min(4, documentCount);
			int i = 0;
			IEnumerable<IEnumerable<Document>> chunks = from document in project.Documents
				group document by i++ % chunkSize
				into part
				select part.AsEnumerable();

			List<Task> tasks = chunks.Select(chunk => Task.Run(() => ProcessChunk(chunk, bar))).ToList();

			await Task.WhenAll(tasks);
		}

		private static async Task ProcessChunk(IEnumerable<Document> chunk, IProgress<int> progress)
		{
			foreach (Document document in chunk)
				await ProcessFile(document, progress);
		}

		private static async Task ProcessFile(Document document, IProgress<int> progress)
		{
			SyntaxTree root = await document.GetSyntaxTreeAsync() ??
			                  throw new Exception("No syntax root - " + document.FilePath);

			SyntaxNode rootNode = await root.GetRootAsync();

			MainRewriter rewriter = new(await document.GetSemanticModelAsync());
			// Visit all the nodes to know what to change
			rewriter.Visit(rootNode);
			// Modify all nodes
			CompilationUnitSyntax result = rewriter.RewriteNodes(rootNode) as CompilationUnitSyntax;
			result = rewriter.AddUsingDirectives(result);

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

			progress.Report(1);
		}

		private static string GetProjectPath(string[] args)
		{
			// Check if the args have a valid file path
			if (args.Length > 0 && File.Exists(Path.ChangeExtension(args[0], ".csproj")))
				return args[0];

			// Ask the user for a path
			WriteLine("Enter the path to the .csproj of the mod you want to port");
			string filePath = Path.ChangeExtension(ReadLine(), ".csproj");

			// Continue asking until a valid file is passed
			while (!File.Exists(filePath)) {
				Clear();
				ForegroundColor = ConsoleColor.Yellow;
				WriteLine("The path you entered doesn't exist");
				ForegroundColor = ConsoleColor.Gray;

				filePath = Path.ChangeExtension(ReadLine(), ".csproj");
			}

			// Reset the console
			ForegroundColor = ConsoleColor.Gray;
			Clear();

			// Return the path passed in by the user
			return filePath;
		}

		private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
		{
			public void Report(ProjectLoadProgress loadProgress)
			{
				string? projectDisplay = Path.GetFileName(loadProgress.FilePath);
				if (loadProgress.TargetFramework != null) {
					projectDisplay += $" ({loadProgress.TargetFramework})";
				}

				WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
			}
		}
	}
}