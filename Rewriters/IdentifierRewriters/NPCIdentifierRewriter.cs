using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class NPCIdentifierRewriter : SimpleIdentifierRewriter
	{
		public NPCIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string OldIdentifier => "npc";
		public override string NewIdentifier => "NPC";
	}
}
