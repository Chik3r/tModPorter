namespace tModPorter.Tests.TestData.FindTypeRewriterTest
{
    public class FindTypeTest : InheritableMod
    {
        void Method()
        {
            int a = Mod.Find<ModBuff>("BuffClass").Type;
            int b = Mod.Find<ModDust>("DustClass").Type;
            int c = Mod.Find<ModItem>("ItemClass").Type;
            int d = Mod.Find<ModMountData>("MountClass").Type;
            int e = Mod.Find<ModNPC>("NPCClass").Type;
            int f = Mod.Find<ModPrefix>("PrefixClass").Type;
            int g = Mod.Find<ModProjectile>("ProjectileClass").Type;
            int h = Mod.Find<ModTileEntity>("TileEntityClass").Type;
            int i = Mod.Find<ModTile>("TileClass").Type;
            int j = Mod.Find<ModWall>("WallClass").Type;
        }
    }
}