using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.MethodDeclarationRewriters;
using Xunit;

namespace tModPorter.Tests.RewriterTests
{
	public class SetStaticDefaultsTest
	{
		[Fact]
		public void RewriteToken_ShouldMatchTarget()
		{
			// Get source code from .cs files
			string source = File.ReadAllText("TestData/SetStaticDefaultsTest/Simple.cs");
			string target = File.ReadAllText("TestData/SetStaticDefaultsTest/Simple.Fix.cs");
			
			// Create compilation for .cs source
			Utils.CreateCSharpCompilation(source, nameof(SetStaticDefaultsTest), out _, out CompilationUnitSyntax root, out SemanticModel model);

			// Create a rewriter
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokenDict = new();
			BaseRewriter rewriter = new SetStaticDefaultsRename(model, null, null, tokenDict);
			
			foreach (SyntaxNode assigmentNode in root.DescendantNodes())
				rewriter.VisitNode(assigmentNode);
			
			Assert.Equal(2, tokenDict.Count);

			root = root.RewriteMultipleNodes(null, tokenDict);
			Assert.Equal(target, root.ToFullString());
		}
	}
}