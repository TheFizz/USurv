using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : MonoBehaviour, IWeapon
{
    public BaseRangedWeaponSO WeaponData;
    private Transform _source;
    public void Attack()
    {
        var projectile = (GameObject)Instantiate(WeaponData.projectile, _source.position, _source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            WeaponData.projectileSpeed,
            WeaponData.pierceCount,
            WeaponData.attackRange,
            WeaponData.attackDamage,
            _source.position
            );
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
}
