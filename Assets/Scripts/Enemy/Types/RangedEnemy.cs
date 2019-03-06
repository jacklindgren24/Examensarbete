using UnityEngine;

public class RangedEnemy : EnemyController
{
    [Space(15)]
    public Transform projectileSpawn;
    public GameObject projectilePrefab;
    public float projectileSpeed = 50;
    public float inaccuracy = 3;
    public float stopDelay = 0.5f;

    void Update()
    {
        if (target != null)
        {
            if (Physics.Linecast(transform.position, target.position, ~LayerMask.GetMask("Enemy", "Player"), QueryTriggerInteraction.Ignore)
                || Vector3.Distance(transform.position, target.position) > range)
            { // View of target is blocked or enemy is out of attack range.
                CancelInvoke();
                agent.isStopped = false;
                agent.SetDestination(target.position);

                attackTimer = 0;

                transform.LookAt(rb.velocity);
                projectileSpawn.LookAt(rb.velocity);
            }
            else
            {
                transform.LookAt(target.position);
                projectileSpawn.LookAt(target.position);

                Invoke("Stop", stopDelay);

                attackTimer += Time.deltaTime;
                if (attackTimer >= cooldown)
                { // Attack is off cooldown.
                    Attack();
                }
            }
        }
        else
        {
            agent.isStopped = true;
        }
	}
    
    void Stop()
    {
        agent.isStopped = true;
    }

    protected override void Attack()
    {
        attackTimer = 0;

        float x = Random.Range(-inaccuracy, inaccuracy) * 0.5f;
        float y = Random.Range(-inaccuracy, inaccuracy);
        Quaternion rot = Quaternion.Euler(projectileSpawn.eulerAngles.x + x, projectileSpawn.eulerAngles.y + y, projectileSpawn.eulerAngles.z);

        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, rot);
        proj.GetComponent<EnemyProjectile>().damage = damage;
        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * projectileSpeed;
        Destroy(proj, 2);

        //FMODUnity.RuntimeManager.PlayOneShotAttached(enemyAttackEventRef, gameObject);
    }

    public override void Die()
    {
        GameManager.totalRangedKills++;
        ScoreCounter.Score += 25;

        base.Die();
    }
}
