using tModPorter.Tests.GenericClasses;

namespace tModPorter.Tests.TestData.DamageClassRewriterData
{
    public class RewriteNode_Multiple
    {
        void Method()
        {
            Item item = new Item();
            item.DamageType = DamageClass.Melee;
            item.DamageType = DamageClass.Magic;
            item.DamageType = DamageClass.Summon;
            item.DamageType = DamageClass.Ranged;
            item.DamageType = DamageClass.Throwing;
        }
    }
}