using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    const float radius = 0.15f;

    void FixedUpdate()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, radius, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

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

            Destroy(gameObject); // Did not hit enemy, destroy self.
        }
    }
}
