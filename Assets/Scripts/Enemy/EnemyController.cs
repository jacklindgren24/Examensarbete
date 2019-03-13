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
                hitScript.HitMarker();
                Die();
            }
            else if (health < old)
            {
                hitScript.HitMarker();
                windUpTimer = 0;
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
    protected float windUpTimer = 0;

    protected NavMeshAgent agent;
    protected PlayerController player;
    protected Transform target;
    protected Rigidbody rb;
    protected Animator anim;
    protected HitScript hitScript;

    public FMOD.Studio.EventInstance enemyFootstepsEv;

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
        Health = baseHealth;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        target = player.gameObject.transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        hitScript = FindObjectOfType<HitScript>();

        agent.speed += Random.Range(-speedRandomness, speedRandomness);

        RuntimeManager.PlayOneShot(enemySpawnEventRef, transform.position);

        enemyFootstepsEv = RuntimeManager.CreateInstance(enemyFootstepsEventRef);
        RuntimeManager.AttachInstanceToGameObject(enemyFootstepsEv, transform, rb);
        PlayFootstep();

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
        Vector3 pos = transform.position;
        pos.y = 1.5f;
        Instantiate(healthPickupPrefab, pos, transform.rotation);
    }

    public virtual void Die()
    {
        anim.SetTrigger("Death");

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
        else
        {
            MobSpawner.activeMobs--;
            //print(MobSpawner.activeMobs);
        }

        enemies.Remove(gameObject);
    }
}
