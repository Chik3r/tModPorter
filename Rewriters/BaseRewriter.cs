using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters
{
	abstract class BaseRewriter
	{
		public virtual SyntaxNode VisitNode(SyntaxNode node, SemanticModel model) => node;
	}
}
