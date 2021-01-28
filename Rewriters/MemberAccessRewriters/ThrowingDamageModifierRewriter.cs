using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class ThrowingDamageModifierRewriter : SimpleModifierRewriter
	{
		public ThrowingDamageModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Throwing";
		public override string OldModifier => "thrownDamage";
		public override ModifierType ModifierType => ModifierType.Damage;
	}

	class ThrowingCritModifierRewriter : SimpleModifierRewriter
	{
		public ThrowingCritModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Throwing";
		public override string OldModifier => "thrownCrit";
		public override ModifierType ModifierType => ModifierType.CritChance;
	}
}
