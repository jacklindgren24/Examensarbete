﻿public static class ProjectileWeapon
{
    public static int damage = 50;
    public static float cooldown = 0.5f;
    public static float velocity = 100;
    public static float lifetime = 2;

    public static string sound = "event:/Weapons/ProjectileShoot (2D)";
}

public static class HitscanWeapon
{
    public static int damage = 34;
    public static float cooldown = 0.3f;
    public static float range = 1000;

    public static string sound = "event:/Weapons/HitscanShoot (2D)";
}