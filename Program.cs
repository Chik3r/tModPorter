using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using static System.Console;

namespace tModPorter
{
	class Program
	{
		//public static readonly Dictionary<Regex, string> RegexToRun = new Dictionary<Regex, string>
		//{
		//	{new Regex(@"Mod.Find<(.+)>(\("".+"".*?\){1,})"), @"Mod.Find<$1>$2.Type"},
		//	{new Regex(@"Mod.Find<(.+)>(\("".+"".*?\))\)\.Type;"), @"Mod.Find<$1>$2.Type);"},
		//	{new Regex(@"Mod.GetBackgroundSlot\(""(.+)""\)"), @"Mod.GetBackgroundSlot(""$1.rawimg"")"},
		//	{new Regex(@"TextureAssets\.(.+?\[.+?\])(?!\.Value)"), @"TextureAssets.$1.Value"},
		//	{new Regex(@"Mod.GetTexture(\(.+?\))"), @"Mod.GetTexture$1.Value"},
		//	{new Regex(@"ModContent.GetTexture(\(.+?\))"), @"ModContent.GetTexture$1.Value"},
		//	{new Regex(@"player\.MinionNPCTargetAim\(\);"), @"player.MinionNPCTargetAim(true);"}
		//};

		static async Task Main(string[] args)
		{
			// Attempt to set the version of MSBuild.
			var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
			var instance = visualStudioInstances.Length == 1
				// If there is only one instance of MSBuild on this machine, set that as the one to use.
				? visualStudioInstances[0]
				// Handle selecting the version of MSBuild you want to use.
				: SelectVisualStudioInstance(visualStudioInstances);

			WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

			// NOTE: Be sure to register an instance with the MSBuildLocator 
			//       before calling MSBuildWorkspace.Create()
			//       otherwise, MSBuildWorkspace won't MEF compose.
			MSBuildLocator.RegisterInstance(instance);

			using (var workspace = MSBuildWorkspace.Create())
			{
				// Print message for WorkspaceFailed event to help diagnosing project load failures.
				workspace.WorkspaceFailed += (o, e) => WriteLine(e.Diagnostic.Message);

				var solutionPath = args[0];
				WriteLine($"Loading solution '{solutionPath}'");

				// Attach progress reporter so we print projects as they are loaded.
				var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
				WriteLine($"Finished loading solution '{solutionPath}'");

				foreach (var document in solution.Projects
					.SelectMany(x => x.Documents))
				{
					var root = await document.GetSyntaxTreeAsync() ??
					           throw new Exception("No syntax root - " + document.FilePath);

					var rootNode = await root.GetRootAsync();

					var rewriter = new MainRewriter(await document.GetSemanticModelAsync());
					// Visit all the nodes to know what to change
					rewriter.Visit(rootNode);
					// Modify all nodes
					var result = rewriter.RewriteNodes(rootNode) as CompilationUnitSyntax;
					result = rewriter.AddUsings(result);

					if (!result.IsEquivalentTo(rootNode))
					{
						WriteLine("MODIFIED!!! -> " + document.FilePath);
						File.WriteAllText(document.FilePath,
							result.ToFullString());
					}
				}
			}
		}

		private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
		{
			WriteLine("Multiple installs of MSBuild detected please select one:");
			for (int i = 0; i < visualStudioInstances.Length; i++)
			{
				WriteLine($"Instance {i + 1}");
				WriteLine($"    Name: {visualStudioInstances[i].Name}");
				WriteLine($"    Version: {visualStudioInstances[i].Version}");
				WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
			}

			while (true)
			{
				var userResponse = ReadLine();
				if (int.TryParse(userResponse, out int instanceNumber) &&
					instanceNumber > 0 &&
					instanceNumber <= visualStudioInstances.Length)
				{
					return visualStudioInstances[instanceNumber - 1];
				}
				WriteLine("Input not accepted, try again.");
			}
		}

		private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
		{
			public void Report(ProjectLoadProgress loadProgress)
			{
				var projectDisplay = Path.GetFileName(loadProgress.FilePath);
				if (loadProgress.TargetFramework != null)
				{
					projectDisplay += $" ({loadProgress.TargetFramework})";
				}

				WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
			}
		}
	}
}
