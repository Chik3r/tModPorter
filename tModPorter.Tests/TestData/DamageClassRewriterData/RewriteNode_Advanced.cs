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
            item.melee = true;
            item.magic = false;
            item.width = 0;
        }
    }
}