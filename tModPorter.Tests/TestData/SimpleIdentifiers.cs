using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class SimpleIdentifiers : InheritableMod
{
    public void MethodA()
    {
        projectile.FieldA = 1;
        mod.FieldA = 1;
        player.FieldA = 1;
        item.FieldA = 1;
    }
}