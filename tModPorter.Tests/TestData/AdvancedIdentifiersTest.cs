using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class AdvancedIdentifiersTest : Mod
{
    public void MethodA()
    {
        item.type = 1;
        Main.tile[0, 0].type = 0;
    }
}