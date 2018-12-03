using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float lifetime = 2;
    public float velocity = 50;
    public float cooldown = 0.2f;

    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && timer > cooldown) // Fire.
        {
            Fire();
        }
    }

    void Fire()
    {
        timer = 0;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * velocity;
        Destroy(bullet, lifetime);
    }
}
