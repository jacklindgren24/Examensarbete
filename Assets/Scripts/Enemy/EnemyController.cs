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

            if (health <= 0) Die();
            else if (health < old) RuntimeManager.PlayOneShot(enemyHit, transform.position);
        }
    }

    public int damage = 34;
    public float cooldown = 1;
    public float pushback = 5;
    public float range = 4;

    float timer = 0;

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
        timer += Time.deltaTime;

        if (agent != null && target != null)
        {
            if (Vector3.Distance(transform.position, target.position) > range)
            { // Enemy is not within attacking range of target, move towards target.
                //agent.enabled = true;
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            { // Enemy is within attacking range of target, stop and attack target.
                //agent.enabled = false;
                agent.isStopped = true;
                if (timer >= cooldown) Attack();
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
        timer = 0;
        GameObject player = GameObject.FindWithTag("Player");

        player.GetComponent<PlayerController>().Health -= damage;
        player.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * pushback, ForceMode.Impulse);
    }

    void Die()
    {
        Destroy(gameObject);
        RuntimeManager.PlayOneShot(enemyDeath, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
