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
            // item.magic = false;
            item.width = 0;
        }
    }
}