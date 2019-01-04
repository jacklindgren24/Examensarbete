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
    public float pushback = 5;

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
        if (agent != null && target != null) agent.SetDestination(target.position);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        { // Collided with player, hurt player.
            other.gameObject.GetComponent<PlayerController>().Health -= damage;
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * pushback, ForceMode.VelocityChange);
        }
    }

    void Die()
    {
        Destroy(gameObject);
        RuntimeManager.PlayOneShot(enemyDeath, transform.position);
        enemyFootstepsEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
