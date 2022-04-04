using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class DangersenseTile : ModTile {
	public override bool Dangersense(int a) {
		return false;
	}
}

public class DangersenseGlobal : GlobalTile {
	public override bool Dangersense(int a) {
		return false;
	}
}