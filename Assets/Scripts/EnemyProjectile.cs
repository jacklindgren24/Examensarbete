using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    const int layerMask = ~0;
    const float radius = 0.1f;

    public int damage;

    void FixedUpdate()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Ignore);

        if (results.Length > 0)
        {
            foreach (Collider result in results)
            {
                if (result.gameObject.tag == "Player")
                { // Hit player, hurt and destroy self.
                    result.GetComponent<PlayerController>().Health -= damage;
                    Destroy(gameObject);
                    return;
                }
            }

            // Did not hit player, destroy self.
            Destroy(gameObject);
        }
    }
}
