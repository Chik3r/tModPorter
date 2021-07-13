using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    public class RewriteNode_Advanced
    {
        void Method()
        {
            Item item = new Item();
            item.DamageType = DamageClass.Melee;
            // item.magic = false;
            item.width = 0;
        }
    }
}