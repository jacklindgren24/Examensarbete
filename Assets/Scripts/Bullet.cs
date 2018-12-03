using UnityEngine;

public class Bullet : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        { // Hit enemy, hurt and destroy self.
            other.gameObject.GetComponent<EnemyController>().Health -= ProjectileWeapon.damage;
            Destroy(gameObject);
        }
        else
        { // Did not hit enemy, destroy self.
            Destroy(gameObject);
        }
    }
}
