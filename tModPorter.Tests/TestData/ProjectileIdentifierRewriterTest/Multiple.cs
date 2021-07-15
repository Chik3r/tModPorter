namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Projectile { }
    
    public class Multiple
    {
        void MethodA(Projectile projectile) { }

        void MethodB()
        {
            projectile.field = 0;
            projectile.otherField = 0;
        }

        void MethodC()
        {
            int projectile = 0;
            projectile = 0;
        }

        void MethodD()
        {
            projectile = "";
            projectile.field = "";
        }
    }
}