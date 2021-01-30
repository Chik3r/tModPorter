using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class SummonDamageModifierRewriter : SimpleModifierRewriter
	{
		public SummonDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Summon";
		protected override string OldModifier => "minionDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}
}
