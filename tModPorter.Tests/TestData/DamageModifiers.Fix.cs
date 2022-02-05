using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class DamageModifiers : Mod
{
    public void MethodA()
    {
        Item.GetDamage(DamageClass.Magic) += 2;
        Item.GetCritChance(DamageClass.Melee) *= 5;
        Item.GetDamage(DamageClass.Summon) = 8;
    }
}