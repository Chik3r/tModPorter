using Terraria;

namespace Terraria.ModLoader;

public abstract class InheritableMod
{
    public Player Player { get; set; }
    public Item Item { get; set; }
    public Projectile Projectile { get; set; }
    public NPC NPC { get; set; }
}

public class Mod { }