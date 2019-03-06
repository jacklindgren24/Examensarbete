using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    const int layerMask = ~0;
    const float radius = 0.1f;

    void FixedUpdate()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Ignore);

        if (results.Length > 0)
        {
            foreach (Collider result in results)
            {
                if (result.gameObject.tag == "Enemy")
                { // Hit enemy, hurt and destroy self.
                    result.GetComponentInParent<EnemyController>().Health -= ProjectileWeapon.damage;
                    GameManager.totalProjectilesHit++;
                    Destroy(gameObject);
                    return;
                }
            }

            // Did not hit enemy, destroy self.
            Destroy(gameObject);
        }
    }
}
