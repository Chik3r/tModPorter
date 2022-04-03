using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class TooltipLineFields {
	void A() {
		TooltipLine line = new TooltipLine();
		line.Text = "";
		line.IsModifier = true;
		line.IsModifierBad = false;
		line.OverrideColor = 0;
	}
}