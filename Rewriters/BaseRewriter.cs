using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters
{
	abstract class BaseRewriter
	{
		public RewriterType RewriterType => RewriterType.None;
		private SemanticModel _model;
		private List<string> _usingList;

		public BaseRewriter(SemanticModel model, List<string> UsingList)
		{
			_model = model;
			_usingList = UsingList;
		}

		public virtual SyntaxNode VisitNode(SyntaxNode node) => node;

		protected void AddUsing(string newUsing)
		{
			if (!_usingList.Contains(newUsing.Trim()))
				_usingList.Add(newUsing.Trim());
		}
	}
}
