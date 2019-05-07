using UnityEngine;

public class EliteEnemy : EnemyController
{
    public float windUp = 0.33f;

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) > range)
            { // Enemy is not within attacking range of target, move towards target.
                agent.isStopped = false;
                agent.SetDestination(target.position);
                anim.SetBool("Attacking", false);
            }
            else
            { // Enemy is within attacking range of target, stop and begin attacking target.
                agent.isStopped = true;

                if (attackTimer >= cooldown)
                { // Attack is off cooldown.
                    anim.SetBool("Attacking", true);
                    attackTimer = 0;
                    FMODUnity.RuntimeManager.PlayOneShotAttached(enemyAttackEventRef, gameObject);
                    Invoke("Attack", windUp);
                }
            }
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("Attacking", false);
        }
    }

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, target.position) < range)
            player.Health -= damage;

        anim.SetBool("Attacking", false);
    }

    public override void Die()
    {
        GameManager.totalEliteKills++;
        ScoreCounter.Score += scoreValue;

        base.Die();
    }
}
