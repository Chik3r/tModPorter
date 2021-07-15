namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Item
    {
        public int field;
    }
    
    public class ItemIdentifier_None
    {
        void MethodA()
        {
            Item item = new Item();
            item.field = 0;
        }

        void MethodB()
        {
            string item = "";
            item = " ";
        }
    }
}