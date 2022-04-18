using Terraria;

namespace Terraria.ModLoader;

public abstract class Mod
{
    public Player Player { get; set; }
    public Item Item { get; set; }
    public Projectile Projectile { get; set; }
    public NPC NPC { get; set; }
}

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
    
    public virtual bool IsTileDangerous(int i) { return false; }
}

public abstract class GlobalTile
{
    public virtual void SetStaticDefaults() { }
    
    public virtual bool? IsTileDangerous(int i) { return false; }
}

public abstract class ModMount
{
    public virtual void SetStaticDefaults() { }
}

public class TooltipLine {
	public string Mod;
	public string Name;
	public string Text;
	public bool IsModifier;
	public int OverrideColor;
}

public enum DamageClass {
	Magic, Melee, Ranged, Summon, Generic, SummonMeleeSpeed,
}