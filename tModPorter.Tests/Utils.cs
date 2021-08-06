using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;

namespace tModPorter.Tests {
	public static class Utils
	{
		public static CSharpCompilation CreateCSharpCompilation(string source, string assemblyName, out SyntaxTree tree,
			out CompilationUnitSyntax root, out SemanticModel model)
		{
			tree = CSharpSyntaxTree.ParseText(source);

			root = tree.GetCompilationUnitRoot();

			CSharpCompilation compilation = CSharpCompilation.Create(assemblyName)
				.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(MainRewriter).Assembly.Location))
				.AddSyntaxTrees(tree);

			model = compilation.GetSemanticModel(tree);

			return compilation;
		}

		internal static CompilationUnitSyntax RewriteMultipleNodes(this CompilationUnitSyntax root,
			IEnumerable<(BaseRewriter rewriter, SyntaxNode originalNode)>? nodesToRewrite = null,
			IEnumerable<(BaseRewriter rewriter, SyntaxToken originalToken)>? tokensToRewrite = null)
		{
			nodesToRewrite ??= Array.Empty<(BaseRewriter rewriter, SyntaxNode originalNode)>();
			Dictionary<SyntaxNode, SyntaxNode> nodeDictionary = new();
			foreach ((BaseRewriter rewriter, SyntaxNode originalNode) in nodesToRewrite)
			{
				SyntaxNode newNode = rewriter.RewriteNode(originalNode);
				nodeDictionary.Add(originalNode, newNode);
			}

			tokensToRewrite ??= Array.Empty<(BaseRewriter rewriter, SyntaxToken originalToken)>();
			Dictionary<SyntaxToken, SyntaxToken> tokenDictionary = new();
			foreach ((BaseRewriter rewriter, SyntaxToken originalToken) in tokensToRewrite)
			{
				SyntaxToken newToken = rewriter.RewriteToken(originalToken);
				tokenDictionary.Add(originalToken, newToken);
			}

			root = root.ReplaceSyntax(
				nodes: nodeDictionary.Keys.AsEnumerable(), (original, _) => nodeDictionary[original],
				tokens: Array.Empty<SyntaxToken>(), (original, _) => tokenDictionary[original],
				trivia: Array.Empty<SyntaxTrivia>(), (original, _) => original);
			return root;
		}
	}
}