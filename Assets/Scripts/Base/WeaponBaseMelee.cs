using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseMelee : WeaponBase
{

    [SerializeField] protected GameObject _damageArc;
    protected override void Attack()
    {
        var go = Instantiate(_damageArc, _source.position, _source.rotation, _source);
        go.transform.Rotate(90f, 0, 0);

        var forward = _source.forward;
        var arcValue = Mathf.Cos((_weaponDataModified.AttackArc / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(_source.position.x, 0, _source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, _weaponDataModified.AttackRange, _weaponDataModified.EnemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            base.Attack();

            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - sourceFloored).normalized;
            var a = enemyPos.x;
            var dot = Vector3.Dot(vectorToCollider, forward); //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
            {
                Debug.Log("Cur: " + arcValue);

                Debug.Log("Target:" + dot);
            }
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(_weaponDataModified.AttackDamage);
            }
        }
        if (hitEnemies.Length > 0)
            _heat.AddHeat(hitEnemies.Length);
    }

    private void OnDrawGizmos()
    {
        /*
        var sourceFloored = _source.position;
        sourceFloored.y = 0;
        Gizmos.DrawWireSphere(sourceFloored, _weaponDataModified.attackRange);
        Gizmos.DrawWireSphere(sourceFloored, 7f);
        */
    }
}
