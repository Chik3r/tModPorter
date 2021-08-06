namespace tModPorter.Tests.TestData.SetStaticDefaultsTest
{
	public class GlobalTile
	{
		virtual void SetStaticDefaults() { }
	}
	
	public class ModMount
	{
		virtual void SetStaticDefaults() { }
	}
	
	public class SimpleA : GlobalTile
	{
		override void SetDefaults() { }
	}
	
	public class SimpleB : ModMount
	{
		override void SetDefaults() { }
	}
}