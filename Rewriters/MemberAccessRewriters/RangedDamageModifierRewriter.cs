using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class RangedDamageModifierRewriter : SimpleModifierRewriter
	{
		public RangedDamageModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Ranged";
		public override string OldModifier => "rangedDamage";
		public override ModifierType ModifierType => ModifierType.Damage;
	}

	class RangedCritModifierRewriter : SimpleModifierRewriter
	{
		public RangedCritModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Ranged";
		public override string OldModifier => "rangedCrit";
		public override ModifierType ModifierType => ModifierType.CritChance;
	}
}
