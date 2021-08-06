namespace tModPorter.Tests.TestData.NetReceiveTest
{
	public class ModItem
	{
		virtual void NetReceive() { }
	}
	
	public class Single : ModItem
	{
		override void NetRecieve() { }
	}
}