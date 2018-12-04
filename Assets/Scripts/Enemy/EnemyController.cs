using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    [FMODUnity.EventRef]
    public string EnemySpawn;
    [FMODUnity.EventRef]
    public string EnemyDeath;

    public int health = 100;
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
            other.gameObject.GetComponent<Rigidbody>().AddForce(-GetComponent<Rigidbody>().velocity * pushback, ForceMode.VelocityChange);
        }
    }

    void Die()
    {
        Destroy(gameObject);
        FMODUnity.RuntimeManager.PlayOneShot(EnemyDeath, transform.position);
    }
}
