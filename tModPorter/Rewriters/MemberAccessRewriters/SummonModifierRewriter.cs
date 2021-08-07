using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	public class SummonDamageModifierRewriter : SimpleModifierRewriter
	{
		public SummonDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Summon";
		protected override string OldModifier => "minionDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}
}