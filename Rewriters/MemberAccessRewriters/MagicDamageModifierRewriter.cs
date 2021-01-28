using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class MagicDamageModifierRewriter : SimpleModifierRewriter
	{
		public MagicDamageModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Magic";
		public override string OldModifier => "magicDamage";
		public override ModifierType ModifierType => ModifierType.Damage;
	}

	class MagicCritModifierRewriter : SimpleModifierRewriter
	{
		public MagicCritModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Magic";
		public override string OldModifier => "magicCrit";
		public override ModifierType ModifierType => ModifierType.CritChance;
	}
}
