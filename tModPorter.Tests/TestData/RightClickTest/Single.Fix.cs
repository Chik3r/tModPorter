namespace tModPorter.Tests.TestData.RightClickTest
{
	public class ModTile
	{
		virtual void RightClick() { }
	}
	
	public class Single : ModTile
	{
		override void RightClick() { }
	}
}