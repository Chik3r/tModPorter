namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public int width;
        public bool magic;
        public bool melee;
        public bool ranged;
        public bool summon;
        public bool thrown;
    }
    
    public class VisitNode_Multiple
    {
        private void Method()
        {
            Item item = new();
            item.width = 0;
            item.magic = true;
            item.melee = true;
            item.ranged = true;
            item.summon = false;
            item.thrown = false;
        }
    }
}