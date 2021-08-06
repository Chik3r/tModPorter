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
		override void SetStaticDefaults() { }
	}
	
	public class SimpleB : ModMount
	{
		override void SetStaticDefaults() { }
	}
}