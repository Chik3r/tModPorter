using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class TileTest {
	void A() {
		Tile tile = new Tile();

		if (tile.HasTile) {
			tile.TileType = 0;
			tile.WallType = 0;
		}

		if (tile.HasUnactuatedTile) {
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
			tile.WallFrameX = 0;
			tile.WallFrameY = 0;
		}
	}
}