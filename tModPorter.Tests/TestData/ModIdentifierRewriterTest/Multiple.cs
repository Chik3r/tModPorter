namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Mod { }
    
    public class Multiple
    {
        void MethodA(Mod mod) { }

        void MethodB()
        {
            mod.field = 0;
            mod.otherField = 0;
        }

        void MethodC()
        {
            int mod = 0;
            mod = 0;
        }

        void MethodD()
        {
            mod = "";
            mod.field = "";
        }
    }
}