﻿using Terraria;
using Terraria.ModLoader;

namespace tModPorter.Tests.TestData;

public class SimpleIdentifiersTest : Mod
{
    public void MethodA()
    {
        Projectile.FieldA = 1;
        Mod.FieldA = 1;
        Player.FieldA = 1;
        Item.FieldA = 1;
    }

    public void MethodB()
    {
        int item, mod, player, projectile;
        item = 1;
        mod = 2;
        player = 3;
        projectile = 4;
    }

    public void MethodC()
    {
        Dummy item = new();
        item.FieldA = 0;
    }
}