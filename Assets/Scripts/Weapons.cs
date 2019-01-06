public static class ProjectileWeapon {

    public static int damage = 50;
    public static float cooldown = 0.4f;
    public static float velocity = 200;
    public static float lifetime = 2;

    public static string sound = "event:/Weapons/ProjectileShoot (2D)";
}

public static class HitscanWeapon {

    public static int damage = 34;
    public static float cooldown = 0.2f;
    public static float range = 1000;

    public static string sound = "event:/Weapons/HitscanShoot (2D)";
}

public static class MeleeWeapon {

    public static int damage = 70;
    public static float cooldown = 0.75f;
    public static float range = 4f;
    public static float delay = 0.1f;
    public static float knockback = 30;

    public static string sound = "event:/Weapons/Melee (2D)";
}