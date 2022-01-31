namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public bool melee;
        public bool magic;
        public int width;
    }
    
    public class RewriteNode_Advanced
    {
        void Method()
        {
            Item item = new Item();
            item.DamageType = DamageClass.Melee;
            // item.magic = false // tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes;
            item.width = 0;
        }
    }
}