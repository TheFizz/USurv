using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKama : MonoBehaviour, IWeapon
{
    public BaseWeaponSO WeaponData;
    private Transform _source;
    public void Attack()
    {
        var forward = transform.forward;
        var arcValue = (ConvertAngleToValue(WeaponData.attackArc / 2) * -1);
        var sourceFloored = new Vector3(_source.position.x, 0, _source.position.z);
        var error = 0.0001f;

        //StartCoroutine(ShowMeleeAttack(.2f));

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, WeaponData.attackRange, WeaponData.enemyLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            Vector3 enemyPos = hitEnemy.transform.position;
            Vector3 enemyPosFloored = new Vector3(enemyPos.x, 0, enemyPos.z);
            Vector3 vectorToCollider = (enemyPosFloored - sourceFloored).normalized;
            var dot = Vector3.Dot(vectorToCollider, forward) + error; //1 = right in front, -1 = right behind
            if (hitEnemy.name == "tpoint")
                Debug.Log(dot);
            if (dot >= arcValue)
            {
                var enemy = hitEnemy.GetComponent<EnemyBase>();
                enemy.Damage(WeaponData.attackDamage);
            }
        }
    }

    public void StartAttack(Transform source)
    {
        _source = source;
        InvokeRepeating("Attack", 1, WeaponData.attackSpeed);
    }

    public void StopAttack(Transform source)
    {
        CancelInvoke();
    }
    private static float ConvertAngleToValue(float degrees)
    {
        degrees %= 360f;
        if (degrees > 180f)
            degrees = 360f - degrees;
        float value = (degrees / 180f) * 2f - 1f;
        return value;
    }
}
