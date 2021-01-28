using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

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

		/// <summary>
		/// Override this method to modify a node. The type of node modified depends on <see cref="RewriterType"/>
		/// </summary>
		/// <param name="node">The original node</param>
		/// <param name="finalNode">The modified node</param>
		/// <returns>Return <see langword="true" /> if 'base.VisitX()' should be called, return false if not</returns>
		public virtual bool VisitNode(SyntaxNode node, out SyntaxNode finalNode)
		{
			finalNode = node;
			return true;
		}

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
				// If it throws an error, hope it was because the node was changed and return false
				// TODO: Find a way to get the symbol even after changing the node
				Console.WriteLine("Symbol not found on node: " + node);
				symbol = null;
				return false;
			}
		}
	}
}
