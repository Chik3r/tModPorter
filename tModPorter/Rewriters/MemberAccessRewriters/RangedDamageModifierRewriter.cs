using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class RangedDamageModifierRewriter : SimpleModifierRewriter
	{
		public RangedDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Ranged";
		protected override string OldModifier => "rangedDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	class RangedCritModifierRewriter : SimpleModifierRewriter
	{
		public RangedCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Ranged";
		protected override string OldModifier => "rangedCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}
