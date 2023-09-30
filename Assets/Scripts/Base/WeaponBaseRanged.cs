using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseRanged : WeaponBase
{
    private float coneTreshold = 9;
    protected override void Attack()
    {
        if (HeatStatus == HeatStatus.Overheated || HeatStatus == HeatStatus.Cooling)
            return;
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

        WeaponRangedSO weaponData = (WeaponRangedSO)WeaponData;

        var sourceAngles = Source.rotation.eulerAngles;
        var projectile = Instantiate(weaponData.Projectile, Source.position, Quaternion.Euler(sourceAngles));
        projectile.GetComponent<ProjectileBehaviour>().Setup(this, Source);

        float cone = weaponData.GetStat(StatParam.AttackCone).Value;
        float projectileTiers = (cone - (cone % coneTreshold)) / coneTreshold;
        float degDev = 0;
        for (int i = 0; i < projectileTiers; i++)
        {
            degDev += coneTreshold / 2;
            for (int j = 0; j < 2; j++)
            {
                var mult = 1;
                if (j == 1)
                    mult = -1;

                var tmpAngles = sourceAngles;
                tmpAngles.y += degDev * mult;

                projectile = Instantiate(weaponData.Projectile, Source.position, Quaternion.Euler(tmpAngles));
                projectile.GetComponent<ProjectileBehaviour>().Setup(this, Source);
            }
        }

        AddHeat(1);
    }
}
