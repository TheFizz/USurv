using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseRanged : WeaponBase
{
    protected override void Attack()
    {
        base.Attack();

        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning(name + " is not active! (Attack)");
            return;
        }

        if (WeaponDataModified is not WeaponRangedSO)
        {
            Debug.LogError(name + " Weapon Data type mismatch");
            return;
        }

        WeaponRangedSO wd = (WeaponRangedSO)WeaponDataModified;
        var projectile = (GameObject)Instantiate(wd.projectile, Source.position, Source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            wd.projectileSpeed,
            wd.pierceCount,
            wd.AttackRange,
            wd.AttackDamage,
            Source.position
            );

        Heat.AddHeat(wd.pierceCount);
    }
}
