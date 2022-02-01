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

public abstract class ModItem
{
    public virtual void NetReceive { }
    public virtual bool? UseItem() { }
}

public abstract class GlobalItem
{
    public virtual void PreReforge() { }
}

public abstract class ModTile
{
    public virtual void RightClick() { }
}

public abstract class GlobalTile
{
    public virtual void SetStaticDefaults() { }
}

public abstract class ModMount
{
    public virtual void SetStaticDefaults() { }
}