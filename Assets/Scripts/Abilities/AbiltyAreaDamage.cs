using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbiltyAreaDamage", menuName = "Abilities/AreaDamage")]
public class AbiltyAreaDamage : AbilityBase
{
    public override void Use(Transform source)
    {
        var sourceFloored = source.position;
        sourceFloored.y = 0;

        Instantiate(AbilityGraphics, source.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(sourceFloored, Stats.Find(x => x.Parameter == StatParam.AbilityRange).Value, TargetLayer);
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<NewEnemyBase>();
            enemy.Damage(Stats.Find(x => x.Parameter == StatParam.AttackDamage).Value, false);
        }
    }
}
