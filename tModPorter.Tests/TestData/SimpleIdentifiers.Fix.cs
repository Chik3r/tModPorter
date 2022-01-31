using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class SimpleIdentifiers : InheritableMod
{
    public void MethodA()
    {
        Projectile.FieldA = 1;
        Mod.FieldA = 1;
        Player.FieldA = 1;
        Item.FieldA = 1;
    }
}