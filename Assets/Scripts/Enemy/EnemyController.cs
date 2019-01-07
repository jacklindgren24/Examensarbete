using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class EnemyController : MonoBehaviour {

    [EventRef]
    public string enemySpawn;
    [EventRef]
    public string enemyDeath;
    [EventRef]
    public string enemyHit;
    [EventRef]
    public string enemyFootsteps;
    FMOD.Studio.EventInstance enemyFootstepsEv;

    int health = 100;
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
                RuntimeManager.PlayOneShot(enemyHit, transform.position);
            }
        }
    }

    public int damage = 34;
    public float spawnChance = 100;
    public float cooldown = 1;
    public float windUp = 0.5f;
    public float pushback = 5;
    public float range = 4;



    public GameObject healthpickupPrefab;

    float attackTimer = 0;
    float windUpTimer = 0;

    NavMeshAgent agent;
    Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;

        RuntimeManager.PlayOneShot(enemySpawn, transform.position);

        enemyFootstepsEv = RuntimeManager.CreateInstance(enemyFootsteps);
        RuntimeManager.AttachInstanceToGameObject(enemyFootstepsEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        EnemyFootstep();
    }

    public void EnemyFootstep()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        enemyFootstepsEv.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) enemyFootstepsEv.start();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (agent != null && target != null)
        {
            if (Vector3.Distance(transform.position, target.position) > range)
            { // Enemy is not within attacking range of target, move towards target.
                agent.isStopped = false;
                windUpTimer = 0;
                agent.SetDestination(target.position);
            }
            else
            { // Enemy is within attacking range of target, stop and begin attacking target.
                agent.isStopped = true;

                if (attackTimer >= cooldown)
                { // Attack is off cooldown.
                    windUpTimer += Time.deltaTime;
                    if (windUpTimer >= windUp)
                    { // Attack has wound up.
                        Attack();
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        { // Collided with player, hurt player.
            other.gameObject.GetComponent<PlayerController>().Health -= damage;
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * pushback, ForceMode.Impulse);
        }
    }

    void Attack()
    {
        windUpTimer = 0;
        attackTimer = 0;
        GameObject player = GameObject.FindWithTag("Player");

        player.GetComponent<PlayerController>().Health -= damage;
        player.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * pushback, ForceMode.Impulse);
    }

    void SpawnHealth()
    {
             Instantiate(healthpickupPrefab, transform.position, transform.rotation);
    }

    void Die()
    {
        Destroy(gameObject);
        RuntimeManager.PlayOneShot(enemyDeath, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        float roll = Random.Range(0, 100);
        if (roll <= spawnChance) SpawnHealth();
    }
}
