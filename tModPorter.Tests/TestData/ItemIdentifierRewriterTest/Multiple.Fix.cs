namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Item
    {
        public int field;
    }
    
    public class ItemIdentifier_Multiple
    {
        void MethodA(Item item) { }

        void MethodB()
        {
            Item.field = 0;
            Item.otherField = 0;
        }

        void MethodC()
        {
            int item = 0;
            item = 0;
        }

        void MethodD()
        {
            item = "";
            Item.field = "";
        }
    }
}