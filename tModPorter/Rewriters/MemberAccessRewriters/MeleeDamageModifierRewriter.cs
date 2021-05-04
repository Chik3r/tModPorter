using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class MeleeDamageModifierRewriter : SimpleModifierRewriter
	{
		public MeleeDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Melee";
		protected override string OldModifier => "meleeDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	class MeleeCritModifierRewriter : SimpleModifierRewriter
	{
		public MeleeCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Melee";
		protected override string OldModifier => "meleeCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}
