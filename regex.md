- Find: `Mod.Find<(.+)>(\(".+".*?\){1,})`
- Replace: `Mod.Find<$1>$2.Type`

This might have left some errors, so also run this:
- Find: `Mod.Find<(.+)>(\(".+".*?\))\)\.Type;`
- Replace: `Mod.Find<$1>$2.Type);`
---
- Find: `Mod.GetBackgroundSlot\("(.+)"\)`
- Replace: `Mod.GetBackgroundSlot("$1.rawimg")`
---
- Find: `TextureAssets\.(.+?\[.+?\])(?!\.Value)`
- Replace: `TextureAssets.$1.Value`
---
- Find: `Mod.GetTexture(\(.+?\))`
- Replace: `Mod.GetTexture$1.Value`