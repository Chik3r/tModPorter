using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters
{
	abstract class BaseRewriter
	{
		public virtual RewriterType RewriterType => RewriterType.None;
		protected SemanticModel _model;
		private List<string> _usingList;
		private HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> _nodesToRewrite;

		public BaseRewriter(SemanticModel model, List<string> usingList, HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite)
		{
			_model = model;
			_usingList = usingList;
			_nodesToRewrite = nodesToRewrite;
		}

		/// <summary>
		/// Override this method to queue a modification to a node. The type of node depends on <see cref="RewriterType"/>
		/// </summary>
		/// <param name="node">The original node</param>
		/// <returns>Return <see cref="node"/></returns>
		public virtual void VisitNode(SyntaxNode node) { }

		/// <summary>
		/// Override this method to rewrite a node added using <see cref="AddNodeToRewrite"/>
		/// </summary>
		/// <param name="node">The node to rewrite</param>
		/// <returns>The rewritten node</returns>
		public virtual SyntaxNode RewriteNode(SyntaxNode node) => node;

		protected void AddUsing(string newUsing)
		{
			if (!_usingList.Contains(newUsing.Trim()))
				_usingList.Add(newUsing.Trim());
		}

		protected void AddNodeToRewrite(SyntaxNode node) => _nodesToRewrite.Add((this, node));

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
				// This should never be reached, if it is reached, something went horribly wrong
				Console.WriteLine("Symbol not found on node: " + node);
				symbol = null;
				return false;
			}
		}
	}
}
