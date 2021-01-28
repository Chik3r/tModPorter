using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace tModPorter.Rewriters.MemberAccessRewriters
{
	class SummonDamageModifierRewriter : SimpleModifierRewriter
	{
		public SummonDamageModifierRewriter(SemanticModel model, List<string> UsingList) : base(model, UsingList) { }

		public override string NewModifier => "DamageClass.Summon";
		public override string OldModifier => "minionDamage";
		public override ModifierType ModifierType => ModifierType.Damage;
	}
}
