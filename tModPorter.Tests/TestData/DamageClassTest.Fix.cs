using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class DamageClassTest : InheritableMod
{
    public void MethodA()
    {
        Item item = new();
        item.DamageType = DamageClass.Melee;
        // item.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
        item.DamageType = DamageClass.Summon;
    }

    public void MethodB()
    {
        Item.DamageType = DamageClass.Melee;
    }
}