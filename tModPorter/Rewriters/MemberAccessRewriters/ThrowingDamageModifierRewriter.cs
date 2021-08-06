using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters {
	public class ThrowingDamageModifierRewriter : SimpleModifierRewriter {
		public ThrowingDamageModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Throwing";
		protected override string OldModifier => "thrownDamage";
		protected override ModifierType ModifierType => ModifierType.Damage;
	}

	public class ThrowingCritModifierRewriter : SimpleModifierRewriter {
		public ThrowingCritModifierRewriter(SemanticModel model, List<string> usingList,
			HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodesToRewrite,
			HashSet<(BaseRewriter rewriter, SyntaxToken originalToken)> tokensToRewrite)
			: base(model, usingList, nodesToRewrite, tokensToRewrite)
		{ }

		protected override string NewModifier => "DamageClass.Throwing";
		protected override string OldModifier => "thrownCrit";
		protected override ModifierType ModifierType => ModifierType.CritChance;
	}
}