namespace tModPorter.Tests.TestData.FindTypeRewriterTest
{
    public class All
    {
        void Method()
        {
            int a = thing.Find<ModBuff>("BuffClass").Type;
            int b = Mod.Find<ModDust>("DustClass").Type;
            int c = thing.Find<ModItem>("ItemClass").Type;
            int d = Mod.Find<ModMountData>("MountClass").Type;
            int e = thing.Find<ModNPC>("NPCClass").Type;
            int f = Mod.Find<ModPrefix>("PrefixClass").Type;
            int g = thing.Find<ModProjectile>("ProjectileClass").Type;
            int h = Mod.Find<ModTileEntity>("TileEntityClass").Type;
            int i = thing.Find<ModTile>("TileClass").Type;
            int j = Mod.Find<ModWall>("WallClass").Type;
        }
    }
}