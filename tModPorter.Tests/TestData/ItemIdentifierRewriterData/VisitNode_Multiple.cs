using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.SimpleIdentifierRewriterData
{
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