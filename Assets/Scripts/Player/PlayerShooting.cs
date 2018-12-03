using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public enum Weapon { Projectile, Hitscan };
    public Weapon weapon = Weapon.Projectile;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        { // Fire.
            switch (weapon)
            {
                case Weapon.Projectile:
                    if (timer >= ProjectileWeapon.cooldown) FireBullet();
                    break;
                case Weapon.Hitscan:
                    if (timer >= HitscanWeapon.cooldown) FireRay();
                    break;
            }
        }
        else if (Input.GetButtonDown("Switch"))
        { // Switch weapon.
            weapon = weapon == Weapon.Projectile ? Weapon.Hitscan : Weapon.Projectile;
            timer = 0;
        }
    }

    void FireBullet()
    {
        timer = 0;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * ProjectileWeapon.velocity;
        Destroy(bullet, ProjectileWeapon.lifetime);
    }

    void FireRay()
    {
        timer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, transform.forward, out hit, HitscanWeapon.range))
        { // Raycast forward from camera.
            if (hit.transform.tag == "Enemy")
            { // Ray hit an enemy, hurt enemy.
                hit.transform.GetComponent<EnemyController>().Health -= HitscanWeapon.damage;
            }
        }
    }
}
