using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class PlayerIdentifierRewriter : SimpleIdentifierRewriter
	{
		public PlayerIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string OldIdentifier => "player";
		public override string NewIdentifier => "Player";
	}
}
