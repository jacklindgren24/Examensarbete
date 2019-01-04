﻿using UnityEngine;

public class Bullet : MonoBehaviour {

    int layerMask = ~0;
    float radius = 0.1f;

    void FixedUpdate()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Ignore);

        if (results.Length > 0)
        {
            foreach (Collider result in results)
            {
                if (result.gameObject.tag == "Enemy")
                { // Hit enemy, hurt and destroy self.
                    result.GetComponent<EnemyController>().Health -= ProjectileWeapon.damage;
                    Destroy(gameObject);
                    return;
                }
            }

            // Did not hit enemy, destroy self.
            Destroy(gameObject);
        }
    }
}
