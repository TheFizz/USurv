using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseRanged : WeaponBase
{
    protected override void Attack()
    {
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (Attack)");
            return;
        }

        if (_weaponData is not WeaponBaseRangedSO)
        {
            Debug.LogError(name + " Weapon Data tpe mismatch");
            return;
        }

        WeaponBaseRangedSO wd = (WeaponBaseRangedSO)_weaponData;
        var projectile = (GameObject)Instantiate(wd.projectile, _source.position, _source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            wd.projectileSpeed,
            wd.pierceCount,
            wd.attackRange,
            wd.attackDamage,
            _source.position
            );

    }
}
