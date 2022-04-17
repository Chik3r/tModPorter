using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class AddRecipes : ModItem {
	public override void AddRecipes() {
		CreateRecipe(1)
			.AddIngredient(ItemID.Wood, 10)
			.AddTile(TileID.WorkBenches)
			.Register();

		Method();
	}
	
	public void Method() { }
}