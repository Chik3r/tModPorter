namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Mod
    {
        public int field;
    }
    
    public class None
    {
        void MethodA()
        {
            Mod mod = new Mod();
            mod.field = 0;
        }

        void MethodB()
        {
            string mod = "";
            mod = " ";
        }
    }
}