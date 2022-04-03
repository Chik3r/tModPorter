using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class TileTest {
	void A() {
		Tile tile = new Tile();

		if (tile.active()) {
			tile.type = 0;
			tile.wall = 0;
		}

		if (tile.nactive()) {
			tile.frameX = 0;
			tile.frameY = 0;
			tile.wallFrameX = 0;
			tile.wallFrameY = 0;
		}
	}
}