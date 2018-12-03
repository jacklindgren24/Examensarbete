using UnityEngine;

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
