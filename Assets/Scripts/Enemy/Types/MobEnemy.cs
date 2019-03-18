using UnityEngine;

public class MobEnemy : EnemyController
{
    public float windUp = 0.25f;

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
                anim.SetBool("Attacking", false);
            }
            else
            { // Enemy is within attacking range of target, stop and begin attacking target.
                agent.isStopped = true;

                if (attackTimer >= cooldown)
                { // Attack is off cooldown.
                    windUpTimer += Time.deltaTime;
                    anim.SetBool("Attacking", true);

                    if (windUpTimer >= windUp)
                    { // Attack has wound up.
                        anim.SetBool("Attacking", false);
                        Attack();
                    }
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
        windUpTimer = 0;
        attackTimer = 0;

        player.Health -= damage;

        FMODUnity.RuntimeManager.PlayOneShotAttached(enemyAttackEventRef, gameObject);
    }

    public override void Die()
    {
        GameManager.totalMobKills++;
        ScoreCounter.Score += scoreValue;

        base.Die();
    }
}
