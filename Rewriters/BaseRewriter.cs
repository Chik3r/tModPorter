using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tModPorter.Rewriters
{
	abstract class BaseRewriter
	{
		public virtual RewriterType RewriterType => RewriterType.None;
		internal SemanticModel _model;
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

		protected bool HasSymbol(SyntaxNode node, out ISymbol symbol)
		{
			// Try to get the symbol
			try
			{
				symbol = _model.GetSymbolInfo(node).Symbol;
				return symbol != null;
			}
			catch
			{
				// If it throws an error, hope it was because the node was changed and return true
				// TODO: Find a way to get the symbol even after changing the node
				Console.WriteLine("Symbol not found on node: " + node);
				symbol = null;
				return true;
			}
		}
	}
}
