namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    class Item
    {
        public int width;
    }
    
    public class VisitNode_None
    {
        private void Method()
        {
            Item item = new();
            item.width = 0;
        }
    }
}