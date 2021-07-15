namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public bool magic;
        public bool melee;
        public bool ranged;
        public bool summon;
        public bool thrown;
    }
    
    public class RewriteNode_Multiple
    {
        void Method()
        {
            Item item = new Item();
            item.melee = true;
            item.magic = true;
            item.summon = true;
            item.ranged = true;
            item.thrown = true;
        }
    }
}