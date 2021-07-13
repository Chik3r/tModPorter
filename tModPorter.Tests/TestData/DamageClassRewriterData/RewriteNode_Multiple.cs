using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    public class RewriteNode_Multiple
    {
        void Method()
        {
            Item item = new Item();
            item.melee = true;
            item.magic = true;
            item.summon = true;
            item.ranged = true;
            item.thrown = true;
        }
    }
}