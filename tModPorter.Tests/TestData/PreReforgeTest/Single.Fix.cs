namespace tModPorter.Tests.TestData.PreReforgeTest
{
	public class GlobalItem
	{
		virtual void PreReforge() { }
	}
	
	public class Single : GlobalItem
	{
		override void PreReforge() { }
	}
}