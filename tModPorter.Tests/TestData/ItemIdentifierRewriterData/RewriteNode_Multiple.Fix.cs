using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.SimpleIdentifierRewriterData
{
    public class ItemIdentifier_Multiple
    {
        void Method(Item item) { }

        void Method()
        {
            Item.field = 0;
            Item.otherField = 0;
        }
    }
}