using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class PlayerController : MonoBehaviour {

    public enum Weapon { Projectile, Hitscan, Melee };

    public bool godMode = false;
    public int baseHealth = 100;
    public int maxHealth = 200;
    public float invincibleDuration = 1;
    int health;
    public int Health
    {
        get { return health; }
        set
        {
            int old = health;
            health = godMode ? health : Mathf.Clamp(value, 0, maxHealth);

            if (playerHealthBar != null) playerHealthBar.UpdateHealthBar();

            if (health <= 0)
            {
                StartCoroutine(Death(cam.transform.position, cam.transform.rotation));
            }
            else if (health < old)
            {
                if (invCo != null) StopCoroutine(invCo);
                invCo = StartCoroutine(Invincible());
                RuntimeManager.PlayOneShot(playerHurt, transform.position);
            }
        }
    }

    [EventRef]
    public string playerHurt;
    [EventRef]
    public string playerDie;
    
    [Space(15)]

    public Weapon weapon = Weapon.Projectile;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    float gunTimer = 0;
    float meleeTimer = 0;
    static float[] intensities = new float[] { 100, 100, 100 };

    Camera cam;
    LayerMask meleeMask;
    PlayerHealthBar playerHealthBar;
    Coroutine invCo;

    void Start()
    {
        cam = Camera.main;
        meleeMask = LayerMask.GetMask("Enemy");
        playerHealthBar = GameObject.FindWithTag("GameCanvas").GetComponentInChildren<PlayerHealthBar>();

        Health = baseHealth;
    }

    void Update()
    {
        gunTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        { // Fire (or melee if selected).
            switch (weapon)
            {
                case Weapon.Projectile:
                    if (gunTimer >= ProjectileWeapon.cooldown) FireBullet();
                    break;
                case Weapon.Hitscan:
                    if (gunTimer >= HitscanWeapon.cooldown) FireRay();
                    break;
                case Weapon.Melee:
                    if (meleeTimer >= MeleeWeapon.cooldown) Melee();
                    break;
            }
        }
        else if (Input.GetButtonDown("Switch"))
        { // Switch weapon.
            weapon = weapon == Weapon.Projectile ? Weapon.Hitscan : Weapon.Projectile;
            gunTimer = 0;
        }
        else if (Input.GetButtonDown("Melee") && meleeTimer >= MeleeWeapon.cooldown)
        { // Melee.
            RuntimeManager.PlayOneShot(MeleeWeapon.sound, "Intensity", intensities[(int)Weapon.Melee], transform.position);
            Invoke("Melee", MeleeWeapon.delay);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        { // Increase intensity by 50 up to a maximum of 100 (guns).
            intensities[(int)weapon] = Mathf.Clamp(intensities[(int)weapon] + 50, 0, 100);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        { // Decrease intensity by 50 down to a minimum of 0 (guns).
            intensities[(int)weapon] = Mathf.Clamp(intensities[(int)weapon] - 50, 0, 100);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        { // Increase intensity by 50 up to a maximum of 100 (melee).
            intensities[(int)Weapon.Melee] = Mathf.Clamp(intensities[(int)Weapon.Melee] + 50, 0, 100);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        { // Decrease intensity by 50 down to a minimum of 0 (melee).
            intensities[(int)Weapon.Melee] = Mathf.Clamp(intensities[(int)Weapon.Melee] - 50, 0, 100);
        }
    }

    void FireBullet()
    {
        gunTimer = 0;

        RaycastHit hit;
        Vector3 dir = bulletSpawn.transform.forward;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
        { // Player is aiming at an object, get direction from bullet spawn to point of raycast hit.
            dir = (hit.point - bulletSpawn.transform.position).normalized;
            //Debug.DrawLine(cam.transform.position, hit.point, Color.red, 2, false);
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = dir.normalized * ProjectileWeapon.velocity;
        Destroy(bullet, ProjectileWeapon.lifetime);

        RuntimeManager.PlayOneShot(ProjectileWeapon.sound, "Intensity", intensities[(int)Weapon.Projectile], transform.position);

        GameManager.totalProjectilesShot++;
    }

    void FireRay()
    {
        gunTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, HitscanWeapon.range))
        { // Raycast forward from camera.
            if (hit.transform.tag == "Enemy")
            { // Ray hit an enemy, hurt enemy.
                hit.transform.GetComponent<EnemyController>().Health -= HitscanWeapon.damage;
                GameManager.totalHitscanHits++;
            }
        }

        RuntimeManager.PlayOneShot(HitscanWeapon.sound, "Intensity", intensities[(int)Weapon.Hitscan], transform.position);

        GameManager.totalHitscanShots++;
    }

    void Melee()
    {
        meleeTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, cam.transform.forward, out hit, MeleeWeapon.range, meleeMask))
        { // Ray hit an enemy, hurt enemy.
            hit.transform.GetComponent<EnemyController>().Health -= MeleeWeapon.damage;
            hit.transform.GetComponent<Rigidbody>().AddForce(transform.forward * MeleeWeapon.knockback, ForceMode.Impulse);
            GameManager.totalMeleeHits++;
        }

        GameManager.totalMeleeAttacks++;
    }

    public static void SetIntensities(float projectile, float hitscan, float melee)
    {
        intensities[(int)Weapon.Projectile] = projectile;
        intensities[(int)Weapon.Hitscan] = hitscan;
        intensities[(int)Weapon.Melee] = melee;
    }

    IEnumerator Death(Vector3 camPos, Quaternion camRot)
    {
        Destroy(GameObject.FindWithTag("GameCanvas"));

        AsyncOperation loading = SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loading.isDone);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameOver"));

        Destroy(transform.GetChild(0).gameObject);

        GameOver.deathSnapEv = RuntimeManager.CreateInstance(GameOver.deathSnapshot);
        GameOver.deathSnapEv.start();

        RuntimeManager.PlayOneShot(playerDie, transform.position);

        Camera.main.transform.position = camPos;
        Camera.main.transform.rotation = camRot;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Destroy(gameObject);
    }

    IEnumerator Invincible()
    {
        godMode = true;
        yield return new WaitForSeconds(invincibleDuration);
        godMode = false;
    }
}
