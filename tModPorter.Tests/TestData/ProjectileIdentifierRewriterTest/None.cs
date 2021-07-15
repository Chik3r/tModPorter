namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Projectile
    {
        public int field;
    }
    
    public class None
    {
        void MethodA()
        {
            Projectile projectile = new Projectile();
            projectile.field = 0;
        }

        void MethodB()
        {
            string projectile = "";
            projectile = " ";
        }
    }
}