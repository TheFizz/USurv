using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Approach,
    Attack,
    Retreat
}
public class RangedEnemy : NewEnemyBase
{
    public EnemyState State = EnemyState.Approach;
    Coroutine attack;
    bool attacking = false;
    public GameObject bulletPrefab;
    private float attackSpeed = 1;
    private float bulletSpeed = 15;
    public override void Update()
    {
        HandleEffects();

        var distanceToPlayer = Vector3.Distance(transform.position, MainTarget.position);
        switch (State)
        {
            case EnemyState.Approach:
                _canMove = true;
                _target = MainTarget.position;
                if (distanceToPlayer <= 15)
                    State = EnemyState.Attack;
                break;
            case EnemyState.Attack:
                _canMove = false;
                _RB.velocity = Vector3.zero;
                _target = MainTarget.position;
                if (!attacking)
                {
                    attack = StartCoroutine(AttackCR(attackSpeed));
                    attacking = !attacking;
                }
                if (distanceToPlayer <= 8)
                {
                    State = EnemyState.Retreat;
                    StopCoroutine(attack);
                    attacking = !attacking;
                }
                else if (distanceToPlayer >= 18)
                {
                    State = EnemyState.Approach;
                    StopCoroutine(attack);
                    attacking = !attacking;
                }
                break;
            case EnemyState.Retreat:
                _canMove = true;
                _target = (transform.position - MainTarget.position).normalized * 9000;
                if (distanceToPlayer >= 15)
                    State = EnemyState.Attack;
                break;
        }

        if (_canMove)
            MoveTo(_target);
        LookAt(_target);
    }

    IEnumerator AttackCR(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time / 2);
            Attack();
            yield return new WaitForSeconds(time / 2);
        }
    }
    void Attack()
    {
        var source = transform.position;
        source.y = 1;
        var sourceAngles = transform.rotation.eulerAngles;
        var bullet = Instantiate(bulletPrefab, source, Quaternion.Euler(sourceAngles));
        bullet.GetComponent<ProjectileBehaviourEnemy>().Setup(ID, bulletSpeed, 25, source);
    }
}

