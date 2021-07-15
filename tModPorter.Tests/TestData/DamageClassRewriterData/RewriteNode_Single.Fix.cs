namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public bool melee;
    }
    
    public class RewriteNode_Single
    {
        void Method()
        {
            Item item = new Item();
            item.DamageType = DamageClass.Melee;
        }
    }
}