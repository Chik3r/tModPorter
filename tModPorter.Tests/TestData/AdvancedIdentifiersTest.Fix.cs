using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class AdvancedIdentifiersTest : Mod
{
    public void MethodA()
    {
        Item.type = 1;
        Main.tile[0, 0].TileType = 0;
    }
}