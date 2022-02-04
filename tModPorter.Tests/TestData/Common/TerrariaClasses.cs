namespace Terraria;

public class Tile
{
    public ushort TileType;
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

public class Main
{
    public static Tile[,] tile;
}

// Works as well as the actual thing for testing
enum DamageClass
{
    Melee,
    Magic,
    Summon,
    Ranged,
    Throwing,
}