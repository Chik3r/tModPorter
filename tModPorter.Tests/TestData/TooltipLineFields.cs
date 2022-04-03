using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData; 

public class TooltipLineFields {
	void A() {
		TooltipLine line = new TooltipLine();
		line.text = "";
		line.isModifier = true;
		line.isModifierBad = false;
		line.overrideColor = 0;
	}
}