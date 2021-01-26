using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ModIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ModIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string OldIdentifier => "mod";
		public override string NewIdentifier => "Mod";
	}
}
