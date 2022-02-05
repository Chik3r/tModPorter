using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class DamageModifiers : Mod
{
    public void MethodA()
    {
        item.magicDamage += 2;
        item.meleeCrit *= 5;
        item.minionDamage = 8;
    }
}