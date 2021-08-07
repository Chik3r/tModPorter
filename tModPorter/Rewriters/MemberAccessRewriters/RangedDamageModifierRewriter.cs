using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	public class RangedDamageModifierRewriter : SimpleModifierRewriter
	{
		public RangedDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Ranged";
		protected override string OldModifier => "rangedDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	public class RangedCritModifierRewriter : SimpleModifierRewriter
	{
		public RangedCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Ranged";
		protected override string OldModifier => "rangedCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}