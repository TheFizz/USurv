using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseMelee : WeaponBase
{

    [SerializeField] private GameObject _damageArc;
    protected override void Attack()
    {
        var go = Instantiate(_damageArc, Source.position, Source.rotation, Source);
        go.transform.Rotate(90f, 0, 0);

        var forward = Source.forward;
        var arcValue = Mathf.Cos((WeaponDataModified.AttackArc / 2) * Mathf.Deg2Rad);
        var sourceFloored = new Vector3(Source.position.x, 0, Source.position.z);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponDataModified.AttackRange, WeaponDataModified.EnemyLayer);
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
                var enemy = hitEnemy.GetComponent<NewEnemyBase>();
                enemy.Damage(WeaponDataModified.AttackDamage);
            }
        }
        if (hitEnemies.Length > 0)
            Heat.AddHeat(hitEnemies.Length);
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
