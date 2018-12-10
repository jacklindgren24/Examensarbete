using UnityEngine;

public class Bullet : MonoBehaviour {

    [FMODUnity.EventRef]
    public string EnemyHit;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        { // Hit enemy, hurt and destroy self.
            other.GetComponent<EnemyController>().Health -= ProjectileWeapon.damage;
            Destroy(gameObject);
            FMODUnity.RuntimeManager.PlayOneShot(EnemyHit, transform.position);
        }
        else
        { // Did not hit enemy, destroy self.
            Destroy(gameObject);

        }
    }
}
