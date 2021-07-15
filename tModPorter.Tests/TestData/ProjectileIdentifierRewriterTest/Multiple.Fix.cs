namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Projectile { }
    
    public class Multiple
    {
        void MethodA(Projectile projectile) { }

        void MethodB()
        {
            Projectile.field = 0;
            Projectile.otherField = 0;
        }

        void MethodC()
        {
            int projectile = 0;
            projectile = 0;
        }

        void MethodD()
        {
            projectile = "";
            Projectile.field = "";
        }
    }
}