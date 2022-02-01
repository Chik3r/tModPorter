using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class DamageClassTest : InheritableMod
{
    public void MethodA()
    {
        Item item = new();
        item.melee = true;
        item.magic = false;
        item.summon = true;
    }

    public void MethodB()
    {
        item.melee = true;

        int melee = 0;
        melee = 1;
    }
}