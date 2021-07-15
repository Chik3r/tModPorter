namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public bool magic;
    }
    
    public class VisitNode_Single
    {
        private void Method()
        {
            Item item = new();
            item.magic = true;
        }
    }
}