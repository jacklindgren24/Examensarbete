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
                windUpTimer = 0;
                agent.SetDestination(target.position);
            }
            else
            { // Enemy is within attacking range of target, stop and begin attacking target.
                agent.isStopped = true;

                if (attackTimer >= cooldown)
                { // Attack is off cooldown.
                    windUpTimer += Time.deltaTime;
                    if (windUpTimer >= windUp)
                    { // Attack has wound up.
                        Attack();
                    }
                }
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    protected override void Attack()
    {
        anim.SetInteger("Random Attack", Random.Range(1, 4));
        anim.SetTrigger("Attack");

        windUpTimer = 0;
        attackTimer = 0;

        player.Health -= damage;

        //FMODUnity.RuntimeManager.PlayOneShotAttached(enemyAttackEventRef, gameObject);
    }

    public override void Die()
    {
        GameManager.totalEliteKills++;
        ScoreCounter.Score += scoreValue;

        base.Die();
    }
}
