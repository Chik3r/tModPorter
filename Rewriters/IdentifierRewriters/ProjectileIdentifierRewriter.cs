using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.IdentifierRewriters
{
	class ProjectileIdentifierRewriter : SimpleIdentifierRewriter
	{
		public ProjectileIdentifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string OldIdentifier => "projectile";
		public override string NewIdentifier => "Projectile";
	}
}
