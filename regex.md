﻿- Find: `Mod.Find<(.+)>(\(".+".*?\){1,})`
- Replace: `Mod.Find<$1>$2.Type`
---
- Find: `Mod.GetBackgroundSlot\("(.+)"\)`
- Replace: `Mod.GetBackgroundSlot("$1.rawimg")`
---
- 