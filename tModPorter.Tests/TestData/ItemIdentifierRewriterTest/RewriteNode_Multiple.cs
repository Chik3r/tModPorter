namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Item
    {
        public int field;
        public int otherField;
    }
    
    public class ItemIdentifier_Multiple
    {
        void Method(Item item) { }

        void Method()
        {
            item.field = 0;
            item.otherField = 0;
        }
    }
}