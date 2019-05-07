using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;

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
                hitScript.HitMarker();
                Die();
            }
            else if (health < old)
            {
                hitScript.HitMarker();
                anim.SetTrigger("Hurt");
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
    public float cooldown = 1;
    public float range = 3;
    public float speedRandomness = 1;
    public int scoreValue;
    public float itemDropChance = 10;
    public GameObject healthPickupPrefab;

    protected float attackTimer = 0;

    protected NavMeshAgent agent;
    protected PlayerController player;
    protected Transform target;
    protected Rigidbody rb;
    protected Animator anim;
    protected HitScript hitScript;

    public EventInstance enemyFootstepsEv;

    protected abstract void Attack();

    void OnEnable()
    {
        if (GetType() == typeof(EliteEnemy))
        {
            EliteSpawner.activeElites++;
            //print(EliteSpawner.activeElites);
        }
        else
        {
            MobSpawner.activeMobs++;
            //print(MobSpawner.activeMobs);
        }
    }

    protected virtual void Start()
    {
        int roll = Random.Range(0, 3);
        Health = roll != 0 ? baseHealth : baseHealth + 8;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        target = player.gameObject.transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        hitScript = FindObjectOfType<HitScript>();

        agent.speed += Random.Range(-speedRandomness, speedRandomness);

        EventInstance spawnEv = RuntimeManager.CreateInstance(enemySpawnEventRef);
        RuntimeManager.AttachInstanceToGameObject(spawnEv, transform, rb);
        spawnEv.start();
        spawnEv.release();

        enemyFootstepsEv = RuntimeManager.CreateInstance(enemyFootstepsEventRef);
        FMOD.ATTRIBUTES_3D attr = RuntimeUtils.To3DAttributes(transform.position);
        enemyFootstepsEv.set3DAttributes(attr);

        enemies.Add(gameObject);
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isPaused)
        {
            if (!agent.isStopped && target != null)
            { // Moving.
                anim.SetBool("Moving", true);
                PlayFootstep();
            }
            else
            { // Stationary.
                anim.SetBool("Moving", false);
                enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    void PlayFootstep()
    {
        FMOD.ATTRIBUTES_3D attr = RuntimeUtils.To3DAttributes(transform.position);
        enemyFootstepsEv.set3DAttributes(attr);

        PLAYBACK_STATE state;
        enemyFootstepsEv.getPlaybackState(out state);

        if (state != PLAYBACK_STATE.PLAYING && state != PLAYBACK_STATE.STARTING)
        {
            enemyFootstepsEv.start();
        }
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
        Vector3 pos = transform.position;
        pos.y += 1.5f;
        Instantiate(healthPickupPrefab, pos, transform.rotation);
    }

    public virtual void Die()
    {
        anim.SetTrigger("Death");

        RuntimeManager.PlayOneShot(enemyDeathEventRef, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

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
        else
        {
            MobSpawner.activeMobs--;
            //print(MobSpawner.activeMobs);
        }

        if (!RuntimeManager.IsQuitting())
        {
            enemyFootstepsEv.release();
        }

        enemies.Remove(gameObject);
    }
}
