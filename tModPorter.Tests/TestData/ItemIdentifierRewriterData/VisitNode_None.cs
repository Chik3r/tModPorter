using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.SimpleIdentifierRewriterData
{
    public class ItemIdentifier_Single
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