using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int health = 100;
    public int Health
    {
        get { return health; }
        set
        {
            health = godMode ? 100 : value;
            if (health <= 0) Die();
        }
    }

    public bool godMode = false;

    void Die()
    {
        Destroy(gameObject);
    }
}
