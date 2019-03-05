using UnityEngine;

public class RangedEnemy : EnemyController
{
    [Space(15)]
    public Transform projectileSpawn;
    public GameObject projectilePrefab;
    public float projectileSpeed = 60;
    public float inaccuracy = 5;
    public float stopDelay = 0.5f;

	void Update()
    {
		if (!Physics.Linecast(transform.position, target.position, ~0, QueryTriggerInteraction.Ignore) 
            || Vector3.Distance(transform.position, target.position) > range)
        { // View of target is blocked or enemy is out of attack range.
            agent.isStopped = false;
            agent.SetDestination(target.position);

            attackTimer = 0;
        }
        else
        {
            Invoke("Stop", stopDelay);

            attackTimer += Time.deltaTime;
            if (attackTimer >= cooldown)
            { // Attack is off cooldown.
                Attack();
            }
        }
	}
    
    void Stop()
    {
        agent.isStopped = true;
    }

    protected override void Attack()
    {
        Quaternion rot = Quaternion.Euler(Random.Range(0, inaccuracy), Random.Range(0, inaccuracy), 0);

        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, rot);

        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * projectileSpeed;

        FMODUnity.RuntimeManager.PlayOneShotAttached(enemyAttackEventRef, gameObject);
    }

    public override void Die()
    {
        GameManager.totalRangedKills++;
        //MobSpawner.activeMobs--;
        ScoreCounter.Score += 25;

        base.Die();
    }
}
