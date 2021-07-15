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
            item.DamageType = DamageClass.Melee;
            item.DamageType = DamageClass.Magic;
            item.DamageType = DamageClass.Summon;
            item.DamageType = DamageClass.Ranged;
            item.DamageType = DamageClass.Throwing;
        }
    }
}