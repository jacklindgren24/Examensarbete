using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public enum Weapon { Projectile, Hitscan };
    public Weapon weapon = Weapon.Projectile;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [FMODUnity.EventRef]
    public string PlayerShoot;

    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        { // Fire.
            switch (weapon)
            {
                case Weapon.Projectile:
                    FMODUnity.RuntimeManager.PlayOneShot(PlayerShoot, transform.position);
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

        RaycastHit hit;
        Vector3 dir = bulletSpawn.transform.forward;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        { // Player is aiming at an object, get direction from bullet spawn to point of raycast hit.
            dir = hit.point - bulletSpawn.transform.position;
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = dir.normalized * ProjectileWeapon.velocity;
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
