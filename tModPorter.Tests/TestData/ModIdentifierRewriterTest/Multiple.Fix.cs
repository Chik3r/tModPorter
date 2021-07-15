namespace tModPorter.Tests.TestData.ItemIdentifierRewriterTest
{
    class Mod { }
    
    public class Multiple
    {
        void MethodA(Mod mod) { }

        void MethodB()
        {
            Mod.field = 0;
            Mod.otherField = 0;
        }

        void MethodC()
        {
            int mod = 0;
            mod = 0;
        }

        void MethodD()
        {
            mod = "";
            Mod.field = "";
        }
    }
}