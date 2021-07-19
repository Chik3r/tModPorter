namespace tModPorter.Tests.TestData.FindTypeRewriterTest
{
    public class None
    {
        void Method()
        {
            int a = thing.BuffType<BuffClass>();
            int b = thing.DustType<DustClass>();
            int c = thing.ItemType<ItemClass>();
            int d = thing.MountType<MountClass>();
            int e = thing.NPCType<NPCClass>();
            int f = thing.PrefixType<PrefixClass>();
            int g = thing.ProjectileType<ProjectileClass>();
            int h = thing.TileEntityType<TileEntityClass>();
            int i = thing.TileType<TileClass>();
            int j = thing.WallType<WallClass>();

            int k = MethodB();
        }
    }
}