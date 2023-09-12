using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseRanged : WeaponBase
{
    protected override void Attack()
    {
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (Attack)");
            return;
        }

        if (_weaponDataModified is not WeaponRangedSO)
        {
            Debug.LogError(name + " Weapon Data type mismatch");
            return;
        }

        WeaponRangedSO wd = (WeaponRangedSO)_weaponDataModified;
        var projectile = (GameObject)Instantiate(wd.projectile, _source.position, _source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            wd.projectileSpeed,
            wd.pierceCount,
            wd.AttackRange,
            wd.AttackDamage,
            _source.position
            );

    }
}
