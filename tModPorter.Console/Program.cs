using static System.Console;

namespace tModPorter.Console;

internal class Program {
	public static async Task Main(string[] args) {
		string projectPath = GetProjectPath(args);

		ProgressBar bar = new();

		tModPorter porter = new();
		Result result = await porter.ProcessProject(projectPath, 4, bar);

		bar.ForceUpdate();
		WriteLine();

		if (result.Success) {
			WriteLine("Successfully ported the project");
			return;
		}
		
		WriteLine("Failed to port the project with the following error:");
		WriteLine(result.ErrorCause);
	}
	
	private static string GetProjectPath(string[] args) {
		// Check if the args have a valid file path
		if (args.Length > 0 && File.Exists(Path.ChangeExtension(args[0], ".csproj")))
			return args[0];

		// Ask the user for a path
		WriteLine("Enter the path to the .csproj of the mod you want to port");
		string filePath = Path.ChangeExtension(ReadLine(), ".csproj")!;

		// Continue asking until a valid file is passed
		while (!File.Exists(filePath)) {
			Clear();
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine("The path you entered doesn't exist");
			ForegroundColor = ConsoleColor.Gray;

			filePath = Path.ChangeExtension(ReadLine(), ".csproj")!;
		}

		// Reset the console
		ForegroundColor = ConsoleColor.Gray;
		Clear();

		// Return the path passed in by the user
		return filePath;
	}
}