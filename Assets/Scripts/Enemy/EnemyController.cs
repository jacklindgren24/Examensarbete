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

    [Space(15)]

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
    protected Rigidbody rb;

    public FMOD.Studio.EventInstance enemyFootstepsEv;

    protected abstract void Attack();

    void OnEnable()
    {
        if (GetType() == typeof(EliteEnemy))
        {
            EliteSpawner.activeElites++;
            //print(EliteSpawner.activeElites);
        }
        else if (GetType() == typeof(RangedEnemy))
        {
            RangedSpawner.activeRangedEnemies++;
            //print(RangedSpawner.activeRangedEnemies);
        }
        else
        {
            MobSpawner.activeMobs++;
            //print(MobSpawner.activeMobs);
        }
    }

    protected virtual void Start()
    {
        Health = baseHealth;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        target = player.gameObject.transform;
        rb = GetComponent<Rigidbody>();

        RuntimeManager.PlayOneShot(enemySpawnEventRef, transform.position);

        enemyFootstepsEv = RuntimeManager.CreateInstance(enemyFootstepsEventRef);
        RuntimeManager.AttachInstanceToGameObject(enemyFootstepsEv, transform, rb);
        PlayFootstep();

        enemies.Add(gameObject);
    }

    void LateUpdate()
    {
        if (!agent.isStopped && target != null) PlayFootstep();
        else enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void PlayFootstep()
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
        RuntimeManager.PlayOneShot(enemyDeathEventRef, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        float roll = Random.Range(0, 101);
        if (roll <= itemDropChance) SpawnHealth();

        Destroy(gameObject);
    }

    void OnDisable()
    {
        if (GetType() == typeof(EliteEnemy))
        {
            EliteSpawner.activeElites--;
            //print(EliteSpawner.activeElites);
        }
        else if (GetType() == typeof(RangedEnemy))
        {
            RangedSpawner.activeRangedEnemies--;
            //print(RangedSpawner.activeRangedEnemies);
        }
        else
        {
            MobSpawner.activeMobs--;
            //print(MobSpawner.activeMobs);
        }

        enemies.Remove(gameObject);
    }
}
