using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string EnemySpawn;
    [FMODUnity.EventRef]
    public string EnemyDeath;
    [FMODUnity.EventRef]
    public string enemyFootstepsEv;
    FMOD.Studio.EventInstance enemyFootstep;

    [FMODUnity.EventRef]
    public string PlayerHit;

    int health = 100;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0) Die();
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

        FMODUnity.RuntimeManager.PlayOneShot(EnemySpawn, transform.position);

        enemyFootstep = FMODUnity.RuntimeManager.CreateInstance(enemyFootstepsEv);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(enemyFootstep, GetComponent<Transform>(), GetComponent<Rigidbody>());
        EnemyFootstep();
    }

    public void EnemyFootstep()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        enemyFootstep.getPlaybackState(out state);

        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING) enemyFootstep.start();
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
            FMODUnity.RuntimeManager.PlayOneShot(PlayerHit, transform.position);
        }
    }

    void Die()
    {
        Destroy(gameObject);
        FMODUnity.RuntimeManager.PlayOneShot(EnemyDeath, transform.position);
        enemyFootstep.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
