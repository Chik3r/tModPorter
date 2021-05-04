using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters
{
	static class RewriterUtilities
	{
		public static T WithExtraTrivia<T>(this T nodeToAddTrivia, SyntaxNode nodeWithOldTrivia) where T : SyntaxNode
		{
			return nodeToAddTrivia.WithLeadingTrivia(nodeWithOldTrivia.GetLeadingTrivia())
				.WithTrailingTrivia(nodeWithOldTrivia.GetTrailingTrivia());
		}
	}
}
