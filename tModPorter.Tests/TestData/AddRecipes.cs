using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class AddRecipes : ModItem {
	public override void AddRecipes() {
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(ItemID.Wood, 10);
		recipe.AddTile(TileID.WorkBenches);
		recipe.SetResult(this);
		recipe.AddRecipe();

		Method();
	}
	
	public void Method() { }
}