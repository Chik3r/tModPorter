namespace Terraria;

public class Tile
{
    public ushort ItemType;
}

public class Projectile
{
    public int FieldA { get; set; }
}

public class Player { }

public class Item
{
    public DamageClass DamageType;
    public int type;
}

public class NPC { }

// Works as well as the actual thing for testing
enum DamageClass
{
    Melee,
    Magic,
    Summon,
    Ranged,
    Throwing,
}