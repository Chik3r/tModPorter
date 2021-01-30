using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class MagicDamageModifierRewriter : SimpleModifierRewriter
	{
		public MagicDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Magic";
		protected override string OldModifier => "magicDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	class MagicCritModifierRewriter : SimpleModifierRewriter
	{
		public MagicCritModifierRewriter(SemanticModel model, List<string> usingList, 
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite) : base(model, usingList, nodesToRewrite) { }

		protected override string NewModifier => "DamageClass.Magic";
		protected override string OldModifier => "magicCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}
