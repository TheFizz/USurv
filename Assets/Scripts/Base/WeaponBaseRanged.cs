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

        if (WeaponData is not WeaponRangedSO)
        {
            Debug.LogError(name + " Weapon Data type mismatch");
            return;
        }

        WeaponRangedSO wd = (WeaponRangedSO)WeaponData;
        var projectile = (GameObject)Instantiate(wd.Projectile, Source.position, Source.rotation);
        var movScript = projectile.GetComponent<ProjectileBehaviour>();

        movScript.Setup(
            wd.GetStat(StatParam.ProjectileSpeed).Value,
            Mathf.RoundToInt(wd.GetStat(StatParam.PierceCount).Value),
            wd.GetStat(StatParam.AttackRange).Value,
            wd.GetStat(StatParam.AttackDamage).Value,
            Source.position
            );

        Heat.AddHeat(1);
    }
}
