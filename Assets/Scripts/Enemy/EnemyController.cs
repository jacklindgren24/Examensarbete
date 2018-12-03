using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

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

    NavMeshAgent agent;
    Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (agent != null) agent.SetDestination(target.position);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        { // Collided with player, hurt player.
            other.gameObject.GetComponent<PlayerController>().Health -= damage;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
