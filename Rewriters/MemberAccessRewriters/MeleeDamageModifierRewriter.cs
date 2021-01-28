using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class MeleeDamageModifierRewriter : SimpleModifierRewriter
	{
		public MeleeDamageModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Melee";
		public override string OldModifier => "meleeDamage";
		public override ModifierType ModifierType => ModifierType.Damage;
	}

	class MeleeCritModifierRewriter : SimpleModifierRewriter
	{
		public MeleeCritModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Melee";
		public override string OldModifier => "meleeCrit";
		public override ModifierType ModifierType => ModifierType.CritChance;
	}
}
