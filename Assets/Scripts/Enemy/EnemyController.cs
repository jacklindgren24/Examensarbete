using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public abstract class EnemyController : MonoBehaviour {

    public static List<GameObject> enemies = new List<GameObject>();

    int health;
    public int Health
    {
        get { return health; }
        set
        {
            int old = health;
            health = value;

            if (health <= 0)
            {
                Die();
            }
            else if (health < old)
            {
                windUpTimer = 0;
                RuntimeManager.PlayOneShot(enemyHitEventRef, transform.position);
            }
        }
    }

    [EventRef]
    public string enemySpawnEventRef;
    [EventRef]
    public string enemyDeathEventRef;
    [EventRef]
    public string enemyHitEventRef;
    [EventRef]
    public string enemyAttackEventRef;
    [EventRef]
    public string enemyFootstepsEventRef;

    FMOD.Studio.EventInstance enemyFootstepsEv;

    public int baseHealth = 100;
    public int damage = 34;
    public float itemDropChance = 10;
    public float cooldown = 1;
    public float range = 3;
    public GameObject healthPickupPrefab;

    protected float attackTimer = 0;
    protected float windUpTimer = 0;

    protected NavMeshAgent agent;
    protected PlayerController player;
    protected Transform target;

    protected abstract void Attack();

    protected virtual void Start()
    {
        Health = baseHealth;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        target = player.gameObject.transform;

        RuntimeManager.PlayOneShot(enemySpawnEventRef, transform.position);

        enemyFootstepsEv = RuntimeManager.CreateInstance(enemyFootstepsEventRef);
        RuntimeManager.AttachInstanceToGameObject(enemyFootstepsEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        EnemyFootstep();

        enemies.Add(gameObject);
    }

    void EnemyFootstep()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        enemyFootstepsEv.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) enemyFootstepsEv.start();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        { // Collided with player, hurt player.
            other.gameObject.GetComponent<PlayerController>().Health -= damage;
        }
    }

    void SpawnHealth()
    {
        Instantiate(healthPickupPrefab, transform.position, transform.rotation);
    }

    public virtual void Die()
    {
        if (gameObject.name.Contains("Mob"))
        {
            GameManager.totalMobKills++;
            MobSpawner.activeMobs--;
            ScoreCounter.Score += 25;
        }
        else
        {
            GameManager.totalEliteKills++;
            EliteSpawner.activeElites--;
            ScoreCounter.Score += 50;
        }

        RuntimeManager.PlayOneShot(enemyDeathEventRef, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        float roll = Random.Range(0, 101);
        if (roll <= itemDropChance) SpawnHealth();

        Destroy(gameObject);
    }

    void OnDisable()
    {
        enemies.Remove(gameObject);
    }
}
