using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	public class MagicDamageModifierRewriter : SimpleModifierRewriter
	{
		public MagicDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Magic";
		protected override string OldModifier => "magicDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	public class MagicCritModifierRewriter : SimpleModifierRewriter
	{
		public MagicCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Magic";
		protected override string OldModifier => "magicCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}