using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	public class MeleeDamageModifierRewriter : SimpleModifierRewriter
	{
		public MeleeDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Melee";
		protected override string OldModifier => "meleeDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	public class MeleeCritModifierRewriter : SimpleModifierRewriter
	{
		public MeleeCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Melee";
		protected override string OldModifier => "meleeCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}