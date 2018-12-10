using UnityEngine;
using FMODUnity;

public class PlayerShooting : MonoBehaviour {

    public enum Weapon { Projectile, Hitscan };
    public Weapon weapon = Weapon.Projectile;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    float timer = 0;
    float[] intensities = new float[] { 100, 100 };

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        { // Increase intensity by 50 up to a maximum of 100.
            intensities[(int)weapon] = Mathf.Clamp(intensities[(int)weapon] + 50, 0, 100);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        { // Decrease intensity by 50 down to a minimum of 0.
            intensities[(int)weapon] = Mathf.Clamp(intensities[(int)weapon] - 50, 0, 100);
        }
    }

    void FireBullet()
    {
        timer = 0;

        RaycastHit hit;
        Vector3 dir = bulletSpawn.transform.forward;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
        { // Player is aiming at an object, get direction from bullet spawn to point of raycast hit.
            dir = hit.point - bulletSpawn.transform.position;
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = dir.normalized * ProjectileWeapon.velocity;
        Destroy(bullet, ProjectileWeapon.lifetime);

        RuntimeManager.PlayOneShot(ProjectileWeapon.sound, "Intensity", intensities[(int)Weapon.Projectile], transform.position);
    }

    void FireRay()
    {
        timer = 0;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, HitscanWeapon.range))
        { // Raycast forward from camera.
            if (hit.transform.tag == "Enemy")
            { // Ray hit an enemy, hurt enemy.
                hit.transform.GetComponent<EnemyController>().Health -= HitscanWeapon.damage;
            }
        }

        RuntimeManager.PlayOneShot(HitscanWeapon.sound, "Intensity", intensities[(int)Weapon.Hitscan], transform.position);
    }
}
