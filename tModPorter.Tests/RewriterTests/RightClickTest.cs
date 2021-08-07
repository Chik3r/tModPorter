using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.MethodDeclarationRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests
{
	public class RightClickTest
	{
		[Fact]
		public void RewriteToken_ShouldMatchTarget()
		{
			// Get source code from .cs files
			string source = File.ReadAllText("TestData/RightClickTest/Single.cs");
			string target = File.ReadAllText("TestData/RightClickTest/Single.Fix.cs");

			// Create compilation for .cs source
			Utils.CreateCSharpCompilation(source, nameof(RightClickTest), out _, out CompilationUnitSyntax root, out SemanticModel model);

			// Create a rewriter
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokenDict = new();
			BaseRewriter rewriter = new RightClickRename(model, null, null, tokenDict);

			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				rewriter.VisitNode(assigmentNode);

			Assert.Single(tokenDict);

			root = root.RewriteMultipleNodes(null, tokenDict);
			Assert.Equal(target, root.ToFullString());
		}
	}
}